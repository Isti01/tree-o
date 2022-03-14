using System;
using System.Collections.Generic;

namespace Logic.Data.World {

public class WorldGenerator {
	public static TileObject[,] GenerateGrid(int seed, int width, int height, ITileObjectConstructors constructors) {
		WorldGenerator generator = new WorldGenerator(seed, width, height, constructors);
		generator.GenerateHardcodedWorld();

		//TODO probably use this approach for the real implementation:
		// - generate half a map
		// - mirror the map

		return generator._grid;
	}

	private readonly Random _random;
	private readonly int _width;
	private readonly int _height;
	private readonly ITileObjectConstructors _constructors;
	private readonly TileObject[,] _grid;

	private WorldGenerator(int seed, int width, int height, ITileObjectConstructors constructors) {
		if (width < 8 || height < 8) throw new ArgumentException("World must be at 8x8");
		_random = new Random(seed);
		_width = width;
		_height = height;
		_constructors = constructors;
		_grid = new TileObject[width, height];
	}

	private IEnumerable<T> EnumerateType<T>() where T : TileObject {
		for (var x = 0; x < _width; x++) {
			for (var y = 0; y < _height; y++) {
				if (_grid[x, y] is T obj) {
					yield return obj;
				}
			}
		}
	}

	private void GenerateHardcodedWorld() {
		Castle castleA = _constructors.CreateCastle(new TilePosition(0, 0), Color.Blue);
		Barrack barrackA1 = _constructors.CreateBarrack(new TilePosition(0, 2), Color.Blue);
		Barrack barrackA2 = _constructors.CreateBarrack(new TilePosition(2, 0), Color.Blue);

		Castle castleB = _constructors.CreateCastle(new TilePosition(_width - 1, _height - 1), Color.Red);
		Barrack barrackB1 = _constructors.CreateBarrack(new TilePosition(_width - 1, _height - 3), Color.Red);
		Barrack barrackB2 = _constructors.CreateBarrack(new TilePosition(_width - 3, _height - 1), Color.Red);

		Obstacle obstacle1 = _constructors.CreateObstacle(new TilePosition(2, 2));
		Obstacle obstacle2 = _constructors.CreateObstacle(new TilePosition(_width - 3, _height - 3));
		Obstacle obstacle3 = _constructors.CreateObstacle(new TilePosition(_width / 2, _height / 2));

		foreach (TileObject obj in new TileObject[] {
				castleA, barrackA1, barrackA2, castleB, barrackB1, barrackB2, obstacle1, obstacle2, obstacle3
			}) {
			_grid[obj.Position.X, obj.Position.Y] = obj;
		}
	}

	public interface ITileObjectConstructors {
		public Castle CreateCastle(TilePosition position, Color team);
		public Barrack CreateBarrack(TilePosition position, Color team);
		public Obstacle CreateObstacle(TilePosition position);
	}
}

}
