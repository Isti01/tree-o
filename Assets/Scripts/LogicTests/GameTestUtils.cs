using System;
using Logic.Data;
using Logic.Data.World;

namespace LogicTests {

public static class GameTestUtils {
	public static GameOverview CreateOverview() {
		return CreateOverview(((config, economy, world) => {}));
	}

	public static GameOverview CreateOverview(Action<GameOverviewConfig,
		GameEconomyConfig, GameWorldConfig> configInitializer) {
		Random random = RandomUtils.CreateRandomlySeededRandom();
		Action<Exception> errorHandler = e => throw e;
		int seed = random.Next();
		int width = random.Next(10, 20);
		int height = random.Next(10, 20);
		GameOverviewConfig overviewConfig = new GameOverviewConfig();
		GameEconomyConfig economyConfig = new GameEconomyConfig();
		GameWorldConfig worldConfig = new GameWorldConfig();
		configInitializer(overviewConfig, economyConfig, worldConfig);
		return new GameOverview(errorHandler, seed, width, height,
			overviewConfig, economyConfig, worldConfig);
	}

	public class GameOverviewConfig : IGameOverviewConfig {
		public float FightingPhaseDuration { get; set; } = 10;
	}

	public class GameEconomyConfig : IGameEconomyConfig {
		public int StartingBalance { get; set; } = 10;
		public int RoundBasePay { get; set; } = 10;
		public int NewUnitsKilledPay { get; set; } = 10;
		public int TotalUnitsPurchasedPay { get; set; } = 10;
	}

	public class GameWorldConfig : IGameWorldConfig {
		public float BarrackSpawnCooldownTime { get; set; } = 1;
		public float CastleStartingHealth { get; set; } = 10;
		public int MaxBuildingDistance { get; set; } = 5;
	}

	public class UnitTypeData : IUnitTypeData {
		public string Name { get; set; } = "Test Unit";
		public float Health { get; set; } = 10;
		public float Damage { get; set; } = 1;
		public float Speed { get; set; } = 1;
		public int Cost { get; set; } = 1;
	}

	public class TowerTypeData : ITowerTypeData {
		public string Name { get; set; } = "Test Tower";
		public float Damage { get; set; } = 1;
		public float Range { get; set; } = 3;
		public float CooldownTime { get; set; } = 1;
		public int BuildingCost { get; set; } = 1;
		public int DestroyRefund { get; set; } = 1;
		public int UpgradeCost { get; set; } = 1;
		public ITowerTypeData AfterUpgradeType { get; set; } = null;
	}
}

}
