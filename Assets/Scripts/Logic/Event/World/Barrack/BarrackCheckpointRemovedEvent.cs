using Logic.Data.World;

namespace Logic.Event.World.Barrack {
public class BarrackCheckpointRemovedEvent : BaseEvent, IBarrackEvent {
	public BarrackCheckpointRemovedEvent(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}

	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }
}
}
