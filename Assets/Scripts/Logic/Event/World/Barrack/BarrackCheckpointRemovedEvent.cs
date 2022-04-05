using Logic.Data.World;

namespace Logic.Event.World.Barrack {
public class BarrackRemovedEvent : BaseEvent, IBarrackEvent {
	public BarrackRemovedEvent(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}

	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }
}
}
