using Logic.Data.World;

namespace Logic.Event.World.Unit {

public class UnitDeployedEvent : BaseEvent, IUnitEvent {
	public IUnitTypeData Type => Unit.Type;
	public Data.World.Unit Unit { get; }
	public Barrack Barrack { get; }

	public UnitDeployedEvent(Data.World.Unit unit, Barrack barrack) {
		Unit = unit;
		Barrack = barrack;
	}
}

}
