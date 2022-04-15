using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Barrack {

/// <summary>
/// Command for adding an entry to <see cref="Data.World.Barrack.CheckPoints"/>
/// during the <see cref="GamePhase.Prepare"/> phase.
/// </summary>
public class AddBarrackCheckpointCommand : BaseCommand<AddBarrackCheckpointCommand.CommandResult> {
	public Data.World.Barrack Barrack { get; }
	public TilePosition Position { get; }

	public AddBarrackCheckpointCommand(Data.World.Barrack barrack, TilePosition position) {
		Barrack = barrack;
		Position = position;
	}

	public sealed class CommandResult : DiscreteCommandResult {
		public static readonly CommandResult Success = new CommandResult("Success");
		public static readonly CommandResult AlreadyCheckpoint = new CommandResult("AlreadyCheckpoint");
		public static readonly CommandResult InvalidPosition = new CommandResult("InvalidPosition");
		public static readonly CommandResult UnreachablePosition = new CommandResult("UnreachablePosition");
		public static readonly CommandResult MiscFailure = new CommandResult("MiscFailure");

		public override bool IsSuccess => this == Success;

		private CommandResult(string name) : base(name) {}
	}
}

}
