using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Unit {

public class PurchaseUnitCommand : BaseCommand {
	public GameTeam Team { get; }
	public IUnitTypeData Type { get; }

	public PurchaseUnitCommand(GameTeam team, IUnitTypeData type) {
		Team = team;
		Type = type;
	}
}

}
