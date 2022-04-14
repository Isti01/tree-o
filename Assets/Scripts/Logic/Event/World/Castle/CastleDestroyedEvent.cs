namespace Logic.Event.World.Castle {

/// <summary>
/// Raised when a castle's <see cref="Data.World.Castle.Health"/> reaches 0.
/// </summary>
public class CastleDestroyedEvent : BaseEvent, ICastleEvent {
	public Data.World.Castle Castle { get; }

	public CastleDestroyedEvent(Data.World.Castle castle) {
		Castle = castle;
	}
}

}
