using Logic.Data;

namespace Logic.Command {

public class AdvancePhaseCommand : BaseCommand {
	public GameOverview Overview { get; }

	public AdvancePhaseCommand(GameOverview overview) {
		Overview = overview;
	}
}

}
