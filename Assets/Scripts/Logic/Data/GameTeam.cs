using System.Collections.Generic;
using Logic.Data.World;

namespace Logic.Data {
public class GameTeam {

	#region Properties

	public Color TeamColor { get; }

	public IReadOnlyCollection<Barrack> Barracks { get; }

	public Castle Castle { get; }

	public int Money { get; }

	public int AliveUnitCount { get; }

	public int PresentTowerCount { get; }

	public int MoneySpent { get; }

	public int PurchasedUnitCount { get; }

	public int BuiltTowerCount { get; }


	#endregion
}
}
