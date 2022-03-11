using Logic.Command;

namespace Logic.Handler {

/// <summary>
/// Base class for all handlers.
/// Handlers are responsible for consuming the different commands.
/// Handlers should be stateless if possible.
/// </summary>
public abstract class BaseHandler {
	public abstract void RegisterConsumers(CommandDispatcher dispatcher);
}

}
