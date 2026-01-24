using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Tests.Extensions.Fixtures;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Extensions;

public class SensitiveDataMaskerTests(ITestOutputHelper output, SensitiveDataMaskerTestsFixture fixture) : Test(output), IClassFixture<SensitiveDataMaskerTestsFixture>
{
    private readonly SensitiveDataMaskerTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given json with sensitive keys, " +
        "When masking string, " +
        "Then it masks sensitive values")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void MaskString_Json_MasksSensitiveKeys()
    {
        //Given
        var masker = _fixture.CreateWithKeys("password", "token");
        var input = "{\"user\":\"alice\",\"password\":\"p@ss\",\"nested\":{\"token\":\"abc\"}}";

        //When
        var output = masker.MaskString(input);

        //Then
        using var doc = JsonDocument.Parse(output);
        var root = doc.RootElement;
        root.GetProperty("password").GetString().Should().Be("***");
        root.GetProperty("nested").GetProperty("token").GetString().Should().Be("***");
    }

    [Fact(DisplayName =
        "Given nested dictionary with sensitive keys, " +
        "When masking object dictionary, " +
        "Then it masks values recursively")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void MaskObjectDictionary_MasksRecursively()
    {
        //Given
        var masker = _fixture.CreateWithKeys("secret");
        var input = new Dictionary<string, object?>
        {
            ["secret"] = "value",
            ["child"] = new Dictionary<string, object?>
            {
                ["secret"] = "nested"
            }
        };

        //When
        var result = masker.MaskObjectDictionary(input)!;

        //Then
        result["secret"].Should().Be("***");
        var child = result["child"].Should().BeOfType<Dictionary<string, object?>>().Which;
        child["secret"].Should().Be("***");
    }

    [Fact(DisplayName =
        "Given non-json string matching sensitive patterns, " +
        "When masking string, " +
        "Then it replaces matches with mask value")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void MaskString_UsesPatterns_WhenNotJson()
    {
        //Given
        var masker = _fixture.CreateWithPatterns("token=\\w+");

        //When
        var output = masker.MaskString("token=abc123; ok");

        //Then
        output.Should().Contain("***");
        output.Should().NotContain("abc123");
    }

    [Fact(DisplayName =
        "Given masking disabled, " +
        "When masking object, " +
        "Then it returns the input unchanged")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void Mask_Disabled_ReturnsInput()
    {
        //Given
        var masker = new SensitiveDataMasker(new NedMonitor.Core.Settings.SensitiveDataMaskerSettings
        {
            Enabled = false,
            SensitiveKeys = ["secret"],
            MaskValue = "***"
        });
        var input = new Dictionary<string, string> { ["secret"] = "value" };

        //When
        var result = masker.Mask(input);

        //Then
        result.Should().BeSameAs(input);
    }

    [Fact(DisplayName =
        "Given dictionary with list values and sensitive key, " +
        "When masking, " +
        "Then it masks the list values")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void Mask_ListDictionary_MasksValues()
    {
        //Given
        var masker = _fixture.CreateWithKeys("secret");
        var input = new Dictionary<string, List<string>>
        {
            ["secret"] = ["value"],
            ["ok"] = ["safe"]
        };

        //When
        var result = masker.Mask(input)!;

        //Then
        result["secret"].Should().ContainSingle().Which.Should().Be("***");
        result["ok"].Should().ContainSingle().Which.Should().Be("safe");
    }

    [Fact(DisplayName =
        "Given enumerable of key values with sensitive key, " +
        "When masking, " +
        "Then it masks the sensitive entry")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void Mask_EnumerableKeyValues_MasksSensitive()
    {
        //Given
        var masker = _fixture.CreateWithKeys("secret");
        IEnumerable<KeyValuePair<string, string>> input =
        [
            new("secret", "value"),
            new("ok", "safe")
        ];

        //When
        var result = masker.Mask(input) as IDictionary<string, string>;

        //Then
        result.Should().NotBeNull();
        result!["secret"].Should().Be("***");
        result["ok"].Should().Be("safe");
    }

    [Fact(DisplayName =
        "Given a cyclic object, " +
        "When masking complex object, " +
        "Then it returns the original instance")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void Mask_ComplexObject_WithCycle_ReturnsOriginal()
    {
        //Given
        var masker = _fixture.CreateWithKeys("secret");
        var node = new Node();
        node.Next = node;

        //When
        var result = masker.Mask(node);

        //Then
        result.Should().BeSameAs(node);
    }

    [Fact(DisplayName =
        "Given non-json string with no patterns, " +
        "When masking string, " +
        "Then it returns the same string")]
    [Trait("Extensions", nameof(SensitiveDataMasker))]
    public void MaskString_NoPatterns_ReturnsInput()
    {
        //Given
        var masker = _fixture.CreateWithPatterns();
        var input = "plain text";

        //When
        var output = masker.MaskString(input);

        //Then
        output.Should().Be(input);
    }

    private sealed class Node
    {
        public Node? Next { get; set; }
    }
}
