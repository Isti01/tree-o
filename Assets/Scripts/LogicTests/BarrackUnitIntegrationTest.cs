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

		var purchasedUnits = 0;
		while (team.Barracks.Any(b => b.QueuedUnits.Count == 0)) {
			Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));
			purchasedUnits++;
		}

		Assert.AreEqual(purchasedUnits, team.Barracks.Select(b => b.QueuedUnits.Count).Sum());
	}

	[Test]
	public void TestBarracksSlowlySpawnUnits() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameTeam team = overview.Teams.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData { Speed = 0 };

		//Purchase 3 units -> one of the barracks must have at least 2
		for (var i = 0; i < 3; i++) Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));
		Barrack barrack = team.Barracks.OrderByDescending(b => b.QueuedUnits.Count).First();

		//Enter fighting phase
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		Assert.AreEqual(0, overview.World.Units.Count);
		Assert.AreEqual(3, team.Barracks.Sum(b => b.QueuedUnits.Count));

		//Spawn the first unit(s)
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Assert.AreEqual(3, overview.World.Units.Count + team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.IsTrue(barrack.QueuedUnits.Count > 0);
		Assert.IsTrue(barrack.IsOnCooldown);

		//Assert that cooldown lasts
		float deltaTime = overview.World.Config.BarrackSpawnCooldownTime / 10;
		for (var i = 0; i < 9; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.IsTrue(barrack.QueuedUnits.Count > 0);
		Assert.IsTrue(barrack.IsOnCooldown);

		//Assert that the barrack spawn another unit
		int oldQueued = barrack.QueuedUnits.Count;
		for (var i = 0; i < 2; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.AreEqual(3, overview.World.Units.Count + team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.AreEqual(oldQueued - 1, barrack.QueuedUnits.Count);
		Assert.IsTrue(barrack.IsOnCooldown);

		//Assert that the barrack won't be on cooldown after spawning all units
		deltaTime = overview.World.Config.BarrackSpawnCooldownTime * 1.1f;
		for (var i = 0; i < 3; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.AreEqual(3, overview.World.Units.Count);
		Assert.AreEqual(0, team.Barracks.Sum(b => b.QueuedUnits.Count));
		Assert.IsFalse(barrack.IsOnCooldown);
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

		overview.AdvancePhase();
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);

		expectedMoney += config.RoundBasePay + config.TotalUnitsPurchasedPay;
		Assert.AreEqual(expectedMoney, team.Money);
	}
}

}
