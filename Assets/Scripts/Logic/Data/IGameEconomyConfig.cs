namespace Logic.Data {

public interface IGameEconomyConfig {
	public int StartingBalance { get; }
	public int RoundBasePay { get; }
	public int NewUnitsDestroyedPay { get; }
	public int TotalUnitsPurchasedPay { get; }
}

}
