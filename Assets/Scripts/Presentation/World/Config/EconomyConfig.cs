using Logic.Data;
using UnityEngine;

namespace Presentation.World.Config {
/// <summary>
///     Enables the economy related settings to be configured in the Unity Editor
/// </summary>
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
	private int newUnitsDestroyedPay;

	[SerializeField]
	[Min(0)]
	private int totalUnitsPurchasedPay;

	public int StartingBalance => startingBalance;
	public int RoundBasePay => roundBasePay;
	public int NewUnitsDestroyedPay => newUnitsDestroyedPay;
	public int TotalUnitsPurchasedPay => totalUnitsPurchasedPay;
}
}
