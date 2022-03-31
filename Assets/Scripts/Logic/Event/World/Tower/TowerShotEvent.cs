namespace Logic.Event.World.Tower {
public class TowerShotEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }
	public Data.World.Unit Target { get; }

	public TowerShotEvent(Data.World.Tower tower, Data.World.Unit target) {
		Tower = tower;
		Target = target;
	}
}
}
