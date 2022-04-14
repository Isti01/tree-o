﻿using Logic.Data.World;

namespace Logic.Event.World.Unit {

/// <summary>
/// Raised when the value of <see cref="Data.World.Unit.CurrentHealth"/> changes.
/// </summary>
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
