using Logic.Data.World;

namespace Logic.Event.World.Barrack {

/// <summary>
/// Raised when an entry is removed from <see cref="Data.World.Barrack.CheckPoints"/>.
/// The entry could have been removed from any position within the collection.
/// </summary>
public class BarrackCheckpointRemovedEvent : BaseEvent, IBarrackEvent {
	public BarrackCheckpointRemovedEvent(Data.World.Barrack barrack, TilePosition checkpointPosition) {
		Barrack = barrack;
		CheckpointPosition = checkpointPosition;
	}

	public Data.World.Barrack Barrack { get; }
	public TilePosition CheckpointPosition { get; }
}

}
