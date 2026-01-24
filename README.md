# NedMonitor

Biblioteca .NET para observabilidade centralizada de aplicações, com captura de requisições HTTP, logs, exceções, notificações e métricas de dependências (HTTP e banco). O projeto é modular e focado em aplicações ASP.NET Core, com integrações para EF Core, Dapper e HttpClient.

## Visão geral do ecossistema

Este repositório contém os seguintes projetos/pacotes:

- **NedMonitor**: camada principal para ASP.NET Core. Registra serviços, middlewares, provedor de log e o pipeline de envio de dados ao serviço remoto.
- **NedMonitor.Core**: modelos e configurações centrais (snapshot, settings e enums).
- **NedMonitor.Http**: handler de HttpClient para logar chamadas HTTP de saída.
- **NedMonitor.EF**: interceptor do EF Core que conta e registra consultas executadas.
- **NedMonitor.Dapper**: wrapper/conexão que intercepta operações do Dapper e registra consultas.

## Como funciona (fluxo resumido)

1. **Middlewares** capturam requisição/resposta, exceções e habilitam buffering do corpo. O `NedMonitorMiddleware` mede o tempo de execução e enfileira um `Snapshot` ao final da requisição.
2. O **Snapshot** consolida dados de requisição/resposta, usuário, logs, dependências HTTP e consultas ao banco extraídas do `HttpContext.Items`.
3. O **NedMonitorQueue** guarda snapshots em memória, e o **NedMonitorBackgroundService** consome esses itens e envia para a API remota.
4. O **NedMonitorApplication** monta o payload final conforme flags de execução e chama o serviço HTTP.

## Por que usar

- Consolida a telemetria da aplicação em um único payload por requisição.
- Ativa recursos por configuração, sem acoplar módulos que não são usados.
- Oferece integrações opcionais para HTTP, EF Core e Dapper.

## Configuração mínima (NedMonitor base)

Registre o módulo base e habilite o pipeline. Isso ativa a captura do contexto da requisição, o monitoramento de exceções (quando habilitado) e o envio do snapshot.

### Registrar serviços (container)

```csharp
builder.Services.AddNedMonitor(builder.Configuration, options =>
{
    options.Formatter = (args) =>
    {
        if (args.Exception == null)
            return args.DefaultValue;

        string exceptionStr = new ExceptionFormatter().Format(args.Exception);
        return string.Join(Environment.NewLine, new[] { args.DefaultValue, exceptionStr });
    };
});
```

### Pipeline (ordem recomendada)

`UseNedMonitor` deve vir antes da maior parte do pipeline.  
`UseNedMonitorMiddleware` deve vir depois do middleware de exceções do cliente.

```csharp
ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

app.UseRouting();

app.UseNedMonitor();

app.TryUseMiddleware<RequestIndetityMiddleware>();

app.UseNotificationConfig();

app.UseLogDecoratorConfig();

app.TryUseMiddleware<ExceptionMiddleware>();

app.UseNedMonitorMiddleware();

app.UseSwaggleBox(app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseEndpoints(endpoints => endpoints.MapControllers());

return app;
```

## Requisitos mínimos de configuração

O bloco `NedMonitor` no `appsettings.json` precisa conter pelo menos:

- `ProjectId` (GUID válido)
- `ProjectType` (enum `ProjectType`)
- `RemoteService.BaseAddress`
- `RemoteService.Endpoints.NotifyLogContext`

A validação ocorre na inicialização da aplicação.

## Instalação (NuGet)

Instale apenas os módulos necessários para a sua aplicação.

Links NuGet:
- https://www.nuget.org/packages/NedMonitor
- https://www.nuget.org/packages/NedMonitor.Http
- https://www.nuget.org/packages/NedMonitor.EF
- https://www.nuget.org/packages/NedMonitor.Dapper

```bash
dotnet add package NedMonitor
dotnet add package NedMonitor.Http
dotnet add package NedMonitor.EF
dotnet add package NedMonitor.Dapper
```

Se preferir, use `PackageReference`:

```xml
<ItemGroup>
  <PackageReference Include="NedMonitor" Version="x.y.z" />
  <PackageReference Include="NedMonitor.Http" Version="x.y.z" />
  <PackageReference Include="NedMonitor.EF" Version="x.y.z" />
  <PackageReference Include="NedMonitor.Dapper" Version="x.y.z" />
</ItemGroup>
```

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
- Registra opções e validação (`NedMonitorSettingsValidation`).
- Adiciona o provedor de logs quando habilitado.
- Configura `NedMonitorBackgroundService` e fila.
- Registra o HttpClient para envio ao serviço remoto.

### 2) Habilitar middlewares

```csharp
app.UseNedMonitor();
app.UseNedMonitorMiddleware();
```

- `UseNedMonitor` insere os middlewares de captura de requisição/resposta.
- `UseNedMonitorMiddleware` insere o middleware de captura de exceções quando habilitado em `ExecutionModeSettings`.

