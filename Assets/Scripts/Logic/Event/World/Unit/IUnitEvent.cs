namespace Logic.Event.World.Unit {

public interface IUnitEvent : IUnitTypeEvent {
	public Data.World.Unit Unit { get; }
}

}
