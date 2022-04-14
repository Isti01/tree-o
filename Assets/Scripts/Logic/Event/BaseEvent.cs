namespace Logic.Event {

/// <summary>
/// <para>Base class for all events.</para>
/// <para>Subclasses are usually named in past tense, because events are usually invoked
/// after the actions they are "advertising" have already finished.</para>
/// <para>When a subclass has a field that holds the old value of something,
/// then the current/actual value can usually be read from the object the event is about.</para>
/// </summary>
public abstract class BaseEvent {}

}
