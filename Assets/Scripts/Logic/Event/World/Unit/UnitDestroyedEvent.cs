using Logic.Data.World;

namespace Logic.Event.World.Unit {

/// <summary>
/// Raised when a <see cref="Data.World.Unit"/> is removed from <see cref="GameWorld.Units"/>.
/// </summary>
public class UnitDestroyedEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }

	public UnitDestroyedEvent(Data.World.Unit unit) {
		Unit = unit;
	}
}

}
