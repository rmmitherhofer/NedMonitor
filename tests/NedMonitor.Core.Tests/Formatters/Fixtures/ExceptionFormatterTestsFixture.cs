namespace NedMonitor.Core.Tests.Formatters.Fixtures;

public sealed class ExceptionFormatterTestsFixture
{
    public Exception CreateNestedException()
        => new Exception("outer", new ApplicationException("inner", new InvalidOperationException("base")));
}
