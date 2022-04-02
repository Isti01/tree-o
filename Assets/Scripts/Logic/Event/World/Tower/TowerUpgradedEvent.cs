using Logic.Data.World;

namespace Logic.Event.World.Tower {

public class TowerUpgradedEvent : BaseEvent, ITowerEvent {
	public Data.World.Tower Tower { get; }
	public ITowerTypeData BeforeUpgradeType { get; }

	public TowerUpgradedEvent(Data.World.Tower tower, ITowerTypeData beforeUpgradeType) {
		Tower = tower;
		BeforeUpgradeType = beforeUpgradeType;
	}
}

}
