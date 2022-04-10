using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Data.World {
internal class WorldGenerator {
	public static TileObject[,] GenerateGrid(int seed, int width, int height, ITileObjectConstructors constructors) {
		WorldGenerator generator = new WorldGenerator(seed, width, height, constructors);
		generator.GenerateWorld();
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

	private void GenerateWorld() {
		List<TileObject> occupieds = new List<TileObject>();
		List<TileObject> otherside = new List<TileObject>();
		TilePosition tp;
		int x = _random.Next() % _width;
		int y = _random.Next() % (_height / 5);
		occupieds.Add(_constructors.CreateCastle(new TilePosition(x, y), Color.Red));
		do {
			x = _random.Next() % _width;
			y = _random.Next() % (_height / 5);
			tp = new TilePosition(x, y);
		} while (occupieds.Any(occupied => occupied.Position.FirstNormDistance(tp) < 3));

		occupieds.Add(_constructors.CreateBarrack(new TilePosition(x, y), Color.Red));
		do {
			x = _random.Next() % _width;
			y = _random.Next() % (_height / 5);
			tp = new TilePosition(x, y);
		} while (occupieds.Any(occupied => occupied.Position.FirstNormDistance(tp) < 3));

		occupieds.Add(_constructors.CreateBarrack(new TilePosition(x, y), Color.Red));

		int obstacleCount = (_random.Next() % ((_width * _height) / 60)) + 2;
		int i = 0, rep = 0;
		while (i < obstacleCount && rep < 1000) {
			++rep;
			int obstacleSize = _random.Next() % 3 + 1;
			x = _random.Next() % (_width - obstacleSize);
			y = _random.Next() % ((_height / 2) - obstacleSize);
			bool allGood = true;
			for (int j = 0; j < obstacleSize && allGood; j++) {
				for (int k = 0; k < obstacleSize && allGood; k++) {
					tp = new TilePosition(x + j, y + k);
					allGood = !occupieds.Where(building => !(building is Obstacle))
							.Any(occupied => occupied.Position.FirstNormDistance(tp) < 4)
						&& !occupieds.Any(obj => obj.Position.Equals(tp));
				}
			}

			if (allGood) {
				for (int j = 0; j < obstacleSize; j++) {
					for (int k = 0; k < obstacleSize; k++) {
						occupieds.Add(_constructors.CreateObstacle(new TilePosition(x + j, y + k)));
					}
				}

				++i;
			}
		}

		foreach (var castle in occupieds.Where(castle => castle is Castle)) {
			otherside.Add(_constructors.CreateCastle(
				new TilePosition(_width - castle.Position.X - 1, _height - castle.Position.Y - 1), Color.Blue));
		}

		foreach (var barrack in occupieds.Where(barrack => barrack is Barrack)) {
			otherside.Add(_constructors.CreateBarrack(
				new TilePosition(_width - barrack.Position.X - 1, _height - barrack.Position.Y - 1), Color.Blue));
		}

		foreach (var obstacle in occupieds.Where(obstacle => obstacle is Obstacle)) {
			otherside.Add(_constructors.CreateObstacle(new TilePosition(_width - obstacle.Position.X - 1,
				_height - obstacle.Position.Y - 1)));
		}

		foreach (var obj in occupieds) {
			_grid[obj.Position.X, obj.Position.Y] = obj;
		}

		foreach (var obj in otherside) {
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
