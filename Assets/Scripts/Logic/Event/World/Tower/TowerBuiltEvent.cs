using Logic.Data.World;

namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when a <see cref="Data.World.Tower"/> is added to the <see cref="GameWorld"/>.
/// </summary>
public class TowerBuiltEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }

	public TowerBuiltEvent(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
