namespace Logic.Event.World.Tower {

/// <summary>
/// Base class for events that are about a <see cref="Data.World.Tower"/>.
/// </summary>
public interface ITowerEvent {
	public Data.World.Tower Tower { get; }
}

}
