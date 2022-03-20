namespace Logic.Event.World.Tower {

public class TowerDestroyedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerDestroyedEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
