# NedMonitor

Biblioteca .NET para observabilidade centralizada de aplicações, com captura de requisições HTTP, logs, exceções, notificações e métricas de dependências (HTTP e banco). O projeto é dividido em pacotes para uso modular em aplicações ASP.NET Core e integrações específicas (EF Core, Dapper e HttpClient).

## Visão geral do ecossistema

Este repositório contém os seguintes projetos/pacotes:

- **NedMonitor**: camada principal para ASP.NET Core. Registra serviços, middlewares, provedor de log e o pipeline de envio de dados ao serviço remoto.
- **NedMonitor.Core**: modelos e configurações centrais (snapshot, settings e enums).
- **NedMonitor.Http**: handler de HttpClient para logar chamadas HTTP de saída.
- **NedMonitor.EF**: interceptor do EF Core que conta e registra queries executadas.
- **NedMonitor.Dapper**: wrapper/connection que intercepta operações do Dapper e registra queries.

## Como funciona (fluxo resumido)

1. **Middlewares** capturam request/response, exceções e habilitam buffering do corpo. O `NedMonitorMiddleware` mede o tempo de execução e enfileira um `Snapshot` ao final da requisição.
2. O **Snapshot** consolida dados de request/response, usuário, logs, dependências HTTP e queries do banco extraídas do `HttpContext.Items`.
3. O **NedMonitorQueue** guarda snapshots em memória, e o **NedMonitorBackgroundService** consome esses itens e envia para a API remota.
4. O **NedMonitorApplication** monta o payload final conforme flags de execução e chama o serviço HTTP.

## Requisitos mínimos de configuração

O bloco `NedMonitor` no `appsettings.json` precisa conter pelo menos:

- `ProjectId` (GUID válido)
- `ProjectType` (enum `ProjectType`)
- `RemoteService.BaseAddress`
- `RemoteService.Endpoints.NotifyLogContext`

A validação ocorre no startup.

### Exemplo de configuração (appsettings.json)

```json
{
  "NedMonitor": {
    "ProjectId": "00000000-0000-0000-0000-000000000000",
    "ProjectType": "Api",
    "ExecutionMode": {
      "EnableNedMonitor": true,
      "EnableMonitorExceptions": true,
      "EnableMonitorNotifications": true,
      "EnableMonitorLogs": true,
      "EnableMonitorHttpRequests": true,
      "EnableMonitorDbQueries": true
    },
    "HttpLogging": {
      "CaptureResponseBody": true,
      "MaxResponseBodySizeInMb": 1,
      "CaptureCookies": false,
      "WritePayloadToConsole": false
    },
    "SensitiveDataMasking": {
      "Enabled": true,
      "SensitiveKeys": ["password", "token"],
      "SensitivePatterns": [],
      "MaskValue": "***REDACTED***"
    },
    "DataInterceptors": {
      "EF": {
        "Enabled": true,
        "CaptureOptions": ["Query", "Parameters", "Context", "ExceptionMessage"]
      },
      "Dapper": {
        "Enabled": true,
        "CaptureOptions": ["Query", "Parameters", "Context", "ExceptionMessage"]
      }
    },
    "RemoteService": {
      "BaseAddress": "https://sua-api-nedmonitor",
      "Endpoints": {
        "NotifyLogContext": "/api/v1/logs"
      }
    }
  }
}
```

Configurações disponíveis estão definidas em `NedMonitorSettings`, `ExecutionModeSettings`, `HttpLoggingSettings` e `SensitiveDataMaskerSettings`.

## Guia de uso rápido (ASP.NET Core)

### 1) Registrar serviços

```csharp
builder.Services.AddNedMonitor(builder.Configuration);
```

O método `AddNedMonitor`:
- registra opções e validação (`NedMonitorSettingsValidation`),
- adiciona o provedor de logs quando habilitado,
- configura `NedMonitorBackgroundService` e fila,
- registra o client HTTP para envio ao serviço remoto.

### 2) Habilitar middlewares

```csharp
app.UseNedMonitor();
app.UseNedMonitorMiddleware();
```

