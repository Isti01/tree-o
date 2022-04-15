using Logic.Command;
using Logic.Data;

namespace Logic.Handler {

/// <summary>
/// Handler of <see cref="AdvancePhaseCommand"/>.
/// </summary>
internal class AdvancePhaseHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<AdvancePhaseCommand, BiCommandResult>(On);
	}

	private BiCommandResult On(AdvancePhaseCommand command) {
		GameOverview overview = (GameOverview) command.Overview;
		if (overview.CurrentPhase == GamePhase.Finished) {
			return BiCommandResult.Failure;
		}

		overview.AdvancePhase();
		return BiCommandResult.Success;
	}
}

}
