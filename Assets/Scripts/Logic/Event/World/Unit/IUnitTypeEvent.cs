using Logic.Data.World;

namespace Logic.Event.World.Unit {

/// <summary>
/// Base class for events that are about a <see cref="IUnitTypeData"/> (or a <see cref="Data.World.Unit"/>).
/// </summary>
public interface IUnitTypeEvent {
	public IUnitTypeData Type { get; }
}

}
