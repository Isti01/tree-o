using System;
using System.Collections.Generic;
using Logic.Data.World;
using Logic.Event.Team;

namespace Logic.Data {
public class GameTeam {

	#region Properties

	public IGameOverview Overview { get; }

	public Color TeamColor { get; }

	public IReadOnlyList<Barrack> Barracks { get; }

	public Castle Castle { get; }

	public int Money { get; private set; }

	public int PresentTowerCount { get; }

	public int MoneySpent { get; private set; }

	public int PurchasedUnitCount { get; private set; }

	public int BuiltTowerCount { get; }


	#endregion

	#region Methods

	public GameTeam(IGameOverview overview, Color color, Castle castle, IEnumerable<Barrack> barracks) {
		Overview = overview;
		TeamColor = color;
		Castle = castle;
		Barracks = new List<Barrack>(barracks);
	}

	public void SpendMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot spend non-positive amount {amount}");
		if (amount > Money) throw new ArgumentException($"Cannot spend {amount} when balance is {Money}");

		int oldMoney = Money;
		Money -= amount;
		MoneySpent += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	public void IncrementPurchasedUnitCount() {
		PurchasedUnitCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	#endregion
}
}
