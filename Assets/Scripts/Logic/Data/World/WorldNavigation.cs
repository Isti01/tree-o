﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Data.World {
public class WorldNavigation {
	private readonly TileObject[,] _grid;

	public WorldNavigation(TileObject[,] grid) {
		_grid = grid;
	}

	public bool IsPositionReachable(TilePosition from, TilePosition to, ISet<TilePosition> blockedTiles) {
		return GetReachablePositionSubset(from, new HashSet<TilePosition> { to }, blockedTiles).Any();
	}

	public bool IsPositionReachable(TilePosition from, TilePosition to) {
		return GetReachablePositionSubset(from, new HashSet<TilePosition> { to }).Any();
	}

	public ISet<TilePosition> GetReachablePositionSubset(TilePosition from, ISet<TilePosition> to,
		ISet<TilePosition> blockedTiles) {
		if (blockedTiles.Overlaps(to))
			throw new ArgumentException("Positions can't be blocked and be 'to' positions at the same time");

		//We modify the grid. We must be careful not to call any "foreign" code here:
		// outside observers mustn't see the grid in this modified state.

		foreach (TilePosition tile in blockedTiles) {
			if (_grid[tile.X, tile.Y] == null) {
				_grid[tile.X, tile.Y] = new FillerTileObject(tile);
			}
		}

		ISet<TilePosition> result = GetReachablePositionSubset(from, to);

		foreach (TilePosition tile in blockedTiles) {
			if (_grid[tile.X, tile.Y] is FillerTileObject) {
				_grid[tile.X, tile.Y] = null;
			}
		}

		return result;
	}

	public ISet<TilePosition> GetReachablePositionSubset(TilePosition from, ISet<TilePosition> to) {
		Dijkstra[,] dgrid = RunDijkstra(from.X, from.Y, to);
		HashSet<TilePosition> result = new HashSet<TilePosition>(to);
		result.RemoveWhere(pos => dgrid[pos.X, pos.Y].D == int.MaxValue);
		return result;
	}

	private Dijkstra[,] RunDijkstra(int fromX, int fromY, ISet<TilePosition> to) {
		int width = _grid.GetLength(0);
		int height = _grid.GetLength(1);

		Dijkstra[,] dgrid = new Dijkstra[width, height];
		for (var i = 0; i < width; i++) {
			for (var j = 0; j < height; j++) {
				dgrid[i, j] = new Dijkstra(i, j);
			}
		}

		dgrid[fromX, fromY].D = 0;
		IList<Dijkstra> queue = new List<Dijkstra>(width * height);
		queue.Add(dgrid[fromX, fromY]);
		dgrid[fromX, fromY].Queued = true;

		int[] diffX = { 0, 0, -1, 1 };
		int[] diffY = { 1, -1, 0, 0 };

		while (queue.Count > 0) {
			int minValue = queue[0].D;
			var minIndex = 0;
			for (var i = 1; i < queue.Count; i++) {
				if (minValue <= queue[i].D) continue;
				minValue = queue[i].D;
				minIndex = i;
			}

			Dijkstra u = queue[minIndex];
			queue.RemoveAt(minIndex);

			if (u.D == int.MaxValue) break;

			//'to' positions have finite cost but can be blocked
			if ((u.Ox == fromX && u.Oy == fromY) || _grid[u.Ox, u.Oy] == null) {
				for (var i = 0; i < 4; i++) {
					int newX = u.Ox + diffX[i];
					int newY = u.Oy + diffY[i];
					if (newX < 0 || newY < 0 || newX >= width || newY >= height) continue;

					Dijkstra newU = dgrid[newX, newY];
					if (newU.D <= u.D + 1) continue;
					if (_grid[newX, newY] != null && !to.Contains(new TilePosition(newX, newY))) continue;
					newU.D = u.D + 1;
					newU.Px = u.Ox;
					newU.Py = u.Oy;

					if (newU.Queued) continue;
					newU.Queued = true;
					queue.Add(newU);
				}
			}
		}

		return dgrid;
	}

	public List<Vector2> TryGetPathDeltas(Vector2 from, Vector2 to, float colliderRadius) {
		if (from.ToTilePosition().Equals(to.ToTilePosition())) return new List<Vector2>();

		Dijkstra[,] dgrid = RunDijkstra((int) from.X, (int) from.Y,
			new HashSet<TilePosition> { to.ToTilePosition() });
		if (dgrid[(int) to.X, (int) to.Y].D == int.MaxValue) return null;

		List<Vector2> pathDeltas = new List<Vector2>();

		Dijkstra target = dgrid[(int) to.X, (int) to.Y];
		Dijkstra prev = dgrid[target.Px, target.Py];
		while (!(prev.Ox == (int) from.X && prev.Oy == (int) from.Y)) {
			pathDeltas.Add(new Vector2(target.Ox - prev.Ox, target.Oy - prev.Oy));
			target = prev;
			prev = dgrid[prev.Px, prev.Py];
		}

		pathDeltas.Add(new Vector2(target.Ox + 0.5F - from.X, target.Oy + 0.5F - from.Y));

		pathDeltas.Reverse();
		return pathDeltas;
	}

	private class FillerTileObject : TileObject {
		public FillerTileObject(TilePosition position) : base(null, position) {}
	}
}
}
