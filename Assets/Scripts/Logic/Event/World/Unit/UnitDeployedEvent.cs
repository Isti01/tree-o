using Logic.Data.World;

namespace Logic.Event.World.Unit {

/// <summary>
/// Raised when a <see cref="Data.World.Unit"/> is added to <see cref="GameWorld.Units"/>.
/// The unit is also removed from the referenced barrack's <see cref="Data.World.Barrack.QueuedUnits"/>.
/// </summary>
public class UnitDeployedEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }
	public Data.World.Barrack Barrack { get; }

	public UnitDeployedEvent(Data.World.Unit unit, Data.World.Barrack barrack) {
		Unit = unit;
		Barrack = barrack;
	}
}

}
