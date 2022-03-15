﻿using System.Linq;
using Logic.Command;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

public class AdvanceTimeHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AdvanceTimeCommand, BiCommandResult>(Handle);
	}

	private BiCommandResult Handle(AdvanceTimeCommand command) {
		IGameOverview game = command.Game;
		GameWorld world = command.Game.World;
		float deltaTime = command.DeltaTime;

		game.DecreaseTimeLeftFromPhase(deltaTime);
		if (game.CurrentPhase != GamePhase.Fight) return BiCommandResult.Success;

		foreach (Tower tower in world.GetTileObjectsOfType<Tower>()) {
			tower.UpdateTarget();
			if (tower.RemainingCooldownTime > 0) {
				tower.UpdateCooldown(deltaTime);
			} else if (tower.Target != null) {
				tower.Shoot();
			}
		}

		foreach (Unit unit in world.Units) {
			unit.Move(deltaTime);
		}

		foreach (Barrack barrack in world.GetTileObjectsOfType<Barrack>()) {
			if (barrack.RemainingCooldownTime > 0) {
				barrack.UpdateCooldown(deltaTime);
			} else if (barrack.QueuedUnits.Any()) {
				barrack.Spawn();
			}
		}

		return BiCommandResult.Success;
	}
}

}
