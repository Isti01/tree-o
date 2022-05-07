using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Data.World {
/// <summary>
/// Represents the navigation system of the world.
/// </summary>
internal class WorldNavigation {
	private readonly TileObject[,] _grid;

	/// <summary>
	/// Creates a navigation system.
	/// </summary>
	/// <param name="grid">The worldgrid.</param>
	public WorldNavigation(TileObject[,] grid) {
		_grid = grid;
	}

	/// <summary>
	/// Checks if there's a path between two tiles.
	/// </summary>
	/// <param name="from">First tile's position.</param>
	/// <param name="to">Second tile's position.</param>
	/// <param name="blockedTiles">The tiles that are blocked.</param>
	/// <returns>True if a path exists.</returns>
	public bool IsPositionReachable(TilePosition from, TilePosition to, ISet<TilePosition> blockedTiles) {
		return GetReachablePositionSubset(from, new HashSet<TilePosition> {to}, blockedTiles).Any();
	}

	/// <summary>
	/// Checks if there's a path between two tiles.
	/// </summary>
	/// <param name="from">First tile's position.</param>
	/// <param name="to">Second tile's position.</param>
	/// <returns>True if a path exists.</returns>
	public bool IsPositionReachable(TilePosition from, TilePosition to) {
		return GetReachablePositionSubset(from, new HashSet<TilePosition> {to}).Any();
	}

	/// <summary>
	/// Finds all the reachable TilePosition from a TilePosition.
	/// </summary>
	/// <param name="from">The start of the pathfinding.</param>
	/// <param name="to">Set containing the destination for the pathfinding. </param>
	/// <param name="blockedTiles">The tiles that are blocked. </param>
	/// <returns>The set of reachable positions.</returns>
	/// <exception cref="ArgumentException">If 'to' is a blocked position.</exception>
	public ISet<TilePosition> GetReachablePositionSubset(TilePosition from, ISet<TilePosition> to,
		ISet<TilePosition> blockedTiles) {
		if (blockedTiles.Overlaps(to))
			throw new ArgumentException("Positions can't be blocked and be 'to' positions at the same time");

		//We modify the grid. We must be careful not to call any "foreign" code here:
		// outside observers mustn't see the grid in this modified state.

		foreach (TilePosition tile in blockedTiles) {
			_grid[tile.X, tile.Y] ??= new FillerTileObject(tile);
		}

		ISet<TilePosition> result = GetReachablePositionSubset(from, to);

		foreach (TilePosition tile in blockedTiles) {
			if (_grid[tile.X, tile.Y] is FillerTileObject) {
				_grid[tile.X, tile.Y] = null;
			}
		}

		return result;
	}

	/// <summary>
	/// Finds all the reachable TilePosition from a TilePosition.
	/// </summary>
	/// <param name="from">The start of the pathfinding.</param>
	/// <param name="to">Set containing the destination for the pathfinding. </param>
	/// <returns>The set of reachable positions.</returns>
	public ISet<TilePosition> GetReachablePositionSubset(TilePosition from, ISet<TilePosition> to) {
		Dijkstra[,] dGrid = RunDijkstra(from.X, from.Y, to);
		HashSet<TilePosition> result = new HashSet<TilePosition>(to);
		result.RemoveWhere(pos => dGrid[pos.X, pos.Y].D == int.MaxValue);
		return result;
	}

	//Hide suggestion: we want a set in order to ensure optimal performance
	// ReSharper disable once SuggestBaseTypeForParameter
	private Dijkstra[,] RunDijkstra(int fromX, int fromY, ISet<TilePosition> to) {
		int width = _grid.GetLength(0);
		int height = _grid.GetLength(1);

		Dijkstra[,] dGrid = new Dijkstra[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				dGrid[i, j] = new Dijkstra(i, j);
			}
		}

		dGrid[fromX, fromY].D = 0;
		IList<Dijkstra> queue = new List<Dijkstra>(width * height);
		queue.Add(dGrid[fromX, fromY]);
		dGrid[fromX, fromY].Queued = true;

		int[] diffX = {0, 0, -1, 1};
		int[] diffY = {1, -1, 0, 0};

		//Instead of removing from the queue, advance an index: this is faster.
		//The first element always has the least weight due to all edges having the same cost (1).
		int queueIndex = 0;
		while (queue.Count > queueIndex) {
			Dijkstra u = queue[queueIndex++];

			if (u.D == int.MaxValue) break;

			//'to' positions have finite cost but can be blocked
			if ((u.Ox == fromX && u.Oy == fromY) || _grid[u.Ox, u.Oy] == null) {
				for (int i = 0; i < 4; i++) {
					int newX = u.Ox + diffX[i];
					int newY = u.Oy + diffY[i];
					if (newX < 0 || newY < 0 || newX >= width || newY >= height) continue;

					Dijkstra newU = dGrid[newX, newY];
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

		return dGrid;
	}

	/// <summary>
	/// Calculates the vectors from tile to tile in the path between two positions.
	/// </summary>
	/// <param name="from">The start if the path.</param>
	/// <param name="to">The end of the path.</param>
	/// <param name="colliderRadius">The colliderRadius of the object that travels on the path.</param>
	/// <returns>The pathDelta vectors</returns>
	public List<Vector2> TryGetPathDeltas(Vector2 from, Vector2 to, float colliderRadius) {
		if (from.ToTilePosition().Equals(to.ToTilePosition())) return new List<Vector2>();

		Dijkstra[,] dGrid = RunDijkstra((int) from.X, (int) from.Y,
			new HashSet<TilePosition> {to.ToTilePosition()});
		if (dGrid[(int) to.X, (int) to.Y].D == int.MaxValue) return null;

		List<Vector2> pathDeltas = new List<Vector2>();

		Dijkstra target = dGrid[(int) to.X, (int) to.Y];
		Dijkstra prev = dGrid[target.Px, target.Py];
		while (!(prev.Ox == (int) from.X && prev.Oy == (int) from.Y)) {
			pathDeltas.Add(new Vector2(target.Ox - prev.Ox, target.Oy - prev.Oy));
			target = prev;
			prev = dGrid[prev.Px, prev.Py];
		}

		pathDeltas.Add(new Vector2(target.Ox + 0.5F - from.X, target.Oy + 0.5F - from.Y));

		pathDeltas.Reverse();
		return pathDeltas;
	}

	private class FillerTileObject : TileObject {
		public FillerTileObject(TilePosition position) : base(null, position) {}
	}

	private class Dijkstra {
		public int D { get; set; }
		public int Ox { get; }
		public int Oy { get; }
		public int Px { get; set; }
		public int Py { get; set; }
		public bool Queued { get; set; }

		public Dijkstra(int x, int y) {
			D = int.MaxValue;
			Ox = x;
			Oy = y;
			Px = -1;
			Py = -1;
		}
	}
}
}
