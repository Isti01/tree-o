using System.Linq;
using Logic.Command;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {
/// <summary>
/// Tests the interaction between the <see cref="Barrack"/> and the <see cref="Unit"/> classes:
/// unit purchasing causing units to get queued and barracks spawning units.
/// </summary>
public class BarrackUnitIntegrationTest {
	[Test]
	public void TestPurchasedUnitGetsRandomlyQueued() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData();
		Assert.AreNotEqual(0, team.Barracks.Count);

		int purchasedUnits = 0;
		int whileLimit = 100000;
		while (whileLimit-- > 0 && team.Barracks.Any(b => b.QueuedUnits.Count == 0)) {
			Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));
			purchasedUnits++;
		}

		Assert.Greater(whileLimit, 0);
		Assert.AreEqual(purchasedUnits, team.Barracks.Select(b => b.QueuedUnits.Count).Sum());
	}

	[Test]
	public void TestBarracksSlowlySpawnUnits() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData {Speed = 0};

		//Purchase 2 units for the first barrack
		team.Barracks.First().QueueUnit(unitType);
		team.Barracks.First().QueueUnit(unitType);

		//Enter fighting phase
		Assert.IsTrue(overview.Commands.Issue(new AdvancePhaseCommand(overview)));
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		Assert.AreEqual(0, overview.World.Units.Count);
		Assert.AreEqual(2, team.Barracks.First().QueuedUnits.Count);

		//Spawn the first unit
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Assert.AreEqual(2, overview.World.Units.Count + team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.IsTrue(team.Barracks.First().QueuedUnits.Count > 0);
		Assert.IsTrue(team.Barracks.First().IsOnCooldown);

		//Assert that cooldown lasts
		float deltaTime = overview.World.Config.BarrackSpawnCooldownTime / 10;
		for (int i = 0; i < 9; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.IsTrue(team.Barracks.First().QueuedUnits.Count > 0);
		Assert.IsTrue(team.Barracks.First().IsOnCooldown);

		//Assert that the barrack spawn another unit
		int oldQueued = team.Barracks.First().QueuedUnits.Count;
		for (int i = 0; i < 2; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.AreEqual(2, overview.World.Units.Count + team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.AreEqual(oldQueued - 1, team.Barracks.First().QueuedUnits.Count);
		Assert.IsTrue(team.Barracks.First().IsOnCooldown);

		//Assert that the barrack won't be on cooldown after spawning all units
		deltaTime = overview.World.Config.BarrackSpawnCooldownTime * 1.1f;
		for (int i = 0; i < 3; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.AreEqual(2, overview.World.Units.Count);
		Assert.AreEqual(0, team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.IsFalse(team.Barracks.First().IsOnCooldown);
	}

	[Test]
	public void TestUnitPurchasingEconomy() {
		GameOverview overview = GameTestUtils.CreateOverview();
		IGameEconomyConfig config = overview.EconomyConfig;
		GameTeam team = overview.Teams.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData();

		int expectedMoney = config.StartingBalance;
		Assert.AreEqual(expectedMoney, team.Money);

		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));
		expectedMoney -= unitType.Cost;
		Assert.AreEqual(expectedMoney, team.Money);

		Assert.IsTrue(overview.Commands.Issue(new AdvancePhaseCommand(overview)));
		Assert.IsTrue(overview.Commands.Issue(new AdvancePhaseCommand(overview)));
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);

		expectedMoney += config.RoundBasePay + config.TotalUnitsPurchasedPay;
		Assert.AreEqual(expectedMoney, team.Money);
	}
}
}
