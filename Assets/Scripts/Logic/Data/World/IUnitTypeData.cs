namespace Logic.Data.World {
public interface IUnitTypeData {

	#region Properties

	public string Name { get; }
	public float Health { get; }
	public float Damage { get; }
	public float Speed { get; }
	public int Cost { get; }

	#endregion
}
}
