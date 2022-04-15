using Logic.Data;

namespace Logic.Event.Team {

/// <summary>
/// Raised when any statistics regarding a team (e.g. <see cref="GameTeam.MoneySpent"/>) change.
/// </summary>
public class TeamStatisticsUpdatedEvent : BaseEvent, ITeamEvent {
	public GameTeam Team { get; }

	public TeamStatisticsUpdatedEvent(GameTeam team) {
		Team = team;
	}
}

}
