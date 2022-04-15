using Logic.Command;

namespace Logic.Handler {

/// <summary>
/// Base class for all handlers.
/// Handlers are responsible for consuming the different commands.
/// Handlers should be stateless if possible.
/// </summary>
internal abstract class BaseHandler {
	/// <summary>
	/// Registers the command handler callbacks in this instance.
	/// </summary>
	/// <param name="dispatcher">the dispatcher in which to make the registration</param>
	public abstract void RegisterConsumers(CommandDispatcher dispatcher);
}

}
