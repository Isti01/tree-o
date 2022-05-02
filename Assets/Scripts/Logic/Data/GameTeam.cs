using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Data.World;
using Logic.Event.Team;

namespace Logic.Data {

/// <summary>
/// Represents a player in the game.
/// <see cref="Building"/>, <see cref="Unit"/> instances belong to players and they have money to manage.
/// This class also contains some statistics related to them.
/// </summary>
public class GameTeam {
	#region Fields

	private HashSet<TilePosition> _availableTowerPositionsCache;
	private readonly Dictionary<IUnitTypeData, int> _deployedUnitTypeCounts = new Dictionary<IUnitTypeData, int>();

	#endregion

	#region Properties

	/// <summary>
	/// The game instance this team belongs to.
	/// </summary>
	public IGameOverview Overview { get; }

	/// <summary>
	/// The color used to identify this team within the game.
	/// </summary>
	public Color TeamColor { get; }

	/// <summary>
	/// The (unique) barracks of this team.
	/// The list doesn't change and its size of always exactly 2.
	/// </summary>
	public IReadOnlyList<Barrack> Barracks { get; }

	/// <summary>
	/// The castle of this team.
	/// </summary>
	public Castle Castle { get; }

	/// <summary>
	/// The current (unique) towers of this team.
	/// </summary>
	public IEnumerable<Tower> Towers => Overview.World.GetTileObjectsOfType<Tower>()
		.Where(t => t.OwnerColor == TeamColor);

	/// <summary>
	/// The current (unique) units of this team.
	/// </summary>
	public IEnumerable<Unit> Units => Overview.World.Units
		.Where(t => t.Owner == this);

	/// <summary>
	/// The current (unique) positions where a tower could be placed.
	/// This value is either cached, or querying it is fast.
	/// Please keep in mind that building a tower (among other actions) changes the returned value.
	/// </summary>
	public ISet<TilePosition> AvailableTowerPositions {
		get {
			if (_availableTowerPositionsCache == null) RecalculateAvailableTowerPositions();
			return new HashSet<TilePosition>(_availableTowerPositionsCache);
		}
	}

	/// <summary>
	/// The current money of this team.
	/// </summary>
	public int Money { get; private set; }

	/// <summary>
	/// The total amount of money this team has spent.
	/// </summary>
	public int MoneySpent { get; private set; }

	/// <summary>
	/// The total amount of units this team has purchased.
	/// </summary>
	public int PurchasedUnitCount { get; private set; }

	/// <summary>
	/// The total amount of towers this team has built.
	/// Destroying towers doesn't decrement this value.
	/// </summary>
	public int BuiltTowerCount { get; private set; }

	#endregion

	#region Methods

	/// <summary>
	/// Creates and initializes a new instance. Should be called by <see cref="GameOverview"/>.
	/// </summary>
	/// <param name="overview">the game this team belongs to</param>
	/// <param name="color">the value for <see cref="TeamColor"/></param>
	/// <param name="castle">the value for <see cref="Castle"/></param>
	/// <param name="barracks">the value for <see cref="Barracks"/></param>
	internal GameTeam(IGameOverview overview, Color color, Castle castle, IEnumerable<Barrack> barracks) {
		Overview = overview;
		TeamColor = color;
		Castle = castle;
		Barracks = barracks.OrderBy(barrack => barrack.Ordinal).ToList();
		Money = overview.EconomyConfig.StartingBalance;
	}

	/// <summary>
	/// Decreases this team's <see cref="Money"/> and increases <see cref="MoneySpent"/>
	/// by the specified amount. Fails if not enough money is available.
	/// </summary>
	/// <param name="amount">the amount of money to spend</param>
	/// <exception cref="ArgumentException">if the specified amount is invalid
	/// or if this team doesn't have enough money</exception>
	internal void SpendMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot spend non-positive amount {amount}");
		if (amount > Money) throw new ArgumentException($"Cannot spend {amount} when balance is {Money}");

		int oldMoney = Money;
		Money -= amount;
		MoneySpent += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	/// <summary>
	/// Increases the value of <see cref="Money"/> by the specified amount.
	/// </summary>
	/// <param name="amount">the amount of money to give</param>
	/// <exception cref="ArgumentException">if the specified amount is invalid</exception>
	internal void GiveMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot give non-positive amount {amount}");

		int oldMoney = Money;
		Money += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	/// <summary>
	/// Gets the count of units from the specified type this team has purchased in total.
	/// </summary>
	/// <param name="unitTypeData">the unit type to query</param>
	/// <returns>the total amount of units of the specified type this team has purchased</returns>
	public int GetDeployedUnitTypeCount(IUnitTypeData unitTypeData) {
		return _deployedUnitTypeCounts.TryGetValue(unitTypeData, out int count) ? count : 0;
	}

	/// <summary>
	/// Increases the value returned by <see cref="GetDeployedUnitTypeCount"/> for the specified type.
	/// </summary>
	/// <param name="unitTypeData">the type whose counter to increment</param>
	internal void IncrementPurchasedUnitCount(IUnitTypeData unitTypeData) {
		int newCount = 1;
		if (_deployedUnitTypeCounts.TryGetValue(unitTypeData, out int count)) {
			newCount += count;
		}

		_deployedUnitTypeCounts[unitTypeData] = newCount;
		PurchasedUnitCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	/// <summary>
	/// Increments the value of <see cref="BuiltTowerCount"/>.
	/// </summary>
	internal void IncrementBuiltTowerCount() {
		BuiltTowerCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	/// <summary>
	/// Clears the <see cref="AvailableTowerPositions"/> cache.
	/// </summary>
	internal void InvalidateCachedAvailableTowerPositions() {
		_availableTowerPositionsCache = null;
	}

	/// <summary>
	/// Calculates the value returned by <see cref="AvailableTowerPositions"/>
	/// (bypassing the cache, meaning the cache isn't used, the value is recalculated).
	/// </summary>
	private void RecalculateAvailableTowerPositions() {
		GameWorld world = Overview.World;
		_availableTowerPositionsCache = new HashSet<TilePosition>();

		foreach (Building building in world.GetTileObjectsOfType<Building>()
			.Where(b => b.OwnerColor == TeamColor)) {
			int maxDelta = world.Config.MaxBuildingDistance;
			for (int dx = -maxDelta; dx <= maxDelta; dx++) {
				for (int dy = -maxDelta; dy <= maxDelta; dy++) {
					TilePosition pos = building.Position.Added(dx, dy);
					if (pos.X < 0 || pos.X >= world.Width || pos.Y < 0 || pos.Y >= world.Height) continue;
					if (Math.Abs(dx) + Math.Abs(dy) > maxDelta) continue;
					if (world[pos] != null) continue;
					_availableTowerPositionsCache.Add(pos);
				}
			}
		}

		_availableTowerPositionsCache.ExceptWith(world.Units.Select(u => u.TilePosition));

		_availableTowerPositionsCache.RemoveWhere(position => {
			ISet<TilePosition> blocked = new HashSet<TilePosition> {position};

			foreach (GameTeam sourceTeam in Overview.Teams) {
				TilePosition from = Overview.GetEnemyTeam(sourceTeam).Castle.Position;
				ISet<TilePosition> to = new HashSet<TilePosition>();
				to.UnionWith(sourceTeam.Barracks.Select(b => b.Position));
				to.UnionWith(sourceTeam.Units.Select(u => u.TilePosition));
				ISet<TilePosition> reachable = world.Navigation.GetReachablePositionSubset(from, to, blocked);
				if (reachable.Count != to.Count) return true;
			}

			return false;
		});
	}

	#endregion
}

}
