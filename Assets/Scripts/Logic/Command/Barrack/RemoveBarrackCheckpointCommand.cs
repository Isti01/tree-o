using Logic.Data.World;

namespace Logic.Command.Barrack {

public class RemoveBarrackCheckpointCommand : BaseCommand {
	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }

	public RemoveBarrackCheckpointCommand(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}
}

}
