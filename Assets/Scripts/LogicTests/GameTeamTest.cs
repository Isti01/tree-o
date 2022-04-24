using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="GameTeam"/> class.
/// </summary>
public class GameTeamTest {
	[Test]
	public void TestGiveMoney() {
		GameTeam team = GameTestUtils.CreateOverview().Teams.First();
		int oldMoney = team.Money;
		const int delta = 42;
		team.GiveMoney(delta);
		Assert.AreEqual(oldMoney + delta, team.Money);
	}

	[Test]
	public void TestSpendMoney() {
		GameTeam team = GameTestUtils.CreateOverview().Teams.First();
		int oldMoney = team.Money;
		int delta = oldMoney / 2 + 1;
		team.SpendMoney(delta);
		Assert.AreEqual(oldMoney - delta, team.Money);
		Assert.AreEqual(delta, team.MoneySpent);
	}

	[Test]
	public void TestUnitTypeCounter() {
		GameTeam team = GameTestUtils.CreateOverview().Teams.First();

		IUnitTypeData typeOne = new GameTestUtils.UnitTypeData();
		Assert.AreEqual(0, team.GetDeployedUnitTypeCount(typeOne));
		team.IncrementPurchasedUnitCount(typeOne);
		Assert.AreEqual(1, team.GetDeployedUnitTypeCount(typeOne));

		IUnitTypeData typeTwo = new GameTestUtils.UnitTypeData();
		Assert.AreEqual(0, team.GetDeployedUnitTypeCount(typeTwo));
	}

	[Test]
	public void TestTowerPositionsExcludeUnits() {
		GameOverview overview = GameTestUtils.CreateOverview(world => {
			world.MaxBuildingDistance = world.Width * world.Height;
		});
		GameTeam team = overview.Teams.First();

		IUnitTypeData unitType = new GameTestUtils.UnitTypeData { Speed = 1 };
		Assert.IsTrue(overview.Commands.Issue(new PurchaseUnitCommand(team, unitType)));
		overview.AdvancePhase();
		for (int i = 0; i < 3; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.5f)));

		Unit unit = team.Units.First();
		TilePosition originalPosition = unit.TilePosition;

		team.InvalidateCachedAvailableTowerPositions();
		Assert.IsFalse(team.AvailableTowerPositions.Contains(originalPosition));

		for (int i = 0; i < 3; i++) Assert.IsTrue(overview.Commands.Issue(new AdvanceTimeCommand(overview, 0.5f)));
		team.InvalidateCachedAvailableTowerPositions();
		Assert.IsTrue(team.AvailableTowerPositions.Contains(originalPosition));
	}

	[Test]
	public void TestTowerPositionsDisallowEnclosingCastle() {
		GameOverview overview = GameTestUtils.CreateOverview();
		GameWorld world = overview.World;
		GameTeam team = overview.Teams.First();

		int[] dx = { 1, -1, 0, 0 };
		int[] dy = { 0, 0, 1, -1 };
		IList<int> freeSides = new List<int>();

		for (int i = 0; i < dx.Length; i++) {
			TilePosition neighbor = team.Castle.Position.Added(dx[i], dy[i]);
			if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X == world.Width || neighbor.Y == world.Height) continue;
			if (world[neighbor] != null) continue;
			freeSides.Add(i);
			Assert.IsTrue(team.AvailableTowerPositions.Contains(neighbor));
		}

		for (int i = 1; i < freeSides.Count; i++) {
			int index = freeSides[i];
			overview.Commands.Issue(new BuildTowerCommand(team,
				new GameTestUtils.TowerTypeData(), team.Castle.Position.Added(dx[index], dy[index])));
		}

		team.InvalidateCachedAvailableTowerPositions();
		int freeIndex = freeSides[0];
		TilePosition position = team.Castle.Position.Added(dx[freeIndex], dy[freeIndex]);
		Assert.IsFalse(team.AvailableTowerPositions.Contains(position));
	}
}

}
