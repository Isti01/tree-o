using Logic.Data;

namespace Logic.Event.Team {

public class TeamMoneyUpdatedEvent : BaseEvent, ITeamEvent {
	public GameTeam Team { get; }
	public int OldMoney { get; }

	public TeamMoneyUpdatedEvent(GameTeam team, int oldMoney) {
		Team = team;
		OldMoney = oldMoney;
	}
}

}
