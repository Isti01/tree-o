using Logic.Data.World;

namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when a <see cref="Data.World.Tower"/> is removed from the <see cref="GameWorld"/>.
/// </summary>
public class TowerDestroyedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerDestroyedEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
