using Logic.Data.World;

namespace Logic.Event.World.Unit {
public class UnitDamagedEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }

	public Data.World.Tower Attacker { get; }

	public UnitDamagedEvent(Data.World.Unit unit, Data.World.Tower attacker) {
		Unit = unit;
		Attacker = attacker;
	}
}
}
