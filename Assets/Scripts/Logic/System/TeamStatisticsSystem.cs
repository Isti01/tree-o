using Logic.Data;
using Logic.Event;
using Logic.Event.Team;
using Logic.Event.World.Castle;
using Logic.Event.World.Unit;

namespace Logic.System {

/// <summary>
/// System responsible for calling <see cref="TeamStatisticsUpdatedEvent"/> when a value changes
/// that's technically a <see cref="GameTeam"/> statistic, but isn't persisted as such.
/// </summary>
internal class TeamStatisticsSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<CastleDamagedEvent>(e => {
			On(e.Attacker.Owner);
			On(e.Castle.Owner);
		});
		dispatcher.AddListener<UnitDestroyedEvent>(e => On(e.Unit.Owner));
	}

	private void On(GameTeam team) {
		team.Overview.Events.Raise(new TeamStatisticsUpdatedEvent(team));
	}
}

}
