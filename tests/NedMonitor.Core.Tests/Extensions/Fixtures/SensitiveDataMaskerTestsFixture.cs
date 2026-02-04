using NedMonitor.Core.Extensions;
using NedMonitor.Core.Settings;

namespace NedMonitor.Core.Tests.Extensions.Fixtures;

public sealed class SensitiveDataMaskerTestsFixture
{
    internal SensitiveDataMasker CreateWithKeys(params string[] keys) =>
        new(new SensitiveDataMaskerSettings
        {
            SensitiveKeys = [.. keys],
            MaskValue = "***"
        });

    internal SensitiveDataMasker CreateWithPatterns(params string[] patterns) =>
        new(new SensitiveDataMaskerSettings
        {
            SensitivePatterns = [.. patterns],
            MaskValue = "***"
        });
}
