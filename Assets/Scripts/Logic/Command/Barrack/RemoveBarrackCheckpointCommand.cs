using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Barrack {

/// <summary>
/// Command for deleting an entry from <see cref="Data.World.Barrack.CheckPoints"/>
/// during the <see cref="GamePhase.Prepare"/> phase.
/// </summary>
public class RemoveBarrackCheckpointCommand : BaseCommand {
	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }

	public RemoveBarrackCheckpointCommand(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}
}

}
