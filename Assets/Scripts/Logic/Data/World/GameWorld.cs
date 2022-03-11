using System;
using System.Collections.Generic;

namespace Logic.Data.World {
public class GameWorld {

	#region Fields

	private TileObject[,] _grid;

	#endregion

	#region Properties

	public GameOverview Overview { get; }

	public int Width { get; }

	public int Height { get; }

	public TileObject this[int x, int y] => GetTile(x, y);

	public IReadOnlyCollection<TileObject> TileObjects { get; }

	public IReadOnlyCollection<Unit> Units { get; }

	#endregion

	#region Methods

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
}
}
