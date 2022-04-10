using Logic.Command;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event.World.Unit;
using NUnit.Framework;

namespace LogicTests {

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
		for (var i = 0; i < 10; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 1)));

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
		for (var i = 0; i < 10; i++) overview.Commands.Issue(new AdvanceTimeCommand(overview, 1));

		//Validate the results
		Assert.AreEqual(0, overview.World.Units.Count);
		foreach (GameTeam team in overview.Teams) Assert.AreEqual(0, team.Castle.Health, 0.01);
		Assert.AreEqual(GamePhase.Finished, overview.CurrentPhase);
	}

	[Test]
	public void TestTowerDestroysUnit() {
		GameOverview overview = GameTestUtils.CreateOverview(((overviewConfig, economyConfig, worldConfig) => {
			worldConfig.MaxBuildingDistance = worldConfig.Width * worldConfig.Height;
		}));

		GameTeam unitTeam = overview.GetTeam(Color.Blue);
		GameTeam towerTeam = overview.GetEnemyTeam(unitTeam);

		//Purchase a unit
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData();
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(unitTeam, unitType)));

		//Build a tower with high damage and range
		GameTestUtils.TowerTypeData towerType = new GameTestUtils.TowerTypeData {
			Damage = unitType.Health * 1.1f, Range = overview.World.Width * overview.World.Height
		};
		TilePosition towerPosition = WorldTestUtils.FindAnyEmptyPosition(overview.World);
		Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
			overview.Commands.Issue(new BuildTowerCommand(towerTeam, towerType, towerPosition)));

		//Set up event listener
		var unitDestroyedEvent = false;
		overview.Events.AddListener<UnitDestroyedEvent>(e => unitDestroyedEvent = true);

		//Enter fighting phase and let the tower kill the unit
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		for (var i = 0; i < 10; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.1f)));

		//Validate the results
		Assert.AreEqual(0, overview.World.Units.Count);
		foreach (GameTeam team in overview.Teams)
			Assert.AreEqual(overview.World.Config.CastleStartingHealth, team.Castle.Health, 0.01);
		Assert.IsTrue(unitDestroyedEvent);
	}
}

}
