namespace Logic.Event.World.Barrack {

/// <summary>
/// Base class for events that are about a <see cref="Data.World.Barrack"/>.
/// </summary>
public interface IBarrackEvent {
	public Data.World.Barrack Barrack { get; }
}

}
