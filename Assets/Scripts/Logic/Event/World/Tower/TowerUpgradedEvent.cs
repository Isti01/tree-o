using Logic.Data.World;

namespace Logic.Event.World.Tower {

/// <summary>
/// Raised when a <see cref="Data.World.Tower"/> is upgraded,
/// causing its <see cref="Data.World.Tower.Type"/> to change.
/// </summary>
public class TowerUpgradedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }
	public ITowerTypeData BeforeUpgradeType { get; }

	public TowerUpgradedEvent(Data.World.Tower tower, ITowerTypeData beforeUpgradeType) {
		Tower = tower;
		BeforeUpgradeType = beforeUpgradeType;
	}
}

}
