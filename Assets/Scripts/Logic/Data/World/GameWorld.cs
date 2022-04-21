﻿using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class GameWorld {
	#region Fields

	private readonly TileObject[,] _grid;

	private readonly IList<Unit> _units = new List<Unit>();

	#endregion

	#region Properties

	public IGameOverview Overview { get; }

	public int Width => Config.Width;

	public int Height => Config.Height;

	public TileObject this[int x, int y] => GetTile(x, y);

	public TileObject this[TilePosition position] => GetTile(position.X, position.Y);

	public IEnumerable<TileObject> TileObjects {
		get {
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if (_grid[x, y] != null)
						yield return _grid[x, y];
		}
	}

	internal WorldNavigation Navigation { get; }

	public IReadOnlyCollection<Unit> Units => new List<Unit>(_units);

	public IGameWorldConfig Config { get; }

	#endregion

	#region Methods

	internal GameWorld(IGameOverview overview, IGameWorldConfig config) {
		Overview = overview;
		Config = config;
		_grid = WorldGenerator.GenerateGrid(overview.Random.Next(), Width, Height,
			config.GenerateObstacles, new TileObjectConstructors(this));
		Navigation = new WorldNavigation(_grid);
	}

	public TileObject GetTile(int x, int y) {
		return _grid[x, y];
	}

	public IEnumerable<T> GetTileObjectsOfType<T>() where T : TileObject {
		return TileObjects.Where(o => o is T).Cast<T>();
	}

	internal void BuildTower(GameTeam team, ITowerTypeData type, TilePosition position) {
		if (_grid[position.X, position.Y] != null)
			throw new ArgumentException($"Position {position} is already occupied");

		Tower tower = new Tower(this, position, team.TeamColor, type);
		_grid[position.X, position.Y] = tower;
		Overview.Events.Raise(new TowerBuiltEvent(tower));
	}

	internal void DestroyTower(Tower tower) {
		if (_grid[tower.Position.X, tower.Position.Y] != tower)
			throw new ArgumentException("Tower is not at the position it says it is");

		_grid[tower.Position.X, tower.Position.Y] = null;
		Overview.Events.Raise(new TowerDestroyedEvent(tower));
	}

	internal void DeployUnit(Barrack barrack, IUnitTypeData type) {
		Vector2 position = barrack.Position.ToVectorCentered();
		Unit unit = new Unit(type, barrack.Owner, this, position, barrack.CheckPoints);
		_units.Add(unit);
		Overview.Events.Raise(new UnitDeployedEvent(unit, barrack));
	}

	internal void DestroyUnit(Unit unit) {
		if (!_units.Contains(unit)) throw new ArgumentException("Unit is not among this world's units");

		_units.Remove(unit);
		Overview.Events.Raise(new UnitDestroyedEvent(unit));
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
