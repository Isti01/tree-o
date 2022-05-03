using System;
using System.Linq;
using Logic.Command;
using Logic.Data;
using NUnit.Framework;

namespace LogicTests {
/// <summary>
/// Container of unit tests for the <see cref="GameOverview"/> class.
/// This class encapsulates almost all other classes in the logic component,
/// so it's important to note that only the functionalities provided by this specific class
/// are tested in this test class.
/// </summary>
public class GameOverviewTest {
	[Test]
	public void TestNoOpRound() {
		GameOverview overview = GameTestUtils.CreateOverview();
		IGameEconomyConfig economyConfig = overview.EconomyConfig;

		foreach (GameTeam team in overview.Teams) {
			Assert.AreEqual(economyConfig.StartingBalance, team.Money);
		}

		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);

		foreach (GameTeam team in overview.Teams) {
			Assert.AreEqual(economyConfig.StartingBalance + economyConfig.RoundBasePay, team.Money);
		}
	}

	[Test]
	public void TestFightingPhaseEndsWhenNoUnitsLeft() {
		GameOverview overview = GameTestUtils.CreateOverview();
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, float.Epsilon)));
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
	}

	[Test]
	public void TestFightingPhaseHasLimitedTime() {
		GameOverview overview = GameTestUtils.CreateOverview();

		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
		GameTestUtils.UnitTypeData unitType = new GameTestUtils.UnitTypeData {Speed = float.Epsilon};
		overview.Teams.First().Barracks.First().QueueUnit(unitType);

		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);

		float deltaTime = overview.TimeLeftFromPhase * 0.3f;
		for (int i = 0; i < 3; i++) {
			Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
			Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
			Assert.AreEqual(1, overview.World.Units.Count);
		}

		Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime)));
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
	}

	[Test]
	public void TestFinishedPhaseHasNoNextPhase() {
		GameOverview overview = GameTestUtils.CreateOverview(worldConfig => {
			worldConfig.CastleStartingHealth = 0; //Dirty hack, hopefully won't break
		});

		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Finished, overview.CurrentPhase);
		Assert.Throws<Exception>(overview.AdvancePhase);
		Assert.AreEqual(GamePhase.Finished, overview.CurrentPhase);
	}
}
}
