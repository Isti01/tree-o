namespace Logic.Event.World.Unit {

/// <summary>
/// Base class for events that are about a <see cref="Data.World.Unit"/>.
/// </summary>
public interface IUnitEvent : IUnitTypeEvent {
	public Data.World.Unit Unit { get; }
}

}
