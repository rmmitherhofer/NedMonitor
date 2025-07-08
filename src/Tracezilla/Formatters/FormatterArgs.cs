namespace Tracezilla.Formatters;

/// <summary>
/// Arguments used for formatting, encapsulating state, exception, and default value.
/// </summary>
public class FormatterArgs
{
    /// <summary>
    /// Gets the state object associated with the formatting operation.
    /// </summary>
    public object State { get; }

    /// <summary>
    /// Gets the exception to be formatted.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the default string value used when no formatting is applied.
    /// </summary>
    public string DefaultValue { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="FormatterArgs"/> using specified options.
    /// </summary>
    /// <param name="options">Options containing the state, exception, and default value.</param>
    internal FormatterArgs(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

        State = options.State;
        Exception = options.Exception;
        DefaultValue = options.DefaultValue;
    }

    /// <summary>
    /// Options used to create a <see cref="FormatterArgs"/> instance.
    /// </summary>
    internal class CreateOptions
    {
        /// <summary>
        /// Gets or sets the state object.
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// Gets or sets the exception instance.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the default string value.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
