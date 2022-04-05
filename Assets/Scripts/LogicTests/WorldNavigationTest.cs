using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {
public class WorldNavigationTest {
	private const int LowRepeatCount = 10;
	private const int HighRepeatCount = 100;

	[Test]
	public void CanReachOwnEmptyTile() {
		WorldNavigation navigation = CreateNavigation(CreateGrid(3, 3));
		var tile = new TilePosition(1, 1);
		Assert.IsTrue(navigation.IsPositionReachable(tile, tile));
	}

	[Test]
	public void CanReachOwnFilledTile() {
		var tile = new TilePosition(1, 1);
		WorldNavigation navigation = CreateNavigation(CreateGrid(3, 3, tile));
		Assert.IsTrue(navigation.IsPositionReachable(tile, tile));
	}

	[Test]
	public void CanReachEmptyTile() {
		WorldNavigation navigation = CreateNavigation(CreateGrid(5, 3));
		var from = new TilePosition(1, 1);
		var to = new TilePosition(4, 1);
		Assert.IsTrue(navigation.IsPositionReachable(from, to));
	}

	[Test]
	public void CanReachFilledTile() {
		var to = new TilePosition(4, 1);
		WorldNavigation navigation = CreateNavigation(CreateGrid(5, 3, to));
		var from = new TilePosition(1, 1);
		Assert.IsTrue(navigation.IsPositionReachable(from, to));
	}

	[Test]
	public void CanNotReachEmptyButUnreachableTile() {
		WorldNavigation navigation = CreateNavigation(CreateGrid(3, 1, new TilePosition(1, 0)));
		var from = new TilePosition(0, 0);
		var to = new TilePosition(2, 0);
		Assert.IsFalse(navigation.IsPositionReachable(from, to));
	}

	[Test]
	public void CanReachLineOfSightBlockedButReachableEmptyTile() {
		WorldNavigation navigation = CreateNavigation(CreateGrid(7, 3, new TilePosition(3, 1)));
		var from = new TilePosition(1, 1);
		var to = new TilePosition(5, 1);
		Assert.IsTrue(navigation.IsPositionReachable(from, to));
	}

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
		Random random = RandomUtils.CreateRandomlySeededRandom();
		Vector2 from = new Vector2((random.Next() % world.Width) + 0.5F, (random.Next() % world.Height) + 0.5F);
		Vector2 to = new Vector2((random.Next() % world.Width) + 0.5F, (random.Next() % world.Height) + 0.5F);
		List<Vector2> path = world.Navigation.TryGetPathDeltas(from, to, 0);
		float x = from.X;
		float y = from.Y;
		foreach (var delta in path) {
			x += delta.X;
			y += delta.Y;
		}

		Assert.AreEqual(to.X, x);
		Assert.AreEqual(to.Y, y);
	}

	[Test]
	[Repeat(HighRepeatCount)]
	public void TestNoObstacles() {
		GameWorld world = WorldTestUtils.GenerateWorld();
		foreach (var to in world.GetTileObjectsOfType<Castle>()) {
			foreach (var from in world.GetTileObjectsOfType<Barrack>().Where(obj => !obj.OwnerColor.Equals(to.OwnerColor))) {
				List<Vector2> path = world.Navigation.TryGetPathDeltas(from.Position.ToVectorCentered(),
					to.Position.ToVectorCentered(), 0);
				float x = from.Position.X;
				float y = from.Position.Y;
				path.RemoveAt(path.Count - 1);
				foreach (var delta in path) {
					x += delta.X;
					y += delta.Y;
					Assert.IsNull(world[(int) x, (int) y]);
				}
			}
		}
	}

	private static WorldNavigation CreateNavigation(TileObject[,] grid) {
		return new WorldNavigation(grid);
	}

	private static TileObject[,] CreateGrid(int width, int height, params TilePosition[] obstacles) {
		TileObject[,] grid = new TileObject[width, height];
		foreach (TilePosition position in obstacles) {
			grid[position.X, position.Y] = new FillerTileObject(position);
		}

		return grid;
	}

	private class FillerTileObject : TileObject {
		public FillerTileObject(TilePosition position) : base(null, position) {}
	}
}
}
