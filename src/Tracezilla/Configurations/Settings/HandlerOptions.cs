namespace Tracezilla.Configurations.Settings;

/// <summary>
/// Configuration for custom handlers in Tracezilla.
/// Allows registering functions for additional processing, such as exception detail enrichment.
/// </summary>
public class HandlerOptions
{
    /// <summary>
    /// Internal container that holds the configured handlers.
    /// </summary>
    internal HandlersContainer Handlers { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="HandlerOptions"/> with default handlers.
    /// </summary>
    public HandlerOptions() => Handlers = new HandlersContainer();

    /// <summary>
    /// Configures the handler responsible for adding custom details to an exception.
    /// </summary>
    /// <param name="handler">Function that takes an <see cref="Exception"/> and returns a string with additional details.</param>
    /// <returns>The <see cref="HandlerOptions"/> instance for chaining.</returns>
    public HandlerOptions AppendExceptionDetails(Func<Exception, string> handler)
    {
        if (handler is null) return this;

        Handlers.AppendExceptionDetails = handler;
        return this;
    }

    /// <summary>
    /// Internal container storing delegates for the configured handlers.
    /// </summary>
    internal class HandlersContainer
    {
        /// <summary>
        /// Function that adds additional details to an exception.
        /// </summary>
        public Func<Exception, string> AppendExceptionDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersContainer"/> with default values.
        /// </summary>
        public HandlersContainer()
        {
            AppendExceptionDetails = (ex) => null;
        }
    }
}
