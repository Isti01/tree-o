using System.Linq;
using Logic.Command;
using Logic.Command.Barrack;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

/// <summary>
/// Handler of <see cref="Barrack"/> related commands:
/// <see cref="AddBarrackCheckpointCommand"/> and <see cref="RemoveBarrackCheckpointCommand"/>.
/// </summary>
internal class ManageBarrackHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AddBarrackCheckpointCommand, AddBarrackCheckpointCommand.CommandResult>(Handle);
		dispatcher.RegisterConsumer<RemoveBarrackCheckpointCommand, BiCommandResult>(Handle);
	}

	private AddBarrackCheckpointCommand.CommandResult Handle(AddBarrackCheckpointCommand command) {
		Barrack barrack = command.Barrack;
		GameWorld world = barrack.World;
		IGameOverview overview = world.Overview;
		TilePosition position = command.Position;

		if (overview.CurrentPhase != GamePhase.Prepare) return AddBarrackCheckpointCommand.CommandResult.MiscFailure;
		if (barrack.CheckPoints.Contains(position)) return AddBarrackCheckpointCommand.CommandResult.AlreadyCheckpoint;
		if (world[position] != null) return AddBarrackCheckpointCommand.CommandResult.InvalidPosition;

		if (!world.Navigation.IsPositionReachable(barrack.Position, position))
			return AddBarrackCheckpointCommand.CommandResult.UnreachablePosition;

		//Position can be reached from the barrack, but the enemy castle might not be reachable from that position
		TilePosition enemyCastle = overview.GetEnemyTeam(barrack.Owner).Castle.Position;
		if (!world.Navigation.IsPositionReachable(enemyCastle, position))
			return AddBarrackCheckpointCommand.CommandResult.UnreachablePosition;

		barrack.PushCheckPoint(position);
		return AddBarrackCheckpointCommand.CommandResult.Success;
	}

	private BiCommandResult Handle(RemoveBarrackCheckpointCommand command) {
		Barrack barrack = command.Barrack;
		TilePosition position = command.Position;

		if (barrack.World.Overview.CurrentPhase != GamePhase.Prepare) return BiCommandResult.Failure;
		if (!barrack.CheckPoints.Contains(position)) return BiCommandResult.Failure;

		barrack.DeleteCheckPoint(position);
		return BiCommandResult.Success;
	}
}

}
