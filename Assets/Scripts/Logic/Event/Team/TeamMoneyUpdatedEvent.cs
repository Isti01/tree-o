using Logic.Data;

namespace Logic.Event.Team {

/// <summary>
/// Raised when a team's <see cref="Data.GameTeam.Money"/> amount changes.
/// </summary>
public class TeamMoneyUpdatedEvent : BaseEvent, ITeamEvent {
	public GameTeam Team { get; }
	public int OldMoney { get; }

	public TeamMoneyUpdatedEvent(GameTeam team, int oldMoney) {
		Team = team;
		OldMoney = oldMoney;
	}
}

}
