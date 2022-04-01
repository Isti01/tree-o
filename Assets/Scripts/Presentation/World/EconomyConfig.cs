using Logic.Data;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New Economy Config", menuName = "World/Config/Economy Config", order = 1)]
public class EconomyConfig : ScriptableObject, IGameEconomyConfig {
	public int startingBalance;
	public int roundBasePay;
	public int newUnitsKilledPay;
	public int totalUnitsPurchasedPay;

	public int StartingBalance => startingBalance;
	public int RoundBasePay => roundBasePay;
	public int NewUnitsKilledPay => newUnitsKilledPay;
	public int TotalUnitsPurchasedPay => totalUnitsPurchasedPay;
}

}
