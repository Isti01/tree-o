using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
/// <summary>
/// Represents the world in which the game takes place.
/// </summary>
public class GameWorld {
	#region Fields

	private readonly TileObject[,] _grid;

	private readonly IList<Unit> _units = new List<Unit>();

	#endregion

	#region Properties

	/// <summary>
	/// The overview of the game.
	/// </summary>
	public IGameOverview Overview { get; }

	/// <summary>
	///  The width of the world.
	/// </summary>
	public int Width => Config.Width;

	/// <summary>
	///  The height of the world.
	/// </summary>
	public int Height => Config.Height;

	/// <summary>
	/// The object at the given coordinates.
	/// </summary>
	/// <param name="x">The first coordinate.</param>
	/// <param name="y">The second coordinate.</param>
	public TileObject this[int x, int y] => GetTile(x, y);

	/// <summary>
	/// The object at the given TilePosition.
	/// </summary>
	/// <param name="position">The TilePosition</param>
	public TileObject this[TilePosition position] => GetTile(position.X, position.Y);

	/// <summary>
	/// All the TileObjects in the game.
	/// </summary>
	public IEnumerable<TileObject> TileObjects {
		get {
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if (_grid[x, y] != null)
						yield return _grid[x, y];
		}
	}

	/// <summary>
	/// The navigation system of the world.
	/// </summary>
	internal WorldNavigation Navigation { get; }

	/// <summary>
	/// All the units alive in the world.
	/// </summary>
	public IReadOnlyCollection<Unit> Units => new List<Unit>(_units);

	/// <summary>
	/// The configurations of the world.
	/// </summary>
	public IGameWorldConfig Config { get; }

	#endregion

	#region Methods

	/// <summary>
	/// Creates a new world. Generates the obstacles, the castles and the barracks.
	/// </summary>
	/// <param name="overview">The overview of the world.</param>
	/// <param name="config">The configurations of the world.</param>
	internal GameWorld(IGameOverview overview, IGameWorldConfig config) {
		Overview = overview;
		Config = config;
		_grid = WorldGenerator.GenerateGrid(overview.Random.Next(), Width, Height,
			config.GenerateObstacles, new TileObjectConstructors(this));
		Navigation = new WorldNavigation(_grid);
	}

	/// <param name="x">The first coordinate.</param>
	/// <param name="y">The second coordinate.</param>
	/// <returns>Returns the object at the given coordinates.</returns>
	public TileObject GetTile(int x, int y) {
		return _grid[x, y];
	}

	/// <typeparam name="T">the type of the TileObjects.</typeparam>
	/// <returns>Returns  all the TileObjects of the given type. </returns>
	public IEnumerable<T> GetTileObjectsOfType<T>() where T : TileObject {
		return TileObjects.Where(o => o is T).Cast<T>();
	}

	/// <summary>
	/// Builds a tower.
	/// </summary>
	/// <param name="team">The team of the tower.</param>
	/// <param name="type">The type of the tower.</param>
	/// <param name="position">The position of the tower.</param>
	/// <exception cref="ArgumentException">If the position is already occupied.</exception>
	internal void BuildTower(GameTeam team, ITowerTypeData type, TilePosition position) {
		if (_grid[position.X, position.Y] != null)
			throw new ArgumentException($"Position {position} is already occupied");

		Tower tower = new Tower(this, position, team.TeamColor, type);
		_grid[position.X, position.Y] = tower;
		Overview.Events.Raise(new TowerBuiltEvent(tower));
	}

	/// <summary>
	/// Destroys a tower.
	/// </summary>
	/// <param name="tower">The tower to destroy.</param>
	/// <exception cref="ArgumentException">If the Object on the grid at the tower's position isn't the tower.</exception>
	internal void DestroyTower(Tower tower) {
		if (_grid[tower.Position.X, tower.Position.Y] != tower)
			throw new ArgumentException("Tower is not at the position it says it is");

		_grid[tower.Position.X, tower.Position.Y] = null;
		Overview.Events.Raise(new TowerDestroyedEvent(tower));
	}

	/// <summary>
	/// Deploys a unit.
	/// </summary>
	/// <param name="barrack">The barrack from which the unit will be deployed.</param>
	/// <param name="type">The type of the unit.</param>
	internal void DeployUnit(Barrack barrack, IUnitTypeData type) {
		Vector2 position = barrack.Position.ToVectorCentered();
		Unit unit = new Unit(type, barrack.Owner, this, position, barrack.CheckPoints);
		_units.Add(unit);
		Overview.Events.Raise(new UnitDeployedEvent(unit, barrack));
	}

	/// <summary>
	/// Destroys a unit.
	/// </summary>
	/// <param name="unit">The unit to be destroyed.</param>
	/// <exception cref="ArgumentException">If the world doesn't have the unit.</exception>
	internal void DestroyUnit(Unit unit) {
		if (!_units.Contains(unit)) throw new ArgumentException("Unit is not among this world's units");

		_units.Remove(unit);
		Overview.Events.Raise(new UnitDestroyedEvent(unit));
	}

	#endregion

	private class TileObjectConstructors : WorldGenerator.ITileObjectConstructors {
		private readonly IDictionary<Color, int> _barrackCounter = new Dictionary<Color, int>();
		private readonly GameWorld _world;

		public TileObjectConstructors(GameWorld world) {
			_world = world;
		}

		public Castle CreateCastle(TilePosition position, Color team) {
			return new Castle(_world, position, team);
		}

		public Barrack CreateBarrack(TilePosition position, Color team) {
			_barrackCounter.TryGetValue(team, out int index);
			Barrack barrack = new Barrack(_world, position, team, index);
			_barrackCounter[team] = index + 1;
			return barrack;
		}

		public Obstacle CreateObstacle(TilePosition position) {
			return new Obstacle(_world, position);
		}
	}
}
}
