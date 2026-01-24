using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Enums;

public class EnumValuesTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given capture options enum, " +
        "When checking values, " +
        "Then they match the expected flags")]
    [Trait("Enums", nameof(CaptureOptions))]
    public async Task CaptureOptions_Values_AreExpected()
    {
        //Given
        var none = CaptureOptions.None;

        //When
        var query = (int)CaptureOptions.Query;

        //Then
        ((int)none).Should().Be(0);
        query.Should().Be(1);
        ((int)CaptureOptions.Parameters).Should().Be(2);
        ((int)CaptureOptions.Context).Should().Be(4);
        ((int)CaptureOptions.ExceptionMessage).Should().Be(8);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given log attention level enum, " +
        "When checking values, " +
        "Then they match the expected order")]
    [Trait("Enums", nameof(LogAttentionLevel))]
    public async Task LogAttentionLevel_Values_AreExpected()
    {
        //Given
        var none = LogAttentionLevel.None;

        //When
        var high = (int)LogAttentionLevel.High;

        //Then
        ((int)none).Should().Be(0);
        high.Should().Be(3);
        ((int)LogAttentionLevel.Critical).Should().Be(4);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given project type enum, " +
        "When checking values, " +
        "Then they match the expected order")]
    [Trait("Enums", nameof(ProjectType))]
    public async Task ProjectType_Values_AreExpected()
    {
        //Given
        var undefined = ProjectType.Undefined;

        //When
        var api = (int)ProjectType.Api;

        //Then
        ((int)undefined).Should().Be(0);
        api.Should().Be(2);
        ((int)ProjectType.Test).Should().Be(12);
        await Task.CompletedTask;
    }
}
