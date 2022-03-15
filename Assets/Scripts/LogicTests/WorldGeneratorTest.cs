using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Logic.Data;
using Logic.Data.World;
using Moq;
using NUnit.Framework;

namespace LogicTests {

public class WorldGeneratorTest {
	private const int LowRepeatCount = 10;
	private const int HighRepeatCount = 100;

	[Test]
	public void TestGenerationSpeed() {
		const double maxSecondsPerWorld = 0.1;

		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (var i = 0; i < HighRepeatCount; i++) {
			GenerateWorld();
		}

		stopwatch.Stop();
		Assert.Less(stopwatch.Elapsed.TotalSeconds, maxSecondsPerWorld * HighRepeatCount);
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestCastleCount() {
		Assert.AreEqual(2, GenerateWorld().TileObjects.Count(o => o is Castle));
		foreach (Color color in Enum.GetValues(typeof(Color))) {
			Assert.AreEqual(1, GenerateWorld().TileObjects
				.Where(o => o is Castle)
				.Count(o => ((Building) o).OwnerColor == color));
		}
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestBarrackCount() {
		Assert.AreEqual(4, GenerateWorld().TileObjects.Count(o => o is Barrack));
		foreach (Color color in Enum.GetValues(typeof(Color))) {
			Assert.AreEqual(2, GenerateWorld().TileObjects
				.Where(o => o is Barrack)
				.Count(o => ((Building) o).OwnerColor == color));
		}
	}

	[Test]
	[Repeat(LowRepeatCount)]
	public void TestObstacleCount() {
		int count = GenerateWorld().TileObjects
			.Count(o => o is Obstacle);
		Assert.Greater(count, 1);
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacleNearCastle() {
		GameWorld world = GenerateWorld();

		ICollection<TilePosition> castles = world.TileObjects
			.Where(o => o is Castle)
			.Select(o => o.Position)
			.ToList();

		Assert.IsTrue(world.TileObjects
			.Where(o => o is Obstacle)
			.Select(o => o.Position)
			.All(o => castles.All(p => o.Distance2(p) > 4)));
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacleNearBarrack() {
		GameWorld world = GenerateWorld();

		ICollection<TilePosition> barracks = world.TileObjects
			.Where(o => o is Barrack)
			.Select(o => o.Position)
			.ToList();

		Assert.IsTrue(world.TileObjects
			.Where(o => o is Obstacle)
			.Select(o => o.Position)
			.All(o => barracks.All(p => o.Distance2(p) > 2)));
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestCastleCastleDistance() {
		GameWorld world = GenerateWorld();
		int minDistance = Math.Min(world.Width, world.Height) / 4;

		ICollection<TilePosition> castles = world.TileObjects
			.Where(o => o is Castle)
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
		GameWorld world = GenerateWorld();
		int minDistance = Math.Min(world.Width, world.Height) / 10;

		ICollection<TilePosition> barracks = world.TileObjects
			.Where(o => o is Barrack)
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
		GameWorld world = GenerateWorld();

		foreach (Barrack barrack in world.TileObjects.Where(o => o is Barrack).Cast<Barrack>()) {
			double friendly = world.TileObjects
				.Where(o => o is Castle c && c.OwnerColor == barrack.OwnerColor)
				.Select(o => barrack.Position.Distance(o.Position))
				.Max();
			double enemy = world.TileObjects
				.Where(o => o is Castle c && c.OwnerColor != barrack.OwnerColor)
				.Select(o => barrack.Position.Distance(o.Position))
				.Min();
			Assert.Less(friendly, 1.5 * enemy);
		}
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestEnemyCastleReachable() {
		GameWorld world = GenerateWorld();

		foreach (Barrack barrack in world.TileObjects.Where(o => o is Barrack).Cast<Barrack>()) {
			foreach (TileObject enemy in world.TileObjects
				.Where(o => o is Castle c && c.OwnerColor != barrack.OwnerColor)) {
				Assert.IsTrue(world.Navigation.IsPositionReachable(barrack.Position, enemy.Position));
			}
		}
	}

	private GameWorld GenerateWorld() {
		Random random = new Random();
		int width = random.Next(10, 15);
		int height = random.Next(10, 15);

		Mock<IGameOverview> overview = new Mock<IGameOverview>();
		overview.Setup(o => o.Random).Returns(new Random());
		GameWorld world = new GameWorld(overview.Object, width, height);

		TestContext.Out.WriteLine("Generated world:");
		TestContext.Out.WriteLine(WorldAsMultilineString(world));
		return world;
	}

	private static string WorldAsMultilineString(GameWorld world) {
		const char boundary = '#';
		StringBuilder result = new StringBuilder();
		result.Append(boundary, world.Width + 2);
		result.Append(Environment.NewLine);

		for (var y = 0; y < world.Height; y++) {
			result.Append(boundary);

			for (var x = 0; x < world.Width; x++) {
				result.Append(world.GetTile(x, y) switch {
					null => ' ',
					Castle c => c.OwnerColor == Color.Red ? 'R' : 'B',
					Barrack b => b.OwnerColor == Color.Red ? 'r' : 'b',
					Obstacle _ => 'X',
					var v => throw new Exception($"Unexpected object: {v}")
				});
			}

			result.Append(boundary);
			result.Append(Environment.NewLine);
		}

		result.Append(boundary, world.Width + 2);
		result.Append(Environment.NewLine);
		return result.ToString();
	}
}

}
