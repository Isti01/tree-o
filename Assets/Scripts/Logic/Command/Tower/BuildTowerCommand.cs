using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Tower {

public class BuildTowerCommand : BaseCommand<BuildTowerCommand.CommandResult> {
	public GameTeam Team { get; }
	public ITowerTypeData Type { get; }
	public TilePosition Position { get; }

	public BuildTowerCommand(GameTeam team, ITowerTypeData type, TilePosition position) {
		Team = team;
		Type = type;
		Position = position;
	}

	public sealed class CommandResult : ICommandResult {
		public static readonly CommandResult Success = new CommandResult();
		public static readonly CommandResult NotEnoughMoney = new CommandResult();
		public static readonly CommandResult TileAlreadyOccupied = new CommandResult();
		public static readonly CommandResult LeavesNoPathForUnit = new CommandResult();
		public static readonly CommandResult MiscFailure = new CommandResult();

		public bool IsSuccess => this == Success;

		private CommandResult() {
			//Disallow construction
		}
	}
}

}
