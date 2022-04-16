using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Logic.Data;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the procedural world generation by repeatedly generating new worlds
/// and checking whether they adhere to specific criteria (e.g. the two castles are far enough away).
/// </summary>
public class WorldGeneratorTest {
	private const int LowRepeatCount = 10;
	private const int HighRepeatCount = 100;

	[Test]
	public void TestGenerationSpeed() {
		const double maxSecondsPerWorld = 0.1;

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (var i = 0; i < HighRepeatCount; i++) {
			WorldTestUtils.GenerateWorld();
		}

		stopwatch.Stop();
		Assert.Less(stopwatch.Elapsed.TotalSeconds, maxSecondsPerWorld * HighRepeatCount);
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestCastleCount() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		Assert.AreEqual(2, world.GetTileObjectsOfType<Castle>().Count());
		foreach (Color color in Enum.GetValues(typeof(Color))) {
			Assert.AreEqual(1, world.GetTileObjectsOfType<Castle>()
				.Count(o => o.OwnerColor == color));
		}
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestBarrackCount() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		Assert.AreEqual(4, world.GetTileObjectsOfType<Barrack>().Count());
		foreach (Color color in Enum.GetValues(typeof(Color))) {
			Assert.AreEqual(2, world.GetTileObjectsOfType<Barrack>()
				.Count(o => o.OwnerColor == color));
		}
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestObstacleCount() {
		Assert.Greater(WorldTestUtils.GenerateWorld().GetTileObjectsOfType<Obstacle>().Count(), 1);
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacleNearCastle() {
		GameWorld world = WorldTestUtils.GenerateWorld();

		ICollection<TilePosition> castles = world.GetTileObjectsOfType<Castle>()
			.Select(o => o.Position)
			.ToList();

		Assert.IsTrue(world.GetTileObjectsOfType<Obstacle>()
			.Select(o => o.Position)
			.All(o => castles.All(p => o.FirstNormDistance(p) >= 4)));
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacleNearBarrack() {
		GameWorld world = WorldTestUtils.GenerateWorld();

		ICollection<TilePosition> barracks = world.GetTileObjectsOfType<Barrack>()
			.Select(o => o.Position)
			.ToList();

		Assert.IsTrue(world.GetTileObjectsOfType<Obstacle>()
			.Select(o => o.Position)
			.All(o => barracks.All(p => o.FirstNormDistance(p) >= 4)));
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestCastleCastleDistance() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		int minDistance = Math.Min(world.Width, world.Height) / 4;

		ICollection<TilePosition> castles = world.GetTileObjectsOfType<Castle>()
			.Select(o => o.Position)
			.ToList();

		foreach (TilePosition a in castles) {
			foreach (TilePosition b in castles) {
				if (!a.Equals(b)) {
					Assert.Greater(a.Distance(b), minDistance);
				}
			}
		}
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestBarrackBarrackDistance() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		int minDistance = Math.Min(world.Width, world.Height) / 10;

		ICollection<TilePosition> barracks = world.GetTileObjectsOfType<Barrack>()
			.Select(o => o.Position)
			.ToList();

		foreach (TilePosition a in barracks) {
			foreach (TilePosition b in barracks) {
				if (!a.Equals(b)) {
					Assert.Greater(a.Distance(b), minDistance);
				}
			}
		}
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestBarrackCastleDistance() {
		GameWorld world = WorldTestUtils.GenerateWorld();

		foreach (Barrack barrack in world.GetTileObjectsOfType<Barrack>()) {
			foreach (Castle castle in world.GetTileObjectsOfType<Castle>()) {
				Assert.GreaterOrEqual(barrack.Position.FirstNormDistance(castle.Position), 3);
			}
		}
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestEnemyCastleReachable() {
		GameWorld world = WorldTestUtils.GenerateWorld();

		foreach (Barrack barrack in world.GetTileObjectsOfType<Barrack>()) {
			foreach (Castle enemy in world.GetTileObjectsOfType<Castle>()
				.Where(c => c.OwnerColor != barrack.OwnerColor)) {
				Assert.IsTrue(world.Navigation.IsPositionReachable(barrack.Position, enemy.Position));
			}
		}
	}
}

}
