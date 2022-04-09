using Logic.Data;
using Logic.Event;
using Logic.Event.Team;
using Logic.Event.World.Castle;
using Logic.Event.World.Unit;

namespace Logic.System {

public class TeamStatisticsSystem : BaseSystem {
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
