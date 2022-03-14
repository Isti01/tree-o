using System;
using System.Collections.Generic;

namespace Logic.Data.World {
public class GameWorld {

	#region Fields

	private readonly TileObject[,] _grid;

	#endregion

	#region Properties

	public GameOverview Overview { get; }

	public int Width { get; }

	public int Height { get; }

	public TileObject this[int x, int y] => GetTile(x, y);

	public IEnumerable<TileObject> TileObjects {
		get {
			for (var x = 0; x < Width; x++)
				for (var y = 0; y < Height; y++)
					if (_grid[x, y] != null)
						yield return _grid[x, y];
		}
	}

	public WorldNavigation Navigation { get; }

	public IReadOnlyCollection<Unit> Units { get; }

	#endregion

	#region Methods

	public GameWorld(GameOverview overview, int width, int height) {
		Overview = overview;
		Width = width;
		Height = height;
		_grid = WorldGenerator.GenerateGrid(overview.Random.Next(), Width, Height, new TileObjectConstructors(this));
		Navigation = new WorldNavigation(_grid);
	}

	public TileObject GetTile(int x, int y)
	{
		return _grid[x, y];

	}

	public void BuildTower(GameTeam team, ITowerTypeData data, TilePosition position) {
		throw new NotImplementedException();
	}
	public void DestroyTower(Tower tower) {
		throw new NotImplementedException();
	}

	#endregion

	private class TileObjectConstructors : WorldGenerator.ITileObjectConstructors {
		private readonly GameWorld _world;

		public TileObjectConstructors(GameWorld world) {
			_world = world;
		}

		public Castle CreateCastle(TilePosition position, Color team) {
			return new Castle(_world, position, team);
		}

		public Barrack CreateBarrack(TilePosition position, Color team) {
			return new Barrack(_world, position, team);
		}

		public Obstacle CreateObstacle(TilePosition position) {
			return new Obstacle(_world, position);
		}
	}
}
}
