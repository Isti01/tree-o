namespace Logic.Data {

public interface IGameEconomyConfig {
	public int StartingBalance { get; }
	public int RoundBasePay { get; }
	public int NewUnitsKilledPay { get; }
	public int TotalUnitsPurchasedPay { get; }
}

}
