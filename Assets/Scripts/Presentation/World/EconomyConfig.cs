using Logic.Data;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New Economy Config", menuName = "World/Config/Economy Config", order = 1)]
public class EconomyConfig : ScriptableObject, IGameEconomyConfig {
	[SerializeField]
	[Min(0)]
	private int startingBalance;

	[SerializeField]
	[Min(0)]
	private int roundBasePay;

	[SerializeField]
	[Min(0)]
	private int newUnitsKilledPay;

	[SerializeField]
	[Min(0)]
	private int totalUnitsPurchasedPay;

	public int StartingBalance => startingBalance;
	public int RoundBasePay => roundBasePay;
	public int NewUnitsKilledPay => newUnitsKilledPay;
	public int TotalUnitsPurchasedPay => totalUnitsPurchasedPay;
}

}
