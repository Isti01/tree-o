using Logic.Command;
using Logic.Data;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the functionality of the <see cref="GameOverview"/> class.
/// This class encapsulates almost all other classes in the logic component,
/// so it's important to note that only the functionalities provided by this specific class
/// are tested in this test class.
/// </summary>
public class GameOverviewTest {
	[Test]
	public void TestNoOpRound() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
	}

	[Test]
	public void TestFightingPhaseHasLimitedTime() {
		GameOverview overview = GameTestUtils.CreateOverview();
		overview.AdvancePhase();
		Assert.AreEqual(GamePhase.Fight, overview.CurrentPhase);
		float deltaTime = overview.TimeLeftFromPhase * 1.1f;
		overview.Commands.Issue(new AdvanceTimeCommand(overview, deltaTime));
		Assert.AreEqual(GamePhase.Prepare, overview.CurrentPhase);
	}
}

}
