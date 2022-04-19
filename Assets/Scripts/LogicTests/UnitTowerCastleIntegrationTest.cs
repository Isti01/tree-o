using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the basic interactions between <see cref="Unit"/>, <see cref="Castle"/>
/// and <see cref="Tower"/> instances: unit damaging castle, unit destroying castle,
/// tower targeting unit, tower damaging unit,tower destroying unit.
/// </summary>
public class UnitTowerCastleIntegrationTest {
	[Test]
	public void TestUnitDamagesCastle() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam attacker = overview.GetTeam(Color.Red);

		//Purchase a unit that doesn't kill the castle and moves very, very, very fast
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData {
			Damage = overview.World.Config.CastleStartingHealth * 0.5f,
			Speed = overview.World.Width * overview.World.Height
		};
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(attacker, unitType)));

		//Enter fighting phase and move the unit
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		for (int i = 0; i < 10; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 1)));

		//Validate the results
		Assert.AreEqual(0, overview.World.Units.Count);
		Assert.AreEqual(overview.World.Config.CastleStartingHealth - unitType.Damage,
			overview.GetEnemyTeam(attacker).Castle.Health, 0.01);
		Assert.AreEqual(overview.World.Config.CastleStartingHealth, attacker.Castle.Health, 0.01);
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
	}

	[Test]
	public void TestBothCastlesDestroyed() {
		GameOverview overview = GameTestUtils.CreateOverview();

		//Purchase unit that kill the castles and move very, very, very fast
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData {
			Damage = overview.World.Config.CastleStartingHealth * 1.1f,
			Speed = overview.World.Width * overview.World.Height
		};
		foreach (GameTeam team in overview.Teams)
			Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));

		//Enter fighting phase and move the units
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		for (int i = 0; i < 10; i++) overview.Commands.Issue(new AdvanceTimeCommand(overview, 1));

		//Validate the results
		Assert.AreEqual(0, overview.World.Units.Count);
		foreach (GameTeam team in overview.Teams) Assert.AreEqual(0, team.Castle.Health, 0.01);
		Assert.AreEqual(GamePhase.Finished, overview.CurrentPhase);
	}

	[Test]
	public void TestTowerDestroysUnit() {
		GameOverview overview = GameTestUtils.CreateOverview(worldConfig => {
			worldConfig.MaxBuildingDistance = worldConfig.Width * worldConfig.Height;
		});

		GameTeam unitTeam = overview.GetTeam(Color.Blue);
		GameTeam towerTeam = overview.GetEnemyTeam(unitTeam);

		//Purchase a unit
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData();
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(unitTeam, unitType)));

		//Build a tower with high damage and range
		GameTestUtils.TowerTypeData towerType = new GameTestUtils.TowerTypeData {
			Damage = unitType.Health * 1.1f, Range = overview.World.Width * overview.World.Height
		};
		TilePosition towerPosition = towerTeam.AvailableTowerPositions.First();
		Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new BuildTowerCommand(towerTeam, towerType, towerPosition)));

		//Set up event listener
		bool unitDestroyedEvent = false;
		overview.Events.AddListener<UnitDestroyedEvent>(_ => unitDestroyedEvent = true);

		//Enter fighting phase and let the tower kill the unit
		int oldMoney = towerTeam.Money;
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		for (int i = 0; i < 10; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.1f)));

		//Validate the results
		Assert.AreEqual(0, overview.World.Units.Count);
		foreach (GameTeam team in overview.Teams)
			Assert.AreEqual(overview.World.Config.CastleStartingHealth, team.Castle.Health, 0.01);
		Assert.IsTrue(unitDestroyedEvent);
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
		Assert.AreEqual(oldMoney + overview.EconomyConfig.NewUnitsDestroyedPay
			+ overview.EconomyConfig.RoundBasePay, towerTeam.Money);
	}

	[Test]
	public void TestTowerCooldown() {
		GameOverview overview = GameTestUtils.CreateOverview(overviewConfig => {
			overviewConfig.FightingPhaseDuration = float.PositiveInfinity;
		});

		GameTeam unitTeam = overview.GetTeam(Color.Blue);
		GameTeam towerTeam = overview.GetEnemyTeam(unitTeam);

		//Purchase a unit with zero speed
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData { Speed = 0 };
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(unitTeam, unitType)));

		//Build a tower with zero damage and high range
		GameTestUtils.TowerTypeData towerType = new GameTestUtils.TowerTypeData {
			Damage = 0, Range = overview.World.Width * overview.World.Height
		};
		TilePosition towerPosition = towerTeam.AvailableTowerPositions.First();
		Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new BuildTowerCommand(towerTeam, towerType, towerPosition)));
		Tower tower = towerTeam.Towers.First();

		//Enter fighting phase and let the tower shoot at the unit
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		bool shot = false;
		overview.Events.AddListener<TowerShotEvent>(_ => shot = true);
		while (!shot) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));

		//Assert that the cooldown lasts
		shot = false;
		Assert.IsTrue(tower.IsOnCooldown);
		float deltaTime = towerType.CooldownTime / 10;
		for (int i = 0; i < 9; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.IsFalse(shot);
		Assert.IsTrue(tower.IsOnCooldown);

		//Assert that the tower shoots again
		for (int i = 0; i < 2; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.IsTrue(shot);
		Assert.IsTrue(tower.IsOnCooldown);
	}

	[Test]
	public void TestTowerTargetReachedCastle() {
		GameOverview overview = CreateTowerTargetTestingGame(10, 10,
			(towerTeam, _) => new[] { towerTeam.AvailableTowerPositions.First() }, 1, 0, float.PositiveInfinity, 1);

		//Update the tower's target
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First();
		Assert.AreEqual(tower.Target, overview.World.Units.First());

		//Let the unit reach the castle
		while (overview.World.Units.Any())
			Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.5f)));

		//Validate results
		Assert.IsNull(tower.Target);
	}

	[Test]
	public void TestTowerTargetGotDestroyed() {
		GameOverview overview = CreateTowerTargetTestingGame(10, 10,
			(towerTeam, _) => towerTeam.AvailableTowerPositions.Take(2), 5, 2, float.PositiveInfinity, 0);

		//Update the towers' targets (and let them shoot once)
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		foreach (Tower tower in overview.World.GetTileObjectsOfType<Tower>())
			Assert.AreEqual(tower.Target, overview.World.Units.First());

		//Let the towers kill the unit
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));

		//Validate results
		foreach (Tower tower in overview.World.GetTileObjectsOfType<Tower>()) Assert.IsNull(tower.Target);
	}

	[Test]
	public void TestTowerTargetMovedOutOfRange() {
		IEnumerable<TilePosition> TowerPositionChooser(GameTeam towerTeam, GameWorld world) {
			int[] dx = { -1, 0, 0, 1 };
			int[] dy = { 0, 1, -1, 0 };
			foreach (Barrack barrack in world.Overview.GetEnemyTeam(towerTeam).Barracks) {
				bool any = false;
				for (int i = 0; i < 4 && !any; i++) {
					TilePosition position = barrack.Position.Added(dx[i], dy[i]);
					if (!towerTeam.AvailableTowerPositions.Contains(position)) continue;
					any = true;
					yield return position;
				}

				if (!any) Assert.Fail("Unable to place towers next to the barracks");
			}
		}

		GameOverview overview = CreateTowerTargetTestingGame(30, 30, TowerPositionChooser, 1, 0, 2.5f, 1);

		//Update the tower's target
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Tower tower = overview.World.GetTileObjectsOfType<Tower>().First(t => t.Target != null);
		Assert.NotNull(tower);
		Unit unit = overview.World.Units.First();

		//Let the unit move outside the tower's range
		while (unit.IsAlive && unit.Position.Distance(tower.Position.ToVectorCentered()) <= tower.Type.Range)
			Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.5f)));
		Assert.IsTrue(unit.IsAlive);

		//Let the tower update its target, validate results
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Assert.IsNull(tower.Target);
	}

	private static GameOverview CreateTowerTargetTestingGame(int worldWidth, int worldHeight,
		Func<GameTeam, GameWorld, IEnumerable<TilePosition>> towerPositionChooser,
		float unitHealth, float towerDamage, float towerRange, float towerCooldown) {
		GameOverview overview = GameTestUtils.CreateOverview((overviewConfig, _, worldConfig) => {
			overviewConfig.FightingPhaseDuration = float.PositiveInfinity;
			worldConfig.Width = worldWidth;
			worldConfig.Height = worldHeight;
			worldConfig.MaxBuildingDistance = worldConfig.Height * worldConfig.Width;
		});

		GameTeam unitTeam = overview.Teams.First();
		GameTeam towerTeam = overview.GetEnemyTeam(unitTeam);

		//Purchase a unit
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData { Health = unitHealth };
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(unitTeam, unitType)));

		//Build a tower
		GameTestUtils.TowerTypeData towerType = new GameTestUtils.TowerTypeData {
			Damage = towerDamage, Range = towerRange, CooldownTime = towerCooldown
		};
		foreach (TilePosition towerPosition in towerPositionChooser(towerTeam, overview.World)) {
			Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
				overview.Commands.Issue(new BuildTowerCommand(towerTeam, towerType, towerPosition)));
		}

		//Enter fighting phase, spawn the units
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Assert.AreEqual(1, overview.World.Units.Count);

		return overview;
	}
}

}
