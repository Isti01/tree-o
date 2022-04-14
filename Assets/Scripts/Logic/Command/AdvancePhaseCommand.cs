using Logic.Data;

namespace Logic.Command {

/// <summary>
/// Command that explicitly advances <see cref="IGameOverview.CurrentPhase"/> if possible.
/// </summary>
public class AdvancePhaseCommand : BaseCommand {
	public IGameOverview Overview { get; }

	public AdvancePhaseCommand(IGameOverview overview) {
		Overview = overview;
	}
}

}
