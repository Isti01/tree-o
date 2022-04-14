using Logic.Data;

namespace Logic.Command.Tower {

/// <summary>
/// Command for upgrading a <see cref="Tower"/> during the <see cref="GamePhase.Prepare"/> phase.
/// </summary>
public class UpgradeTowerCommand : BaseCommand<UpgradeTowerCommand.CommandResult> {
	public Data.World.Tower Tower { get; }

	public UpgradeTowerCommand(Data.World.Tower tower) {
		Tower = tower;
	}

	public sealed class CommandResult : DiscreteCommandResult {
		public static readonly CommandResult Success = new CommandResult("Success");
		public static readonly CommandResult NotUpgradeable = new CommandResult("NotUpgradeable");
		public static readonly CommandResult NotEnoughMoney = new CommandResult("NotEnoughMoney");

		public override bool IsSuccess => this == Success;

		private CommandResult(string name) : base(name) {}
	}
}

}
