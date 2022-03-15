using Logic.Data;

namespace Logic.Command {

public class AdvanceTimeCommand : BaseCommand {
	public IGameOverview Game { get; }
	public float DeltaTime { get; }

	public AdvanceTimeCommand(IGameOverview game, float deltaTime) {
		Game = game;
		DeltaTime = deltaTime;
	}
}

}
