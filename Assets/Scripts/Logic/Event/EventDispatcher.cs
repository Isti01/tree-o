using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Event {

/// <summary>
/// Class through which <see cref="BaseEvent"/> instances can be raised
/// and event listeners can be added.
/// <p>
/// Some notable design decisions:
/// <list type="bullet">
/// <item><description>When raising an event, the listeners of the superclasses are also invoked.</description></item>
/// <item><description>Exceptions thrown by the event listener aren't propagated
/// to the invoker of the event: they are handled by this class instead.</description></item>
/// </list>
/// </p>
/// </summary>
public class EventDispatcher {
	private readonly IDictionary<Type, IList<RegisteredListener>> _listeners =
		new Dictionary<Type, IList<RegisteredListener>>();

	private readonly Action<ICollection<EventInvocationException>> _eventRaisingErrorHandler;

	/// <summary>
	/// Creates and initializes a new instance.
	/// </summary>
	/// <param name="eventRaisingErrorHandler">handler of exception(s) (at least 1) that arise when
	/// events are raised (via <see cref="Raise"/>)</param>
	public EventDispatcher(Action<ICollection<EventInvocationException>> eventRaisingErrorHandler) {
		_eventRaisingErrorHandler = eventRaisingErrorHandler;
	}

	/// <summary>
	/// Same as calling <see cref="AddListener{T}(Ordering, Listener{T})"/> with <see cref="Ordering.Normal"/>.
	/// </summary>
	public void AddListener<T>(Listener<T> listener) where T : BaseEvent {
		AddListener(Ordering.Normal, listener);
	}

	/// <summary>
	/// Registers a new listener to the specified event.
	/// The same listener is allowed to listen to the same event multiple times.
	/// The same event is allowed to have multiple listeners.
	/// A listener is also called if a superclass of the specified event is raised,
	/// see <see cref="Raise"/> for more information about this.
	/// </summary>
	/// <param name="ordering">when the listener should be called relative to other listeners</param>
	/// <param name="listener">the consumer of the event</param>
	/// <typeparam name="T">the event to listen to</typeparam>
	public void AddListener<T>(Ordering ordering, Listener<T> listener) where T : BaseEvent {
		if (!_listeners.TryGetValue(typeof(T), out IList<RegisteredListener> list)) {
			list = new List<RegisteredListener>();
			_listeners[typeof(T)] = list;
		}

		list.Add(new RegisteredListener(ordering, listener));
	}

	/// <summary>
	/// Unregisters a listener of the specified event.
	/// The event must have the specified listener as one of its listeners.
	/// If the same listener was registered multiple times,
	/// then this method only removes one occurrence of the listener.
	/// </summary>
	/// <param name="ordering">the ordering of the listener that should be unregistered</param>
	/// <param name="listener">the listener to unregister</param>
	/// <typeparam name="T">the event to unregister the listener from</typeparam>
	/// <exception cref="IllegalListenerStateException">if the specified listener isn't
	/// among the listeners of the specified event</exception>
	public void RemoveListener<T>(Ordering ordering, Listener<T> listener) where T : BaseEvent {
		RegisteredListener registered = new RegisteredListener(ordering, listener);
		if (!_listeners.TryGetValue(typeof(T), out IList<RegisteredListener> list) || !list.Remove(registered)) {
			throw new IllegalListenerStateException($"{registered} isn't a registered listener of {typeof(T)}");
		}
	}

	/// <summary>
	/// Invokes all listeners which listen to the specified event or to any superclasses of the event.
	/// The order in which listeners are invoked is unspecified.
	/// Errors thrown by the listeners are delegated to the handler specified in this class' constructor.
	/// </summary>
	/// <param name="eventData">the event whose listeners to invoke</param>
	public void Raise(BaseEvent eventData) {
		List<EventInvocationException> errors = new List<EventInvocationException>();

		foreach (Ordering ordering in Enum.GetValues(typeof(Ordering))) {
			//Call listeners which listen to the exact event type or one of its superclasses
			Type type = eventData.GetType();
			do {
				// ReSharper disable once AssignNullToNotNullAttribute
				if (!_listeners.TryGetValue(type, out IList<RegisteredListener> listeners)) break;

				foreach (RegisteredListener registered in listeners.Where(x => x.Order == ordering)) {
					try {
						registered.Listener.DynamicInvoke(eventData);
					} catch (Exception e) {
						errors.Add(new EventInvocationException($"Error consuming {eventData} by {registered}", e));
					}
				}

				type = type.BaseType;
			} while (type != typeof(object));
		}

		if (errors.Count > 0) {
			_eventRaisingErrorHandler.Invoke(errors);
		}
	}

	/// <summary>
	/// Determines when a <see cref="Listener{T}"/> should be called
	/// relative to other listeners.
	/// The order when multiple listeners share the same enum value is unspecified.
	/// </summary>
	public enum Ordering {
		First,
		Sooner,
		Normal,
		Later,
		Last
	}

	/// <summary>
	/// Consumes an event and executes action(s) based on said event.
	/// </summary>
	/// <typeparam name="T">type of event being listened to</typeparam>
	public delegate void Listener<in T>(T eventArgs);

	/// <summary>
	/// Exception that signals that listeners aren't in the expected state.
	/// </summary>
	public class IllegalListenerStateException : Exception {
		public IllegalListenerStateException(string message) : base(message) {}
	}

	/// <summary>
	/// Exception that signals that event listener(s) failed exceptionally.
	/// </summary>
	public class EventInvocationException : Exception {
		public EventInvocationException(string message, Exception e) : base(message, e) {}
	}

	private readonly struct RegisteredListener {
		public Ordering Order { get; }
		public Delegate Listener { get; }

		public RegisteredListener(Ordering order, Delegate listener) {
			Order = order;
			Listener = listener;
		}

		public override string ToString() {
			return "{" + Order + ">>" + Listener + "}";
		}
	}
}

}
