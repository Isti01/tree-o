namespace Logic.Event.World.Tower {
public class TowerTargetChangedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public Data.World.Unit OldTarget { get; }

	public TowerTargetChangedEvent(Data.World.Tower tower, Data.World.Unit oldTarget) {
		Tower = tower;
		OldTarget = oldTarget;
	}
}
}