- `UseNedMonitor` insere os middlewares de captura de request/response.
- `UseNedMonitorMiddleware` insere o middleware de captura de exceções quando habilitado em `ExecutionModeSettings`.

## Documentação por projeto

### NedMonitor (principal)

**Responsabilidade:** integração completa com ASP.NET Core, envio de logs para a API e orquestração do pipeline.

**Principais componentes:**
- `NedMonitorMiddleware` (mede duração e enfileira snapshot).
- `NedMonitorExceptionCaptureMiddleware` (captura exceções não esperadas).
- `NedMonitorBackgroundService` + `NedMonitorQueue` (envio assíncrono).
- `NedMonitorApplication` (monta payload e envia via HTTP).

**Quando usar:** sempre que você precisar integrar uma aplicação ASP.NET Core ao NedMonitor (inclusive em conjunto com EF/Dapper/HttpClient).

### NedMonitor.Core

**Responsabilidade:** modelos e configurações centrais usados por todos os pacotes.

**Destaques:**
- `Snapshot` agrega dados de request/response, usuário, logs, dependências e banco.
- `NedMonitorSettings` define ProjectId/ProjectType, flags de execução e endpoints remotos.
- `ExecutionModeSettings` controla o que será monitorado.

**Quando usar:** referência obrigatória para qualquer integração NedMonitor (é dependência dos demais pacotes).

### NedMonitor.Http

**Responsabilidade:** captura requisições HTTP de saída via `HttpClient` (headers, corpo, tempo e erros) e armazena no `HttpContext`.

**Uso recomendado:**

1. Registrar o handler:

```csharp
builder.Services.AddNedMonitorHttp();
```

2. Inserir o handler no `HttpClient`:

```csharp
builder.Services.AddHttpClient("default")
    .AddHttpMessageHandler<NedMonitorHttpLoggingHandler>();
```

**Observações:** o handler só captura quando `EnableNedMonitor` e `EnableMonitorHttpRequests` estão ativos.

### NedMonitor.EF

**Responsabilidade:** interceptor do EF Core que conta e registra queries executadas, incluindo SQL, parâmetros, tempo e erros.

**Uso recomendado:**

1. Registrar serviços:

```csharp
builder.Services.AddNedMonitorEfInterceptors();
```

2. Adicionar o interceptor ao `DbContext`:

```csharp
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<EfQueryCounter>());
});
```

3. Ativar o middleware de reset de contadores por request:

```csharp
app.UseNedMonitorEfInterceptors();
```

**Observações:** o interceptor respeita `DataInterceptors.EF.Enabled` e `CaptureOptions` (Query, Parameters, Context, ExceptionMessage).

### NedMonitor.Dapper

**Responsabilidade:** wrapper para Dapper que intercepta chamadas, incrementa contador e registra queries conforme configuração.

**Uso recomendado:**

1. Registrar serviços:

```csharp
builder.Services.AddNedMonitorDapperInterceptors();
```

2. Criar a conexão interceptada:

```csharp
using var connection = new CountingDbConnection(
    innerConnection,
    sp.GetRequiredService<IQueryCounter>(),
    sp.GetRequiredService<IHttpContextAccessor>(),
    sp.GetRequiredService<IOptions<NedMonitorSettings>>()
);
```

3. Usar o wrapper para executar comandos:

```csharp
var dapper = sp.GetRequiredService<INedDapperWrapper>();
var result = await dapper.QueryAsync<MyDto>(connection, sql, param);
```

4. Habilitar reset por request:

```csharp
app.UseNedMonitorDapperInterceptors();
```

**Observações:** a captura é condicionada por `DataInterceptors.Dapper.Enabled` e `CaptureOptions`.

## Dicas e boas práticas

- **Habilite apenas o necessário** em `ExecutionModeSettings` para reduzir overhead.
- **Controle o tamanho do body** com `HttpLogging.MaxResponseBodySizeInMb` e `CaptureResponseBody`.
- **Mascaramento de dados sensíveis** é configurável via `SensitiveDataMasking`.

## Licença

Distribuído sob a licença MIT.
