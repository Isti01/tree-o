using Logic.Data;
using Logic.Event;

namespace Logic.System {

/// <summary>
/// System responsible for initializing the next round
/// (e.g. giving money to teams) when it starts.
/// </summary>
internal class StartNextRoundSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<PhaseAdvancedEvent>(OnNextRound);
	}

	private void OnNextRound(PhaseAdvancedEvent e) {
		IGameOverview overview = e.Overview;
		if (e.OldPhase != GamePhase.Fight || overview.CurrentPhase != GamePhase.Prepare) return;

		foreach (GameTeam team in overview.Teams) {
			team.GiveMoney(overview.EconomyConfig.RoundBasePay
				+ team.PurchasedUnitCount * overview.EconomyConfig.TotalUnitsPurchasedPay);
		}
	}
}

}
