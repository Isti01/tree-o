using Logic.Data.World;

namespace Logic.Event.World.Barrack {
public class BarrackCheckpointCreatedEvent : BaseEvent, IBarrackEvent {
	public BarrackCheckpointCreatedEvent(Data.World.Barrack barrack, TilePosition checkpointPosition) {
		Barrack = barrack;
		CheckpointPosition = checkpointPosition;
	}

	public Data.World.Barrack Barrack { get; }

	public TilePosition CheckpointPosition { get; }
}
}
