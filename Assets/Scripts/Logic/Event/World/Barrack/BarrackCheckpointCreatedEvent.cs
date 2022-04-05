using Logic.Data.World;

namespace Logic.Event.World.Barrack {
public class BarrackCheckpointCreatedEvent : BaseEvent, IBarrackEvent {
	public BarrackCheckpointCreatedEvent(Data.World.Barrack barrack, TilePosition position) {
		Position = position;
		Barrack = barrack;
	}

	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }
}
}
