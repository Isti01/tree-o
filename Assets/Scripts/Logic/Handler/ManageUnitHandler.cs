using System;
using Logic.Command;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

/// <summary>
/// Handler of <see cref="Unit"/> related commands: <see cref="PurchaseUnitCommand"/>.
/// </summary>
internal class ManageUnitHandler : BaseHandler {
	public override void RegisterConsumers(CommandDispatcher dispatcher) {
		dispatcher.RegisterConsumer<PurchaseUnitCommand, BiCommandResult>(Handle);
	}

	private BiCommandResult Handle(PurchaseUnitCommand command) {
		if (command.Team.Overview.CurrentPhase != GamePhase.Prepare) {
			return BiCommandResult.Failure;
		}

		if (command.Team.Money < command.Type.Cost) {
			return BiCommandResult.Failure;
		}

		Random random = command.Team.Overview.Random;
		Barrack barrack = command.Team.Barracks[random.Next(command.Team.Barracks.Count)];

		command.Team.SpendMoney(command.Type.Cost);
		command.Team.IncrementPurchasedUnitCount(command.Type);
		barrack.QueueUnit(command.Type);
		return BiCommandResult.Success;
	}
}

}
