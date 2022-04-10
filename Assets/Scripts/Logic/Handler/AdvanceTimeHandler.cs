using System.Linq;
using Logic.Command;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

internal class AdvanceTimeHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AdvanceTimeCommand, BiCommandResult>(Handle);
	}

	private BiCommandResult Handle(AdvanceTimeCommand command) {
		GameOverview overview = (GameOverview) command.Overview;
		GameWorld world = command.Overview.World;
		float deltaTime = command.DeltaTime;

		overview.DecreaseTimeLeftFromPhase(deltaTime);
		if (overview.CurrentPhase != GamePhase.Fight) return BiCommandResult.Success;

		bool anyUnitAlive = world.Units.Any();
		bool anyUnitQueued = world.GetTileObjectsOfType<Barrack>().Any(b => b.QueuedUnits.Any());
		if (!anyUnitAlive && !anyUnitQueued) {
			overview.AdvancePhase();
			return BiCommandResult.Success;
		}

		foreach (Tower tower in world.GetTileObjectsOfType<Tower>()) {
			tower.UpdateTarget();
			if (tower.IsOnCooldown) {
				tower.UpdateCooldown(deltaTime);
			} else if (tower.Target != null) {
				tower.Shoot();
			}
		}

		foreach (Unit unit in world.Units) {
			unit.Move(deltaTime);
		}

		foreach (Barrack barrack in world.GetTileObjectsOfType<Barrack>()) {
			if (barrack.IsOnCooldown) {
				barrack.UpdateCooldown(deltaTime);
			} else if (barrack.QueuedUnits.Any()) {
				barrack.Spawn();
			}
		}

		return BiCommandResult.Success;
	}
}

}
