using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Command.Tower;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

public class ManageTowerHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<BuildTowerCommand, BuildTowerCommand.CommandResult>(Handle);
		dispatcher.RegisterConsumer<DestroyTowerCommand, BiCommandResult>(Handle);
	}

	private BuildTowerCommand.CommandResult Handle(BuildTowerCommand command) {
		if (command.Team.Overview.CurrentPhase != GamePhase.Prepare) return BuildTowerCommand.CommandResult.MiscFailure;

		IGameOverview overview = command.Team.Overview;
		GameWorld world = command.Team.Overview.World;

		if (command.Team.Money < command.Type.BuildingCost) return BuildTowerCommand.CommandResult.NotEnoughMoney;

		if (world[command.Position] != null) return BuildTowerCommand.CommandResult.TileAlreadyOccupied;

		if (world.Units.Any(unit => unit.TilePosition.Equals(command.Position)))
			return BuildTowerCommand.CommandResult.TileAlreadyOccupied;

		ICollection<TilePosition> blockedTiles = new List<TilePosition>();
		blockedTiles.Add(command.Position);
		foreach (Unit unit in world.Units) {
			TilePosition from = unit.TilePosition;
			TilePosition to = overview.GetEnemyTeam(unit.Owner).Castle.Position;
			if (!world.Navigation.IsPositionReachable(from, to, blockedTiles)) {
				return BuildTowerCommand.CommandResult.LeavesNoPathForUnit;
			}
		}

		command.Team.SpendMoney(command.Type.BuildingCost);
		command.Team.IncrementBuiltTowerCount();
		world.BuildTower(command.Team, command.Type, command.Position);
		return BuildTowerCommand.CommandResult.Success;
	}

	private BiCommandResult Handle(DestroyTowerCommand command) {
		if (command.Tower.World.Overview.CurrentPhase != GamePhase.Prepare) return BiCommandResult.Failure;

		Tower tower = command.Tower;
		tower.Owner.GiveMoney(tower.Type.DestroyRefund);
		tower.World.DestroyTower(tower);
		return BiCommandResult.Success;
	}
}

}
