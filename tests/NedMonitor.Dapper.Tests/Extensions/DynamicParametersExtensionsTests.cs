using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Dapper.Extensions;
using Xunit.Abstractions;

namespace NedMonitor.Dapper.Tests.Extensions;

public class DynamicParametersExtensionsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null parameters, " +
        "When getting declared parameters, " +
        "Then it throws")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_Null_Throws()
    {
        //Given
        object? parameters = null;

        //When
        var act = () => DynamicParametersExtensions.GetDeclaredParameters(parameters!);

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given instance without dictionary, " +
        "When getting declared parameters, " +
        "Then it returns empty")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_NoDictionary_ReturnsEmpty()
    {
        //Given
        var parameters = new NoParameters();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters field dictionary, " +
        "When getting declared parameters, " +
        "Then it reads values")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_FieldDictionary_ReadsValues()
    {
        //Given
        var parameters = new WithParametersField();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("id");
        result["id"].Should().Be(123);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters property dictionary, " +
        "When getting declared parameters, " +
        "Then it reads values")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_PropertyDictionary_ReadsValues()
    {
        //Given
        var parameters = new WithParametersProperty();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("name");
        result["name"].Should().Be("ned");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given _parameters field dictionary, " +
        "When getting declared parameters, " +
        "Then it reads values")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_UnderscoreParametersField_ReadsValues()
    {
        //Given
        var parameters = new WithUnderscoreParametersField();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("code");
        result["code"].Should().Be("x");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given Parameters field dictionary, " +
        "When getting declared parameters, " +
        "Then it reads values")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_ParametersField_ReadsValues()
    {
        //Given
        var parameters = new WithParametersFieldNamedParameters();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("flag");
        result["flag"].Should().Be(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given empty parameters dictionary, " +
        "When getting declared parameters, " +
        "Then it returns empty")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_EmptyDictionary_ReturnsEmpty()
    {
        //Given
        var parameters = new WithEmptyParametersField();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters with null value, " +
        "When getting declared parameters, " +
        "Then it keeps null value")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_NullValue_MapsNull()
    {
        //Given
        var parameters = new WithNullParameterValue();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("nullParam");
        result["nullParam"].Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters with direct primitive value, " +
        "When getting declared parameters, " +
        "Then it returns null for missing value accessors")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_PrimitiveValue_ReturnsNull()
    {
        //Given
        var parameters = new WithPrimitiveParameterValue();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("primitive");
        result["primitive"].Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom dictionary field, " +
        "When getting declared parameters, " +
        "Then it reads values")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_CustomDictionaryField_ReadsValues()
    {
        //Given
        var parameters = new WithCustomDictionaryField();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("custom");
        result["custom"].Should().Be(456);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters with complex value, " +
        "When getting declared parameters, " +
        "Then it returns the original object")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_ComplexValue_ReturnsObject()
    {
        //Given
        var parameters = new WithComplexParameterValue();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("complex");
        result["complex"].Should().BeOfType<ComplexPayload>();
        ((ComplexPayload)result["complex"]!).Data.Should().Be("payload");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters with value field, " +
        "When getting declared parameters, " +
        "Then it reads the value field")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_ValueField_ReadsValue()
    {
        //Given
        var parameters = new WithValueFieldParameter();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("field");
        result["field"].Should().Be("field");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters with _value field, " +
        "When getting declared parameters, " +
        "Then it reads the _value field")]
    [Trait("Extensions", nameof(DynamicParametersExtensions))]
    public async Task GetDeclaredParameters_UnderscoreValueField_ReadsValue()
    {
        //Given
        var parameters = new WithUnderscoreValueFieldParameter();

        //When
        var result = DynamicParametersExtensions.GetDeclaredParameters(parameters);

        //Then
        result.Should().ContainKey("underscore");
        result["underscore"].Should().Be("underscore");
        await Task.CompletedTask;
    }

    private sealed class NoParameters
    {
    }

    private sealed class WithParametersField
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["id"] = new WithValueProperty(123)
        };
    }

    private sealed class WithParametersProperty
    {
        private System.Collections.IDictionary Parameters { get; } = new Dictionary<string, object?>
        {
            ["name"] = new WithValueField("ned")
        };
    }

    private sealed class WithUnderscoreParametersField
    {
        private readonly System.Collections.IDictionary _parameters = new Dictionary<string, object?>
        {
            ["code"] = new WithValueProperty("x")
        };
    }

    private sealed class WithParametersFieldNamedParameters
    {
        private readonly System.Collections.IDictionary Parameters = new Dictionary<string, object?>
        {
            ["flag"] = new WithValueProperty(true)
        };
    }

    private sealed class WithEmptyParametersField
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>();
    }

    private sealed class WithNullParameterValue
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["nullParam"] = null
        };
    }

    private sealed class WithPrimitiveParameterValue
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["primitive"] = 7
        };
    }

    private sealed class WithComplexParameterValue
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["complex"] = new WithValueProperty(new ComplexPayload("payload"))
        };
    }

    private sealed class WithCustomDictionaryField
    {
        private readonly System.Collections.IDictionary dict = new Dictionary<string, object?>
        {
            ["custom"] = new WithValueProperty(456)
        };
    }

    private sealed class WithValueFieldParameter
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["field"] = new WithValueField("field")
        };
    }

    private sealed class WithUnderscoreValueFieldParameter
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["underscore"] = new WithUnderscoreValueField("underscore")
        };
    }

    private sealed class ComplexPayload
    {
        public ComplexPayload(string data) => Data = data;
        public string Data { get; }
    }

    private sealed class WithValueProperty
    {
        public WithValueProperty(object value) => Value = value;
        public object Value { get; }
    }

    private sealed class WithValueField
    {
        private readonly object _value;
        public WithValueField(object value) => _value = value;
    }

    private sealed class WithUnderscoreValueField
    {
        private readonly object _value;
        public WithUnderscoreValueField(object value) => _value = value;
    }
}
