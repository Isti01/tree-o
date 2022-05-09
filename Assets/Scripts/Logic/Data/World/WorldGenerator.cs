using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Data.World {
/// <summary>
/// Class for generating the world.
/// </summary>
internal class WorldGenerator {
	/// <summary>
	/// Generates the world.
	/// </summary>
	/// <param name="seed">Seed for randomisation.</param>
	/// <param name="width">The width of the world.</param>
	/// <param name="height">The height of the world.</param>
	/// <param name="generateObstacles">If false, obstacles won't be generated.</param>
	/// <param name="constructors">The constructors for the TileObjects.</param>
	/// <returns>The grid of the world.</returns>
	public static TileObject[,] GenerateGrid(int seed, int width, int height,
		bool generateObstacles, ITileObjectConstructors constructors) {
		WorldGenerator generator = new WorldGenerator(seed, width, height, generateObstacles, constructors);
		generator.GenerateWorld();
		return generator._grid;
	}

	private readonly Random _random;
	private readonly int _width;
	private readonly int _height;
	private readonly bool _generateObstacles;
	private readonly ITileObjectConstructors _constructors;
	private readonly TileObject[,] _grid;

	private WorldGenerator(int seed, int width, int height,
		bool generateObstacles, ITileObjectConstructors constructors) {
		if (width < 8 || height < 8) throw new ArgumentException("World must be at 8x8");
		_random = new Random(seed);
		_width = width;
		_height = height;
		_generateObstacles = generateObstacles;
		_constructors = constructors;
		_grid = new TileObject[width, height];
	}

	private void GenerateWorld() {
		List<TileObject> occupiedList = new List<TileObject>();
		TilePosition tp;
		int x = _random.Next() % _width;
		int y = _random.Next() % (_height / 5);
		occupiedList.Add(_constructors.CreateCastle(new TilePosition(x, y), Color.Red));
		do {
			x = _random.Next() % _width;
			y = _random.Next() % (_height / 5);
			tp = new TilePosition(x, y);
		} while (occupiedList.Any(occupied => occupied.Position.FirstNormDistance(tp) < 3));

		occupiedList.Add(_constructors.CreateBarrack(new TilePosition(x, y), Color.Red));
		do {
			x = _random.Next() % _width;
			y = _random.Next() % (_height / 5);
			tp = new TilePosition(x, y);
		} while (occupiedList.Any(occupied => occupied.Position.FirstNormDistance(tp) < 3));

		occupiedList.Add(_constructors.CreateBarrack(new TilePosition(x, y), Color.Red));

		int obstacleCount = _generateObstacles ? (_random.Next() % ((_width * _height) / 40)) + 2 : 0;
		int i = 0, rep = 0;
		while (i < obstacleCount && rep < 1000) {
			++rep;
			int obstacleSize = _random.Next() % 2 + 1;
			x = _random.Next() % (_width - obstacleSize);
			y = _random.Next() % ((_height / 2) - obstacleSize);
			bool allGood = true;
			for (int j = 0; j < obstacleSize && allGood; j++) {
				for (int k = 0; k < obstacleSize && allGood; k++) {
					tp = new TilePosition(x + j, y + k);
					allGood = !occupiedList.Where(building => !(building is Obstacle))
							.Any(occupied => occupied.Position.FirstNormDistance(tp) < 4)
						&& !occupiedList.Any(obj => obj.Position.Equals(tp));
				}
			}

			if (allGood) {
				for (int j = 0; j < obstacleSize; j++) {
					for (int k = 0; k < obstacleSize; k++) {
						occupiedList.Add(_constructors.CreateObstacle(new TilePosition(x + j, y + k)));
					}
				}

				++i;
			}
		}

		if (i == 0 && _generateObstacles) {
			List<int> goodIndices = new List<int>();
			for (int j = 0; j < _width; j++) {
				tp = new TilePosition(_height / 2, _width);
				if (!occupiedList.Where(building => !(building is Obstacle))
					.Any(occupied => occupied.Position.FirstNormDistance(tp) < 4)) {
					goodIndices.Add(j);
				}
			}

			occupiedList.Add(_constructors.CreateObstacle(new TilePosition(
				goodIndices[_random.Next() % goodIndices.Count],
				_height / 2)));
		}

		List<TileObject> otherSide = new List<TileObject>();
		foreach (TileObject tileObject in occupiedList) {
			TilePosition newPos = new TilePosition(_width, _height)
				.Subtracted(tileObject.Position).Subtracted(1, 1);
			if (tileObject is Castle) {
				otherSide.Add(_constructors.CreateCastle(newPos, Color.Blue));
			} else if (tileObject is Barrack) {
				otherSide.Add(_constructors.CreateBarrack(newPos, Color.Blue));
			} else if (tileObject is Obstacle) {
				otherSide.Add(_constructors.CreateObstacle(newPos));
			} else {
				throw new Exception($"Unexpected tile object: {tileObject}");
			}
		}

		foreach (TileObject obj in occupiedList) {
			_grid[obj.Position.X, obj.Position.Y] = obj;
		}

		foreach (TileObject obj in otherSide) {
			_grid[obj.Position.X, obj.Position.Y] = obj;
		}
	}

	/// <summary>
	/// Interface for constructing TileObjects during generation.
	/// </summary>
	public interface ITileObjectConstructors {
		public Castle CreateCastle(TilePosition position, Color team);
		public Barrack CreateBarrack(TilePosition position, Color team);
		public Obstacle CreateObstacle(TilePosition position);
	}
}
}
