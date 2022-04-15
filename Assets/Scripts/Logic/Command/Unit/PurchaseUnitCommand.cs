using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Unit {

/// <summary>
/// Command for queuing a <see cref="Unit"/> in a <see cref="Barrack"/>
/// during the <see cref="GamePhase.Prepare"/> phase.
/// Fails if not the team's money (<see cref="GameTeam.Money"/>)
/// cannot cover the unit's cost (<see cref="IUnitTypeData.Cost"/>).
/// </summary>
public class PurchaseUnitCommand : BaseCommand {
	public GameTeam Team { get; }
	public IUnitTypeData Type { get; }

	public PurchaseUnitCommand(GameTeam team, IUnitTypeData type) {
		Team = team;
		Type = type;
	}
}

}
