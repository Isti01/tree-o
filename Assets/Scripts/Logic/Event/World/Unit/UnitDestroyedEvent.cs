using Logic.Data.World;

namespace Logic.Event.World.Unit {

public class UnitDestroyedEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }

	public UnitDestroyedEvent(Data.World.Unit unit) {
		Unit = unit;
	}
}

}
