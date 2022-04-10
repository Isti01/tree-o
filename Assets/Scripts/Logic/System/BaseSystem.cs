using Logic.Event;

namespace Logic.System {

/// <summary>
/// Base class for all systems.
/// Systems are responsible for listening to events and invoking functions
/// in other classes based on the raised event. Systems are responsible
/// for connecting different classes.
/// Systems should be stateless if possible.
/// </summary>
internal abstract class BaseSystem {
	public abstract void RegisterListeners(EventDispatcher dispatcher);
}

}
