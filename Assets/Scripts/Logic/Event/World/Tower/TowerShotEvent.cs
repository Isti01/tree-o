namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when a <see cref="Data.World.Tower"/> shoots,
/// causing <see cref="Data.World.Tower.RemainingCooldownTime"/> to be updated.
/// Other events are expected to be raised in response to this event.
/// </summary>
public class TowerShotEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }
	public Data.World.Unit Target { get; }

	public TowerShotEvent(Data.World.Tower tower, Data.World.Unit target) {
		Tower = tower;
		Target = target;
	}
}

}
