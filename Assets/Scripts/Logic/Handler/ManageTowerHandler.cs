﻿using System.Collections.Generic;
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
		dispatcher.RegisterConsumer<UpgradeTowerCommand, UpgradeTowerCommand.CommandResult>(Handle);
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

		foreach (GameTeam team in overview.Teams) {
			TilePosition to = overview.GetEnemyTeam(team).Castle.Position;
			foreach (TilePosition from in team.Barracks.Select(b => b.Position)
				.Concat(team.Units.Select(u => u.TilePosition))) {
				if (!world.Navigation.IsPositionReachable(from, to, blockedTiles)) {
					return BuildTowerCommand.CommandResult.LeavesNoPathForUnit;
				}
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

	private UpgradeTowerCommand.CommandResult Handle(UpgradeTowerCommand command) {
		Tower tower = command.Tower;
		if (tower.Type.AfterUpgradeType == null) return UpgradeTowerCommand.CommandResult.NotUpgradeable;
		if (tower.Type.UpgradeCost > tower.Owner.Money) return UpgradeTowerCommand.CommandResult.NotEnoughMoney;

		tower.Owner.SpendMoney(tower.Type.UpgradeCost);
		tower.Upgrade();
		return UpgradeTowerCommand.CommandResult.Success;
	}
}

}
