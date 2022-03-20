using System;
using System.Collections.Generic;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {
public class WorldNavigationTest {
	private const int LowRepeatCount = 10;
	private const int HighRepeatCount = 100;

	[Test]
	public void TestPathToSelf() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		List<Vector2> path = world.Navigation.TryGetPathDeltas(new Vector2(1.5F, 1.5F), new Vector2(1.5F, 1.5F), 0);
		Assert.IsTrue(path.Count == 0);
	}

	[Test]
	[Repeat((LowRepeatCount))]
	public void TestPathLeadsToTarget() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		Random random = new Random();
		Vector2 from = new Vector2((random.Next() % world.Width) + 0.5F, (random.Next() % world.Height) + 0.5F);
		Vector2 to = new Vector2((random.Next() % world.Width) + 0.5F, (random.Next() % world.Height) + 0.5F);
		List<Vector2> path = world.Navigation.TryGetPathDeltas(from, to, 0);
		float x = from.X;
		float y = from.Y;
		foreach (var delta in path) {
			x += delta.X;
			y += delta.Y;
		}

		Assert.IsTrue(Single.Equals(to.X, x));
		Assert.IsTrue(Single.Equals(to.Y, y));
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacles() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		Random random = new Random();
		Vector2 from = new Vector2((random.Next() % world.Width) + 0.5F, (random.Next() % world.Height) + 0.5F);
		Vector2 to = new Vector2(0.5F, 0.5F);
		List<Vector2> path = world.Navigation.TryGetPathDeltas(from, to, 0);
		float x = from.X;
		float y = from.Y;
		if (path.Count > 0) path.RemoveAt(path.Count - 1);
		foreach (var delta in path) {
			x += delta.X;
			y += delta.Y;
			Assert.IsNull(world[(int) x, (int) y]);
		}
	}
}
}
