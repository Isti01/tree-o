using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Data.World;
using Logic.Event.Team;

namespace Logic.Data {
public class GameTeam {

	#region Properties

	public IGameOverview Overview { get; }

	public Color TeamColor { get; }

	public IReadOnlyList<Barrack> Barracks { get; }

	public Castle Castle { get; }

	public int Money { get; private set; } = 500; //TODO don't use a hardcoded value

	public int PresentTowerCount => Overview.World.GetTileObjectsOfType<Tower>()
		.Count(tower => tower.OwnerColor == TeamColor);

	public int MoneySpent { get; private set; }

	public int PurchasedUnitCount { get; private set; }

	public int BuiltTowerCount { get; private set; }


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

	public void GiveMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot give non-positive amount {amount}");

		int oldMoney = Money;
		Money += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	public void IncrementPurchasedUnitCount() {
		PurchasedUnitCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	public void IncrementBuiltTowerCount() {
		BuiltTowerCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	#endregion
}
}
