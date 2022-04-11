namespace Logic.Command.Tower {

public class UpgradeTowerCommand : BaseCommand<UpgradeTowerCommand.CommandResult> {
	public Data.World.Tower Tower { get; }

	public UpgradeTowerCommand(Data.World.Tower tower) {
		Tower = tower;
	}

	public sealed class CommandResult : AbstractCommandResult {
		public static readonly CommandResult Success = new CommandResult("Success");
		public static readonly CommandResult NotUpgradeable = new CommandResult("NotUpgradeable");
		public static readonly CommandResult NotEnoughMoney = new CommandResult("NotEnoughMoney");

		public override bool IsSuccess => this == Success;

		private CommandResult(string name) : base(name) {}
	}
}

}
