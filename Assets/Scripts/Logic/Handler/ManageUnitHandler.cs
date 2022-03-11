using System;
using Logic.Command;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;

namespace Logic.Handler {

public class ManageUnitHandler : BaseHandler {
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
		command.Team.IncrementPurchasedUnitCount();
		barrack.QueueUnit(command.Type);
		return BiCommandResult.Success;
	}
}

}
