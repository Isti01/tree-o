using Logic.Data.World;

namespace Logic.Event.World.Barrack {

/// <summary>
/// Raised when an entry is added to <see cref="Data.World.Barrack.CheckPoints"/>.
/// The entry is the last entry in the collection at this point: it got added to the end.
/// </summary>
public class BarrackCheckpointCreatedEvent : BaseEvent, IBarrackEvent {
	public BarrackCheckpointCreatedEvent(Data.World.Barrack barrack, TilePosition checkpointPosition) {
		Barrack = barrack;
		CheckpointPosition = checkpointPosition;
	}

	public Data.World.Barrack Barrack { get; }

	public TilePosition CheckpointPosition { get; }
}

}
