using Logic.Data;

namespace Logic.Command {

public class AdvancePhaseCommand : BaseCommand {
	public IGameOverview Overview { get; }

	public AdvancePhaseCommand(IGameOverview overview) {
		Overview = overview;
	}
}

}
