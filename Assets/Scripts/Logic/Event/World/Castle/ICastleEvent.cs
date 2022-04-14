namespace Logic.Event.World.Castle {

/// <summary>
/// Base class for events that are about a <see cref="Data.World.Castle"/>.
/// </summary>
public interface ICastleEvent {
	public Data.World.Castle Castle { get; }
}

}
