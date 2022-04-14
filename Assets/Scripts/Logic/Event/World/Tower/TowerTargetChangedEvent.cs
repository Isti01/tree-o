namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when the value of <see cref="Data.World.Tower.Target"/> changes.
/// (Also raised when the old and/or new value is null.)
/// </summary>
public class TowerTargetChangedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public Data.World.Unit OldTarget { get; }

	public TowerTargetChangedEvent(Data.World.Tower tower, Data.World.Unit oldTarget) {
		Tower = tower;
		OldTarget = oldTarget;
	}
}

}
