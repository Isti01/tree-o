namespace Logic.Data.World {
public interface ITowerTypeData {
	#region Properties

	/// <summary>
	/// Name of the tower type.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Damage of the tower type.
	/// </summary>
	public float Damage { get; }

	/// <summary>
	/// Range of the tower type.
	/// </summary>
	public float Range { get; }

	/// <summary>
	/// Cooldown time of the tower type between two shots.
	/// </summary>
	public float CooldownTime { get; }

	/// <summary>
	/// Building cost of the tower type.
	/// </summary>
	public int BuildingCost { get; }

	/// <summary>
	/// Amount of money refunded after destroying this type of tower.
	/// </summary>
	public int DestroyRefund { get; }

	/// <summary>
	/// Building cost of the tower type.
	/// </summary>
	public int UpgradeCost { get; }

	/// <summary>
	/// Type of the new tower after upgrade.
	/// </summary>
	public ITowerTypeData AfterUpgradeType { get; }

	#endregion
}
}
