using Logic.Data;

namespace Logic.Command.Tower {

/// <summary>
/// Command for selling a <see cref="Tower"/> during the <see cref="GamePhase.Prepare"/> phase.
/// </summary>
public class DestroyTowerCommand : BaseCommand {
	public Data.World.Tower Tower { get; }

	public DestroyTowerCommand(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
