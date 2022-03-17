using Logic.Command;
using Logic.Data;

namespace Logic.Handler {

public class AdvancePhaseHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AdvancePhaseCommand, BiCommandResult>(On);
	}

	private BiCommandResult On(AdvancePhaseCommand command) {
		IGameOverview overview = command.Overview;
		if (overview.CurrentPhase == GamePhase.Finished) {
			return BiCommandResult.Failure;
		}

		overview.AdvancePhase();
		return BiCommandResult.Success;
	}
}

}
