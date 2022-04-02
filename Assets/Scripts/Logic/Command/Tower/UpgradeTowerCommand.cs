namespace Logic.Command.Tower {

public class UpgradeTowerCommand : BaseCommand<UpgradeTowerCommand.CommandResult> {
	public Data.World.Tower Tower { get; }

	public UpgradeTowerCommand(Data.World.Tower tower) {
		Tower = tower;
	}

	public sealed class CommandResult : ICommandResult {
		public static readonly CommandResult Success = new CommandResult();
		public static readonly CommandResult NotUpgradeable = new CommandResult();
		public static readonly CommandResult NotEnoughMoney = new CommandResult();

		public bool IsSuccess => this == Success;

		private CommandResult() {
			//Disallow construction
		}
	}
}

}
