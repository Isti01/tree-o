using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Data.World;
using Logic.Event.Team;

namespace Logic.Data {
public class GameTeam {
	#region Fields

	private HashSet<TilePosition> _availableTowerPositionsCache;
	private readonly Dictionary<IUnitTypeData, int> _deployedUnitTypeCounts = new Dictionary<IUnitTypeData, int>();

	#endregion

	#region Properties

	public IGameOverview Overview { get; }

	public Color TeamColor { get; }

	public IReadOnlyList<Barrack> Barracks { get; }

	public Castle Castle { get; }

	public IEnumerable<Tower> Towers => Overview.World.GetTileObjectsOfType<Tower>()
		.Where(t => t.OwnerColor == TeamColor);

	public IEnumerable<Unit> Units => Overview.World.Units
		.Where(t => t.Owner == this);

	public ISet<TilePosition> AvailableTowerPositions {
		get {
			if (_availableTowerPositionsCache == null) RecalculateAvailableTowerPositions();
			return new HashSet<TilePosition>(_availableTowerPositionsCache);
		}
	}

	public int Money { get; private set; }

	public int PresentTowerCount => Towers.Count();

	public int MoneySpent { get; private set; }

	public int PurchasedUnitCount { get; private set; }

	public int BuiltTowerCount { get; private set; }
	public int AliveUnits => Units.Count();

	#endregion

	#region Methods

	internal GameTeam(IGameOverview overview, Color color, Castle castle, IEnumerable<Barrack> barracks) {
		Overview = overview;
		TeamColor = color;
		Castle = castle;
		Barracks = barracks.OrderBy(barrack => barrack.Ordinal).ToList();
		Money = overview.EconomyConfig.StartingBalance;
	}

	internal void SpendMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot spend non-positive amount {amount}");
		if (amount > Money) throw new ArgumentException($"Cannot spend {amount} when balance is {Money}");

		int oldMoney = Money;
		Money -= amount;
		MoneySpent += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	internal void GiveMoney(int amount) {
		if (amount <= 0) throw new ArgumentException($"Cannot give non-positive amount {amount}");

		int oldMoney = Money;
		Money += amount;
		Overview.Events.Raise(new TeamMoneyUpdatedEvent(this, oldMoney));
	}

	public int GetDeployedUnitTypeCount(IUnitTypeData unitTypeData) {
		return _deployedUnitTypeCounts.TryGetValue(unitTypeData, out int count) ? count : 0;
	}

	internal void IncrementPurchasedUnitCount(IUnitTypeData unitTypeData) {
		int newCount = 1;
		if (_deployedUnitTypeCounts.TryGetValue(unitTypeData, out int count)) {
			newCount += count;
		}

		_deployedUnitTypeCounts[unitTypeData] = newCount;
		PurchasedUnitCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	internal void IncrementBuiltTowerCount() {
		BuiltTowerCount++;
		Overview.Events.Raise(new TeamStatisticsUpdatedEvent(this));
	}

	internal void InvalidateCachedAvailableTowerPositions() {
		_availableTowerPositionsCache = null;
	}

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
