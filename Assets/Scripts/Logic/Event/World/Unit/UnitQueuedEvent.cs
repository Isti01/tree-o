using Logic.Data.World;

namespace Logic.Event.World.Unit {

public class UnitQueuedEvent : BaseEvent, IUnitTypeEvent {
	public IUnitTypeData Type { get; }
	public Barrack Barrack { get; }

	public UnitQueuedEvent(IUnitTypeData type, Barrack barrack) {
		Type = type;
		Barrack = barrack;
	}
}

}
