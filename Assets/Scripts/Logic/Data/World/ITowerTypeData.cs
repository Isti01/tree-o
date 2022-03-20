namespace Logic.Data.World {
public interface ITowerTypeData {

	#region Properties

	public string Name { get; }
	public float Damage { get; }
	public float Range { get; }
	public float CooldownTime { get; }
	public int BuildingCost { get; }
	public int DestroyRefund { get; }
	public int UpgradeCost { get; }

	#endregion
}
}
