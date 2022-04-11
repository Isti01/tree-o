using Logic.Data.World;

namespace Logic.Command.Barrack {

public class AddBarrackCheckpointCommand : BaseCommand<AddBarrackCheckpointCommand.CommandResult> {
	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }

	public AddBarrackCheckpointCommand(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}

	public sealed class CommandResult : AbstractCommandResult {
		public static readonly CommandResult Success = new CommandResult("Success");
		public static readonly CommandResult AlreadyCheckpoint = new CommandResult("AlreadyCheckpoint");
		public static readonly CommandResult InvalidPosition = new CommandResult("InvalidPosition");
		public static readonly CommandResult UnreachablePosition = new CommandResult("UnreachablePosition");

		public override bool IsSuccess => this == Success;

		private CommandResult(string name) : base(name) {}
	}
}

}
