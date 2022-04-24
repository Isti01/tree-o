using System.Linq;
using Logic.Command.Tower;
using Logic.Data;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the building, destruction and upgrading of <see cref="Tower"/> instances.
/// </summary>
public class TowerManagementTest {
	[Test]
	public void TestBuildSuccess() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		TilePosition position = team.AvailableTowerPositions.First();

		int oldMoney = team.Money;
		Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new BuildTowerCommand(team, towerType, position)));
		Assert.IsTrue(overview.World[position] is Tower);
		Assert.AreEqual(oldMoney - towerType.BuildingCost, team.Money);
	}

	[Test]
	public void TestBuildNotEnoughMoney() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData { BuildingCost = team.Money + 1 };
		TilePosition position = team.AvailableTowerPositions.First();

		int oldMoney = team.Money;
		Assert.AreEqual(BuildTowerCommand.CommandResult.NotEnoughMoney,
			overview.Commands.Issue(new BuildTowerCommand(team, towerType, position)));
		Assert.IsNull(overview.World[position]);
		Assert.AreEqual(oldMoney, team.Money);
	}

	[Test]
	public void TestBuildTileOccupied() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		TilePosition position = team.Castle.Position;

		int oldMoney = team.Money;
		Assert.AreEqual(BuildTowerCommand.CommandResult.TileUnavailable,
			overview.Commands.Issue(new BuildTowerCommand(team, towerType, position)));
		Assert.AreEqual(team.Castle, overview.World[position]);
		Assert.AreEqual(oldMoney, team.Money);
	}

	[Test]
	public void TestBuildTileTooFarAwayFromExistingBuildings() {
		GameOverview overview = GameTestUtils.CreateOverview(worldConfig => {
			worldConfig.MaxBuildingDistance = 0; //Essentially disallow building of towers
		});

		GameTeam team = overview.Teams.First();
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		TilePosition position = WorldTestUtils.GetEmptyPosition(overview.World);

		int oldMoney = team.Money;
		Assert.AreEqual(BuildTowerCommand.CommandResult.TileUnavailable,
			overview.Commands.Issue(new BuildTowerCommand(team, towerType, position)));
		Assert.IsNull(overview.World[position]);
		Assert.AreEqual(oldMoney, team.Money);
	}

	[Test]
	public void TestUpgradeSuccess() {
		GameTestUtils.TowerTypeData levelTwo = new GameTestUtils.TowerTypeData();
		GameTestUtils.TowerTypeData levelOne = new GameTestUtils.TowerTypeData { AfterUpgradeType = levelTwo };
		GameOverview overview = CreateGameWithTower(levelOne);
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First();
		GameTeam team = tower.Owner;

		int oldMoney = team.Money;
		Assert.AreEqual(UpgradeTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new UpgradeTowerCommand(tower)));
		Assert.AreEqual(tower, overview.World[tower.Position]);
		Assert.AreEqual(levelTwo, tower.Type);
		Assert.AreEqual(oldMoney - levelOne.UpgradeCost, team.Money);
	}

	[Test]
	public void TestUpgradeNotUpgradeable() {
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		GameOverview overview = CreateGameWithTower(towerType);
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First();
		GameTeam team = tower.Owner;

		int oldMoney = team.Money;
		Assert.AreEqual(UpgradeTowerCommand.CommandResult.NotUpgradeable,
			overview.Commands.Issue(new UpgradeTowerCommand(tower)));
		Assert.AreEqual(tower, overview.World[tower.Position]);
		Assert.AreEqual(towerType, tower.Type);
		Assert.AreEqual(oldMoney, team.Money);
	}

	[Test]
	public void TestUpgradeNotEnoughMoney() {
		GameTestUtils.TowerTypeData levelTwo = new GameTestUtils.TowerTypeData();
		GameTestUtils.TowerTypeData levelOne = new GameTestUtils.TowerTypeData {
			AfterUpgradeType = levelTwo, UpgradeCost = 9999
		};
		GameOverview overview = CreateGameWithTower(levelOne);
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First();
		GameTeam team = tower.Owner;

		int oldMoney = team.Money;
		Assert.AreEqual(UpgradeTowerCommand.CommandResult.NotEnoughMoney,
			overview.Commands.Issue(new UpgradeTowerCommand(tower)));
		Assert.AreEqual(tower, overview.World[tower.Position]);
		Assert.AreEqual(levelOne, tower.Type);
		Assert.AreEqual(oldMoney, team.Money);
	}

	[Test]
	public void TestDestroy() {
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		GameOverview overview = CreateGameWithTower(towerType);
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First();
		GameTeam team = tower.Owner;

		int oldMoney = team.Money;
		Assert.IsTrue(overview.Commands.Issue(new DestroyTowerCommand(tower)));
		Assert.IsNull(overview.World[tower.Position]);
		Assert.AreEqual(oldMoney + towerType.DestroyRefund, team.Money);
	}

	private GameOverview CreateGameWithTower(ITowerTypeData towerType) {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		TilePosition position = team.AvailableTowerPositions.First();
		Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new BuildTowerCommand(team, towerType, position)));
		return overview;
	}
}

}
