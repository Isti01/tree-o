namespace Logic.Event.World.Tower {
public class TowerCooledDownEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerCooledDownEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}
}
