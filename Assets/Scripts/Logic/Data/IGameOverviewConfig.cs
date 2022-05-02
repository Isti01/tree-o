namespace Logic.Data {

/// <summary>
/// Container of configuration entries related to the <see cref="IGameOverview"/> class.
/// </summary>
public interface IGameOverviewConfig {
	/// <summary>
	/// The preferred duration of the <see cref="GamePhase.Fight"/>.
	/// This value can be overridden in special circumstances
	/// (e.g. this isn't enough to spawn all the units from the barracks)
	/// </summary>
	public float FightingPhaseDuration { get; }
}

}
