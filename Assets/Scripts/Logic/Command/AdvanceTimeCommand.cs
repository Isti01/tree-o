using Logic.Data;

namespace Logic.Command {

public class AdvanceTimeCommand : BaseCommand {
	public IGameOverview Overview { get; }
	public float DeltaTime { get; }

	public AdvanceTimeCommand(IGameOverview overview, float deltaTime) {
		Overview = overview;
		DeltaTime = deltaTime;
	}
}

}
