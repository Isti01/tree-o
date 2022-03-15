using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class GameWorld {

	#region Fields

	private readonly TileObject[,] _grid;

	private readonly IList<Unit> _units = new List<Unit>();

	#endregion

	#region Properties

	public IGameOverview Overview { get; }

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

	public IReadOnlyCollection<Unit> Units => new List<Unit>(_units);

	#endregion

	#region Methods

	public GameWorld(IGameOverview overview, int width, int height) {
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

	public IEnumerable<T> GetTileObjectsOfType<T>() where T : TileObject {
		return TileObjects.Where(o => o is T).Cast<T>();
	}

	public void BuildTower(GameTeam team, ITowerTypeData data, TilePosition position) {
		throw new NotImplementedException();
	}
	public void DestroyTower(Tower tower) {
		throw new NotImplementedException();
	}

	public void DeployUnit(Barrack barrack, IUnitTypeData type) {
		Vector2 position = barrack.Position.ToVectorCentered();
		//TODO Should this really be the position? I don't have any better ideas

		Unit unit = new Unit(type, barrack.Owner, this, position, barrack.CheckPoints);
		_units.Add(unit);
		Overview.Events.Raise(new UnitDeployedEvent(unit, barrack));
	}

	public void DestroyUnit(Unit unit) {
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
