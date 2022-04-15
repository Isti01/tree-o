using Logic.Data.World;

namespace Logic.Event.World.Unit {

/// <summary>
/// Raised when a <see cref="Data.World.Unit"/> is added to <see cref="Data.World.Barrack.QueuedUnits"/>.
/// </summary>
public class UnitQueuedEvent : BaseEvent, IUnitTypeEvent {
	public IUnitTypeData Type { get; }
	public Data.World.Barrack Barrack { get; }

	public UnitQueuedEvent(IUnitTypeData type, Data.World.Barrack barrack) {
		Type = type;
		Barrack = barrack;
	}
}

}
