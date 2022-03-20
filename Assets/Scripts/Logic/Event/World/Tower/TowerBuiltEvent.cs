namespace Logic.Event.World.Tower {

public class TowerBuiltEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerBuiltEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
