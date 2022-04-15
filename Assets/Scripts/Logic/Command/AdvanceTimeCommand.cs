using Logic.Data;

namespace Logic.Command {

/// <summary>
/// Informs the logic component that the specified amount of time has passed
/// and that this time difference should be handled: the simulation should be advanced.
/// </summary>
public class AdvanceTimeCommand : BaseCommand {
	public IGameOverview Overview { get; }
	public float DeltaTime { get; }

	public AdvanceTimeCommand(IGameOverview overview, float deltaTime) {
		Overview = overview;
		DeltaTime = deltaTime;
	}
}

}
