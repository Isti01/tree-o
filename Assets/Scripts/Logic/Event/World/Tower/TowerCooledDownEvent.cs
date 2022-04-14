namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when the value of <see cref="Data.World.Tower.RemainingCooldownTime"/> reaches 0.
/// </summary>
public class TowerCooledDownEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerCooledDownEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
