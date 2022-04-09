using System;
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
		Dijkstra[,] dgrid = new Dijkstra[_grid.GetLength(0), _grid.GetLength(1)];
		for (int i = 0; i < dgrid.GetLength(0); i++) {
			for (int j = 0; j < dgrid.GetLength(1); j++) {
				dgrid[i, j] = new Dijkstra(i, j);
			}
		}

		dgrid[fromX, fromY].D = 0;
		List<Dijkstra> q = new List<Dijkstra>();
		foreach (var item in dgrid) {
			q.Add(item);
		}

		int[] difx = { 0, 0, -1, 1 };
		int[] dify = { 1, -1, 0, 0 };
		Dijkstra u = dgrid[fromX, fromY];
		q.Remove(u);
		while (u.D < Int32.MaxValue && q.Count > 0) {
			//'to' positions have finite cost but can be blocked
			if ((u.Ox == fromX && u.Oy == fromY) || _grid[u.Ox, u.Oy] == null) {
				for (int i = 0; i < 4; i++) {
					int newx = u.Ox + difx[i];
					int newy = u.Oy + dify[i];
					if (Math.Min(newx, newy) >= 0
						&& newx < dgrid.GetLength(0)
						&& newy < dgrid.GetLength(1)) {
						if (dgrid[newx, newy].D > dgrid[u.Ox, u.Oy].D + 1
							&& (_grid[newx, newy] == null || to.Contains(new TilePosition(newx, newy)))) {
							dgrid[newx, newy].D = dgrid[u.Ox, u.Oy].D + 1;
							dgrid[newx, newy].Px = u.Ox;
							dgrid[newx, newy].Py = u.Oy;
						}
					}
				}
			}

			int min = q.Min(y => y.D);
			u = q.Where(x => x.D == min).First();
			q.Remove(u);
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
