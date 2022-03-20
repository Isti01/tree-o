namespace Logic.Command.Tower {

public class DestroyTowerCommand : BaseCommand {
	public Data.World.Tower Tower { get; }

	public DestroyTowerCommand(Data.World.Tower tower) {
		Tower = tower;
	}
}

}
