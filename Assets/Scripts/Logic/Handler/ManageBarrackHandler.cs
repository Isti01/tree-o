﻿using System.Linq;
using Logic.Command;
using Logic.Command.Barrack;
using Logic.Data.World;

namespace Logic.Handler {

public class ManageBarrackHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AddBarrackCheckpointCommand, AddBarrackCheckpointCommand.CommandResult>(Handle);
		dispatcher.RegisterConsumer<RemoveBarrackCheckpointCommand, BiCommandResult>(Handle);
	}

	private AddBarrackCheckpointCommand.CommandResult Handle(AddBarrackCheckpointCommand command) {
		Barrack barrack = command.Barrack;
		TilePosition position = command.Position;

		if (!barrack.CheckPoints.Contains(position)) return AddBarrackCheckpointCommand.CommandResult.AlreadyCheckpoint;

		if (barrack.World[position] != null) return AddBarrackCheckpointCommand.CommandResult.InvalidPosition;

		if (!barrack.World.Navigation.IsPositionReachable(barrack.Position, position))
			return AddBarrackCheckpointCommand.CommandResult.UnreachablePosition;

		barrack.PushCheckPoint(position);
		return AddBarrackCheckpointCommand.CommandResult.Success;
	}

	private BiCommandResult Handle(RemoveBarrackCheckpointCommand command) {
		Barrack barrack = command.Barrack;
		TilePosition position = command.Position;

		if (!barrack.CheckPoints.Contains(position)) return BiCommandResult.Failure;

		barrack.PushCheckPoint(position);
		return BiCommandResult.Success;
	}
}

}
