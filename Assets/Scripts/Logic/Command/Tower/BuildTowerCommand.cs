﻿using Logic.Data;
using Logic.Data.World;

namespace Logic.Command.Tower {

public class BuildTowerCommand : BaseCommand<BuildTowerCommand.CommandResult> {
	public GameTeam Team { get; }
	public ITowerTypeData Type { get; }
	public TilePosition Position { get; }

	public BuildTowerCommand(GameTeam team, ITowerTypeData type, TilePosition position) {
		Team = team;
		Type = type;
		Position = position;
	}

	public sealed class CommandResult : AbstractCommandResult {
		public static readonly CommandResult Success = new CommandResult("Success");
		public static readonly CommandResult NotEnoughMoney = new CommandResult("NotEnoughMoney");
		public static readonly CommandResult TileUnavailable = new CommandResult("TileUnavailable");
		public static readonly CommandResult MiscFailure = new CommandResult("MiscFailure");

		public override bool IsSuccess => this == Success;

		private CommandResult(string name) : base(name) {}
	}
}

}
