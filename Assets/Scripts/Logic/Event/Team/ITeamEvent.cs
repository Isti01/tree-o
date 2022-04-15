using Logic.Data;

namespace Logic.Event.Team {

/// <summary>
/// Base class for events that are about a <see cref="Data.GameTeam"/>.
/// </summary>
public interface ITeamEvent {
	public GameTeam Team { get; }
}

}
