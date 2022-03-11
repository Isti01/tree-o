using Logic.Data;

namespace Logic.Event.Team {

public class TeamStatisticsUpdatedEvent : BaseEvent, ITeamEvent {
	public GameTeam Team { get; }

	public TeamStatisticsUpdatedEvent(GameTeam team) {
		Team = team;
	}
}

}
