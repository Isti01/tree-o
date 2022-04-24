using System;
using System.Linq;
using Logic.Command.Tower;
using Logic.Data;
using Logic.Data.World;
using Logic.Event.World.Tower;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="Tower"/> class.
/// Most features are tested in the <see cref="UnitTowerCastleIntegrationTest"/> class.
/// </summary>
public class TowerTest {
	[Test]
	public void TestTryUpgradeNotUpgradeable() {
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData();
		Tower tower = CreateTower(towerType);

		bool eventCalled = false;
		tower.World.Overview.Events.AddListener<TowerUpgradedEvent>(_ => eventCalled = true);

		Assert.Throws<InvalidOperationException>(() => tower.Upgrade());
		Assert.IsFalse(eventCalled);
		Assert.AreEqual(towerType, tower.Type);
	}

	[Test]
	public void TestUpgradeSuccess() {
		ITowerTypeData newType = new GameTestUtils.TowerTypeData();
		ITowerTypeData originalType = new GameTestUtils.TowerTypeData { AfterUpgradeType = newType };
		Tower tower = CreateTower(originalType);

		bool eventCalled = false;
		tower.World.Overview.Events.AddListener<TowerUpgradedEvent>(_ => eventCalled = true);

		tower.Upgrade();
		Assert.IsTrue(eventCalled);
		Assert.AreEqual(newType, tower.Type);
	}

	[Test]
	public void TestClosestEnemy() {
		Tower tower = CreateTower(new GameTestUtils.TowerTypeData());
		Assert.IsNull(tower.ClosestEnemy);

		tower.World.DeployUnit(tower.Owner.Barracks.First(), new GameTestUtils.UnitTypeData());
		Assert.IsNull(tower.ClosestEnemy);

		Barrack enemyBarrack = tower.World.Overview.GetEnemyTeam(tower.Owner).Barracks.First();
		tower.World.DeployUnit(enemyBarrack, new GameTestUtils.UnitTypeData());
		Assert.AreEqual(tower.World.Units.First(u => u.Owner != tower.Owner), tower.ClosestEnemy);
	}

	private Tower CreateTower(ITowerTypeData type) {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		TilePosition position = team.AvailableTowerPositions.First();
		Assert.IsTrue(overview.Commands.Issue(new BuildTowerCommand(team, type, position)));
		return team.Towers.First();
	}
}

}
