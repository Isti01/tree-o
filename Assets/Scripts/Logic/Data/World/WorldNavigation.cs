using System;
using System.Collections.Generic;

namespace Logic.Data.World {

public class WorldNavigation {
	private readonly TileObject[,] _grid;

	public WorldNavigation(TileObject[,] grid) {
		_grid = grid;
	}

	public bool IsPositionReachable(TilePosition from, TilePosition to,
		ICollection<TilePosition> blockedTiles) {
		return IsPositionReachable(from.ToVectorCentered(), to.ToVectorCentered(), blockedTiles);
	}

	public bool IsPositionReachable(Vector2 from, Vector2 to, ICollection<TilePosition> blockedTiles) {
		//We modify the grid. We must be careful not to call any "foreign" code here:
		// outside observers mustn't see the grid in this modified state.

		foreach (TilePosition tile in blockedTiles) {
			if (_grid[tile.X, tile.Y] == null) {
				_grid[tile.X, tile.Y] = new FillerTileObject(tile);
			}
		}

		bool result = IsPositionReachable(from, to);

		foreach (TilePosition tile in blockedTiles) {
			if (_grid[tile.X, tile.Y] is FillerTileObject) {
				_grid[tile.X, tile.Y] = null;
			}
		}

		return result;
	}

	public bool IsPositionReachable(TilePosition from, TilePosition to) {
		return IsPositionReachable(from.ToVectorCentered(), to.ToVectorCentered());
	}

	public bool IsPositionReachable(Vector2 from, Vector2 to) {
		return true; //TODO implement
	}

	public IEnumerable<Vector2> TryGetPathDeltas(Vector2 from, Vector2 to, float collider) {
		throw new NotImplementedException();
	}

	private class FillerTileObject : TileObject {
		public FillerTileObject(TilePosition position) : base(null, position) {}
	}
}

}
