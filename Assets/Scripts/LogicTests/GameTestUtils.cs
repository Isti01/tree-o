using System;
using Logic.Data;
using Logic.Data.World;

namespace LogicTests {

/// <summary>
/// Utility methods regarding the whole game (<see cref="GameOverview"/>).
/// </summary>
public static class GameTestUtils {
	/// <summary>
	/// Shorthand for the method with the same name.
	/// This overload "ignores" the <see cref="Action"/> parameter.
	/// </summary>
	/// <returns>a newly created overview instance</returns>
	public static GameOverview CreateOverview() {
		return CreateOverview(((config, economy, world) => {}));
	}

	/// <summary>
	/// Creates a new <see cref="GameOverview"/> without any mocking.
	/// This method can be used when the whole game needs to be tested (and not just a particular unit).
	/// </summary>
	/// <param name="configInitializer">callback that initializes the game's configuration</param>
	/// <returns>a newly created overview instance</returns>
	public static GameOverview CreateOverview(Action<GameOverviewConfig,
		GameEconomyConfig, GameWorldConfig> configInitializer) {
		GameOverviewConfig overviewConfig = new GameOverviewConfig();
		GameEconomyConfig economyConfig = new GameEconomyConfig();
		GameWorldConfig worldConfig = new GameWorldConfig();

		Random random = RandomUtils.CreateRandomlySeededRandom();
		int rngSeed = random.Next();
		worldConfig.Width = random.Next(10, 20);
		worldConfig.Height = random.Next(10, 20);

		configInitializer(overviewConfig, economyConfig, worldConfig);

		Action<Exception> errorHandler = e => throw e;
		return new GameOverview(errorHandler, rngSeed, overviewConfig, economyConfig, worldConfig);
	}

	/// <summary>
	/// Implementation of <see cref="IGameOverviewConfig"/> for testing.
	/// Sensible default values have been chosen, but they may need to adjusted for specific tests.
	/// While the values have public setters, they should only be used prior to passing the
	/// instance to the logic component: modifying values while the game is running might break stuff.
	/// </summary>
	public class GameOverviewConfig : IGameOverviewConfig {
		public float FightingPhaseDuration { get; set; } = 10;
	}

	/// <summary>
	/// Implementation of <see cref="IGameEconomyConfig"/> for testing.
	/// Sensible default values have been chosen, but they may need to adjusted for specific tests.
	/// While the values have public setters, they should only be used prior to passing the
	/// instance to the logic component: modifying values while the game is running might break stuff.
	/// </summary>
	public class GameEconomyConfig : IGameEconomyConfig {
		public int StartingBalance { get; set; } = 10;
		public int RoundBasePay { get; set; } = 10;
		public int NewUnitsKilledPay { get; set; } = 10;
		public int TotalUnitsPurchasedPay { get; set; } = 10;
	}

	/// <summary>
	/// Implementation of <see cref="IGameWorldConfig"/> for testing.
	/// Sensible default values have been chosen, but they may need to adjusted for specific tests.
	/// While the values have public setters, they should only be used prior to passing the
	/// instance to the logic component: modifying values while the game is running might break stuff.
	/// </summary>
	public class GameWorldConfig : IGameWorldConfig {
		public int Width { get; set; } = 10;
		public int Height { get; set; } = 10;
		public float BarrackSpawnCooldownTime { get; set; } = 1;
		public float CastleStartingHealth { get; set; } = 10;
		public int MaxBuildingDistance { get; set; } = 5;
	}

	/// <summary>
	/// Implementation of <see cref="IUnitTypeData"/> for testing.
	/// Sensible default values have been chosen, but they may need to adjusted for specific tests.
	/// While the values have public setters, they should only be used prior to passing the
	/// instance to the logic component: modifying values while the game is running might break stuff.
	/// </summary>
	public class UnitTypeData : IUnitTypeData {
		public string Name { get; set; } = "Test Unit";
		public float Health { get; set; } = 10;
		public float Damage { get; set; } = 1;
		public float Speed { get; set; } = 1;
		public int Cost { get; set; } = 1;
	}

	/// <summary>
	/// Implementation of <see cref="ITowerTypeData"/> for testing.
	/// Sensible default values have been chosen, but they may need to adjusted for specific tests.
	/// While the values have public setters, they should only be used prior to passing the
	/// instance to the logic component: modifying values while the game is running might break stuff.
	/// </summary>
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
