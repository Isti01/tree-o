using System;
using System.Collections.Generic;

namespace Logic.Data.World {

public class WorldNavigation {
	private readonly TileObject[,] _grid;

	public WorldNavigation(TileObject[,] grid) {
		_grid = grid;
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
}

}
