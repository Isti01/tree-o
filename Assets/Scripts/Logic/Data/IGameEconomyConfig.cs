using Logic.Data.World;

namespace Logic.Data {

/// <summary>
/// Container of configuration entries related to the <see cref="IGameOverview"/> class.
/// </summary>
public interface IGameEconomyConfig {
	/// <summary>
	/// Initial value of <see cref="GameTeam.Money"/>.
	/// </summary>
	public int StartingBalance { get; }

	/// <summary>
	/// Amount of money each <see cref="GameTeam"/> receives at the end of each round.
	/// </summary>
	public int RoundBasePay { get; }

	/// <summary>
	/// Amount of money a <see cref="GameTeam"/> receives when an enemy <see cref="Unit"/> is destroyed.
	/// </summary>
	public int NewUnitsDestroyedPay { get; }

	/// <summary>
	/// Amount of money a <see cref="GameTeam"/> receives at the end of each round.
	/// This value gets multiplied by <see cref="GameTeam.PurchasedUnitCount"/> and that amount is given.
	/// </summary>
	public int TotalUnitsPurchasedPay { get; }
}

}
