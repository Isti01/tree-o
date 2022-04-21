using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Command.Barrack;
using Logic.Command.Tower;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Barrack;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the <see cref="Barrack.CheckPoints"/> feature
/// in both the <see cref="Barrack"/> class (testing addition, removal and validation)
/// and the <see cref="Unit"/> class (testing navigation).
/// </summary>
public class BarrackCheckpointTest {
	[Test]
	public void TestAddingValidCheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = FindValidCheckpoint(barrack);

		bool eventCalled = false;
		void Listener(BarrackCheckpointCreatedEvent _) => eventCalled = true;
		overview.Events.AddListener((EventDispatcher.Listener<BarrackCheckpointCreatedEvent>) Listener);

		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));

		Assert.AreEqual(1, barrack.CheckPoints.Count);
		Assert.AreEqual(position, barrack.CheckPoints.First());
		Assert.IsTrue(eventCalled);
	}

	[Test]
	public void TestAddingCheckpointThatIsAlreadyCheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = FindValidCheckpoint(barrack);
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.AlreadyCheckpoint,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));
		Assert.AreEqual(1, barrack.CheckPoints.Count);
	}

	[Test]
	public void TestAddingUnreachableCheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview(config => {
			config.MaxBuildingDistance = config.Width * config.Height;
			config.GenerateObstacles = false; //disable obstacles: we need a 5x5 free area
		});

		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = FindFree5X5AreaCenterThatIsValidCheckpoint(barrack);
		BuildTowersAroundPosition(barrack.Owner, new GameTestUtils.TowerTypeData(), position);

		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.UnreachablePosition,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));
		Assert.AreEqual(0, barrack.CheckPoints.Count);
	}

	[Test]
	public void TestAddingOccupiedCheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = barrack.Owner.Castle.Position;
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.InvalidPosition,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));
		Assert.AreEqual(0, barrack.CheckPoints.Count);
	}

	[Test]
	public void TestRemovingValidCheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = FindValidCheckpoint(barrack);
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));
		Assert.IsTrue(overview.Commands.Issue(new RemoveBarrackCheckpointCommand(barrack, position)));
		Assert.AreEqual(0, barrack.CheckPoints.Count);
	}

	[Test]
	public void TestRemovingNotACheckpoint() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();
		Assert.IsFalse(overview.Commands.Issue(new RemoveBarrackCheckpointCommand(barrack, barrack.Position)));
	}

	[Test]
	public void TestBarrackCheckpointBecameInvalid() {
		GameOverview overview = GameTestUtils.CreateOverview(config => {
			config.MaxBuildingDistance = config.Width * config.Height;
			config.GenerateObstacles = false; //disable obstacles: we need a 5x5 free area
		});

		Barrack barrack = overview.Teams.First().Barracks.First();
		TilePosition position = FindFree5X5AreaCenterThatIsValidCheckpoint(barrack);

		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));

		bool eventCalled = false;
		void Listener(BarrackCheckpointRemovedEvent _) => eventCalled = true;
		overview.Events.AddListener((EventDispatcher.Listener<BarrackCheckpointRemovedEvent>) Listener);

		BuildTowersAroundPosition(barrack.Owner, new GameTestUtils.TowerTypeData(), position);

		Assert.AreEqual(0, barrack.CheckPoints.Count);
		Assert.IsTrue(eventCalled);
	}

	[Test]
	public void TestUnitFollowsCheckpoints() {
		GameOverview overview = GameTestUtils.CreateOverview();

		//Purchase unit and set checkpoint
		Barrack barrack = overview.Teams.First().Barracks.First();
		barrack.QueueUnit(new GameTestUtils.UnitTypeData { Speed = 1 });
		TilePosition position = FindValidCheckpoint(barrack);
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));

		//Enter fighting phase and spawn the unit
		overview.AdvancePhase();
		barrack.Spawn();
		Unit unit = overview.World.Units.First();
		Assert.AreEqual(position, unit.NextCheckpoint);

		//Move the unit until it reaches the checkpoint
		const float deltaTime = 0.2f;
		float maxSteps = overview.World.Width * overview.World.Height / deltaTime;
		for (int i = 0; i < maxSteps && unit.NextCheckpoint.Equals(position); i++) unit.Move(deltaTime);
		Assert.AreEqual(position, unit.TilePosition);

		//Move the unit until it reaches the next checkpoint
		TilePosition enemyCastle = overview.GetEnemyTeam(unit.Owner).Castle.Position;
		Assert.AreEqual(enemyCastle, unit.NextCheckpoint);
		for (int i = 0; i < maxSteps && unit.IsAlive; i++) unit.Move(deltaTime);
		Assert.AreEqual(enemyCastle, unit.TilePosition);
	}

	[Test]
	public void TestUnitCheckpointBecameInvalid() {
		GameOverview overview = GameTestUtils.CreateOverview(config => {
			config.MaxBuildingDistance = config.Width * config.Height;
			config.GenerateObstacles = false; //disable obstacles: we need a 5x5 free area
		});

		//Purchase unit and set checkpoint
		Barrack barrack = overview.Teams.First().Barracks.First();
		barrack.QueueUnit(new GameTestUtils.UnitTypeData());
		TilePosition position = FindFree5X5AreaCenterThatIsValidCheckpoint(barrack);
		Assert.AreEqual(AddBarrackCheckpointCommand.CommandResult.Success,
			overview.Commands.Issue(new AddBarrackCheckpointCommand(barrack, position)));

		//Spawn the unit (in fight phase) and return to prepare phase while validating the checkpoint
		overview.AdvancePhase();
		barrack.Spawn();
		overview.AdvancePhase();
		Unit unit = overview.World.Units.First();
		Assert.AreEqual(position, unit.NextCheckpoint);

		//Make the checkpoint unreachable
		ITowerTypeData towerType = new GameTestUtils.TowerTypeData { Damage = 0 };
		BuildTowersAroundPosition(barrack.Owner, towerType, position);

		//Validate results: unit checkpoint got changed after entering fighting phase
		overview.AdvancePhase();
		TilePosition enemyCastle = overview.GetEnemyTeam(unit.Owner).Castle.Position;
		Assert.AreEqual(enemyCastle, unit.NextCheckpoint);
	}

	private TilePosition FindValidCheckpoint(Barrack barrack) {
		GameWorld world = barrack.World;

		TilePosition start = barrack.Position;
		HashSet<TilePosition> possibilities = new HashSet<TilePosition> {
			start.Added(1, 0), start.Added(-1, 0), start.Added(0, 1), start.Added(0, -1)
		};
		possibilities.RemoveWhere(pos => pos.X < 0 || pos.Y < 0
			|| pos.X == world.Width || pos.Y == world.Height);

		TilePosition enemyCastle = world.Overview.GetEnemyTeam(barrack.Owner).Castle.Position;
		return world.Navigation.GetReachablePositionSubset(enemyCastle, possibilities).First();
	}

	private TilePosition FindFree5X5AreaCenterThatIsValidCheckpoint(Barrack barrack) {
		GameWorld world = barrack.World;

		ISet<TilePosition> possibilities = new HashSet<TilePosition>();
		//Don't include positions that are on the edge: this way sides (3x3 area) are really free
		for (int x = 1; x < world.Width - 1; x++) {
			for (int y = 1; y < world.Height - 1; y++) {
				TilePosition pos = new TilePosition(x, y);
				if (world[pos] == null) possibilities.Add(pos);
			}
		}

		possibilities = world.Navigation.GetReachablePositionSubset(barrack.Position, possibilities);
		TilePosition enemyCastle = barrack.World.Overview.GetEnemyTeam(barrack.Owner).Castle.Position;
		possibilities = world.Navigation.GetReachablePositionSubset(enemyCastle, possibilities);

		foreach (TilePosition pos in possibilities) {
			bool valid = true;

			for (int x = -2; x <= 2; x++) {
				for (int y = -2; y <= 2; y++) {
					TilePosition newPos = pos.Added(x, y);
					if (newPos.X < 0 || newPos.Y < 0
						|| newPos.X >= world.Width || newPos.Y >= world.Height)
						continue;
					if (world[newPos] != null) {
						valid = false;
						break;
					}
				}
			}

			if (valid) return pos;
		}

		throw new Exception("Unable to find such a free position");
	}

	private void BuildTowersAroundPosition(GameTeam team, ITowerTypeData towerType, TilePosition position) {
		int[] dx = { 1, -1, 0, 0 };
		int[] dy = { 0, 0, 1, -1 };
		for (int i = 0; i < dx.Length; i++)
			Assert.AreEqual(BuildTowerCommand.CommandResult.Success,
				team.Overview.Commands.Issue(new BuildTowerCommand(team, towerType, position.Added(dx[i], dy[i]))));
	}
}

}