## Como habilitar cada recurso

As seções abaixo mostram o mínimo necessário por recurso/módulo.

### Exceções

- Habilite `EnableMonitorExceptions` em `ExecutionMode`.
- Se sua aplicação possui middleware de exceções, mantenha `UseNedMonitorMiddleware` depois dele.
- Fluxo recomendado: a exceção é interceptada pelo `NedMonitorExceptionCaptureMiddleware` (que a repassa) e, em seguida, tratada pelo middleware de exceções do cliente. Sem middleware do cliente, o ASP.NET Core aplica o tratamento padrão.

### Notificações

- O `INotificationHandler` padrão vem do pacote Zypher.Notifications; você pode usar esse ou implementar o seu.
- Implementações customizadas devem seguir o contrato esperado pelo NedMonitor.
- As notificações precisam ser registradas no `HttpContext.Items` com a chave `__Notifications__`.
- Habilite `EnableMonitorNotifications` em `ExecutionMode`.

### Logs (ILogger)

- Não há configuração adicional: o adapter integra diretamente com `ILogger`.
- Habilite `EnableMonitorLogs` em `ExecutionMode`.

### Requisições HTTP de saída

1. Instale o módulo `NedMonitor.Http`.
2. Registre o handler:

```csharp
builder.Services.AddNedMonitorHttp();
builder.Services.AddHttpClient("default")
    .AddHttpMessageHandler<NedMonitorHttpLoggingHandler>();
```

3. Habilite `EnableMonitorHttpRequests` em `ExecutionMode`.

### Banco de dados (EF Core)

1. Instale o módulo `NedMonitor.EF`.
2. Registre os interceptors:

```csharp
builder.Services.AddNedMonitorEfInterceptors();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetRequiredService<EfQueryCounter>());
});
app.UseNedMonitorEfInterceptors();
```

3. Habilite `DataInterceptors.EF.Enabled` e configure `CaptureOptions`.

### Banco de dados (Dapper)

1. Instale o módulo `NedMonitor.Dapper`.
2. Registre e use o wrapper:

```csharp
builder.Services.AddNedMonitorDapperInterceptors();

using var connection = new CountingDbConnection(
    innerConnection,
    sp.GetRequiredService<IQueryCounter>(),
    sp.GetRequiredService<IHttpContextAccessor>(),
    sp.GetRequiredService<IOptions<NedMonitorSettings>>()
);

var dapper = sp.GetRequiredService<INedDapperWrapper>();
var result = await dapper.QueryAsync<MyDto>(connection, sql, param);

app.UseNedMonitorDapperInterceptors();
```

3. Habilite `DataInterceptors.Dapper.Enabled` e configure `CaptureOptions`.

### Controle de coleta (flags)

Em `ExecutionModeSettings` e `DataInterceptors`, o cliente decide o que será coletado:

- `EnableNedMonitor` (liga/desliga o módulo base)
- `EnableMonitorExceptions`
- `EnableMonitorNotifications`
- `EnableMonitorLogs`
- `EnableMonitorHttpRequests`
- `EnableMonitorDbQueries`
- `DataInterceptors.EF.Enabled`, `DataInterceptors.Dapper.Enabled`

## Documentação por projeto

### NedMonitor (principal)

**Responsabilidade:** integração completa com ASP.NET Core, envio de logs para a API e orquestração do pipeline.

**Principais componentes:**
- `NedMonitorMiddleware` (mede duração e enfileira snapshot).
- `NedMonitorExceptionCaptureMiddleware` (captura exceções não esperadas).
- `NedMonitorBackgroundService` + `NedMonitorQueue` (envio assíncrono).
- `NedMonitorApplication` (monta payload e envia via HTTP).

**Quando usar:** sempre que precisar integrar uma aplicação ASP.NET Core ao NedMonitor (inclusive em conjunto com EF/Dapper/HttpClient).

### NedMonitor.Core

**Responsabilidade:** modelos e configurações centrais usados por todos os pacotes.

**Destaques:**
- `Snapshot` agrega dados de requisição/resposta, usuário, logs, dependências e banco de dados.
- `NedMonitorSettings` define ProjectId/ProjectType, flags de execução e endpoints remotos.
- `ExecutionModeSettings` controla o que será monitorado.

**Quando usar:** referência obrigatória para qualquer integração NedMonitor (dependência dos demais pacotes).

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

**Responsabilidade:** interceptor do EF Core que conta e registra consultas executadas, incluindo SQL, parâmetros, tempo e erros.

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

**Responsabilidade:** wrapper para Dapper que intercepta chamadas, incrementa contador e registra consultas conforme configuração.

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
- **Controle o tamanho do corpo** com `HttpLogging.MaxResponseBodySizeInMb` e `CaptureResponseBody`.
- **Mascaramento de dados sensíveis** é configurável via `SensitiveDataMasking`.

## Licença

Distribuído sob a licença MIT.
