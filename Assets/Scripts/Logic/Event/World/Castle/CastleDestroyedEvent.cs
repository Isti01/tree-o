namespace Logic.Event.World.Castle {

public class CastleDestroyedEvent : BaseEvent, ICastleEvent {
	public Data.World.Castle Castle { get; }

	public CastleDestroyedEvent(Data.World.Castle castle) {
		Castle = castle;
	}
}

}
