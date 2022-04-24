using System.Linq;
using Logic.Data;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="GameWorld"/> class.
/// </summary>
public class GameWorldTest {
	[Test]
	public void TestBuildTower() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameWorld world = overview.World;

		GameTeam team = overview.Teams.First();
		TilePosition position = WorldTestUtils.GetEmptyPosition(world);
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();

		world.BuildTower(team, towerType, position);
		Assert.AreEqual(1, world.GetTileObjectsOfType<Tower>().Count());
		Assert.AreEqual(team, ((Tower) world[position]).Owner);
		Assert.AreEqual(towerType, ((Tower) world[position]).Type);
	}

	[Test]
	public void TestDestroyTower() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameWorld world = overview.World;

		GameTeam team = overview.Teams.First();
		TilePosition position = WorldTestUtils.GetEmptyPosition(world);
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();

		world.BuildTower(team, towerType, position);
		Tower tower = (Tower) world[position];
		world.DestroyTower(tower);
		Assert.AreEqual(0, world.GetTileObjectsOfType<Tower>().Count());
		Assert.IsNull(world[position]);
	}

	[Test]
	public void TestDeployUnit() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameWorld world = overview.World;

		Barrack barrack = overview.Teams.First().Barracks.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData();

		world.DeployUnit(barrack, unitType);
		Assert.AreEqual(1, world.Units.Count);
		Assert.AreEqual(unitType, world.Units.First().Type);
	}

	[Test]
	public void TestDestroyUnit() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameWorld world = overview.World;

		Barrack barrack = overview.Teams.First().Barracks.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData();

		world.DeployUnit(barrack, unitType);
		Assert.AreEqual(1, world.Units.Count);
		world.DestroyUnit(world.Units.First());
		Assert.AreEqual(0, world.Units.Count);
	}
}

}
