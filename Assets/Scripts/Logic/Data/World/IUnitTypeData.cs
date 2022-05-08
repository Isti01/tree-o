namespace Logic.Data.World {
/// <summary>
/// Interface for the different types of <see cref="Unit"/>s
/// </summary>
public interface IUnitTypeData {
	#region Properties

	/// <summary>
	/// Name of the unit type;
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Health of the unit type;
	/// </summary>
	public float Health { get; }

	/// <summary>
	/// Damage of the unit type;
	/// </summary>
	public float Damage { get; }

	/// <summary>
	/// Speed of the unit type;
	/// </summary>
	public float Speed { get; }

	/// <summary>
	/// Cost of the unit type;
	/// </summary>
	public int Cost { get; }

	#endregion
}
}
