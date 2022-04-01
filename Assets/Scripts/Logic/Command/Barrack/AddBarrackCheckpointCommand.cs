using Logic.Data.World;

namespace Logic.Command.Barrack {

public class AddBarrackCheckpointCommand : BaseCommand<AddBarrackCheckpointCommand.CommandResult> {
	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }

	public AddBarrackCheckpointCommand(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}

	public sealed class CommandResult : ICommandResult {
		public static readonly CommandResult Success = new CommandResult();
		public static readonly CommandResult AlreadyCheckpoint = new CommandResult();
		public static readonly CommandResult InvalidPosition = new CommandResult();
		public static readonly CommandResult UnreachablePosition = new CommandResult();

		public bool IsSuccess => this == Success;

		private CommandResult() {
			//Disallow construction
		}
	}
}

}
