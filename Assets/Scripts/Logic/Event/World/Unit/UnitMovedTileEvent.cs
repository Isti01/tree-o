﻿using Logic.Data.World;

namespace Logic.Event.World.Unit {

public class UnitMovedTileEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }
	public TilePosition OldPosition { get; }

	public UnitMovedTileEvent(Data.World.Unit unit, TilePosition oldPosition) {
		Unit = unit;
		OldPosition = oldPosition;
	}
}

}
