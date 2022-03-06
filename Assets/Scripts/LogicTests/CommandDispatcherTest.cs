using System;
using Logic.Command;
using NUnit.Framework;

namespace Assets.Scripts.LogicTests {

public class CommandDispatcherTest {
	[Test]
	public void TestHandlingCommand() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		dispatcher.RegisterConsumer<SampleCommand, BiCommandResult>(_ => BiCommandResult.Success);
		BiCommandResult result = dispatcher.Issue(new SampleCommand());
		Assert.AreSame(result, BiCommandResult.Success);
	}

	[Test]
	public void TestRegisteringConsumerTwice() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		dispatcher.RegisterConsumer<SampleCommand, BiCommandResult>(_ => BiCommandResult.Success);
		Assert.Throws<CommandDispatcher.IllegalHandlerStateException>(() =>
			dispatcher.RegisterConsumer<SampleCommand, BiCommandResult>(_ => BiCommandResult.Success));
	}

	[Test]
	public void TestRemovingConsumerTwice() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		CommandDispatcher.Consumer<SampleCommand, BiCommandResult> consumer = _ => BiCommandResult.Success;
		dispatcher.RegisterConsumer(consumer);
		dispatcher.RemoveConsumer(consumer);
		Assert.Throws<CommandDispatcher.IllegalHandlerStateException>(() => dispatcher.RemoveConsumer(consumer));
	}

	[Test]
	public void TestRemovingMissingConsumer() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		dispatcher.RegisterConsumer<SampleCommand, BiCommandResult>(_ => BiCommandResult.Success);
		Assert.Throws<CommandDispatcher.IllegalHandlerStateException>(() =>
			dispatcher.RemoveConsumer<SampleCommand, BiCommandResult>(_ => BiCommandResult.Failure));
	}

	[Test]
	public void TestReaddingConsumer() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		CommandDispatcher.Consumer<SampleCommand, BiCommandResult> consumer = _ => BiCommandResult.Success;
		dispatcher.RegisterConsumer(consumer);
		dispatcher.RemoveConsumer(consumer);
		dispatcher.RegisterConsumer(consumer);
	}

	[Test]
	public void TestIssuingUnregisteredCommand() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		Assert.Throws<CommandDispatcher.IllegalHandlerStateException>(() => dispatcher.Issue(new SampleCommand()));
	}

	[Test]
	public void TestThrowingCommandHandler() {
		CommandDispatcher dispatcher = new CommandDispatcher();
		dispatcher.RegisterConsumer<SampleCommand, BiCommandResult>(_ => throw new Exception());
		Assert.Throws<CommandDispatcher.CommandInvocationException>(() => dispatcher.Issue(new SampleCommand()));
	}

	private class SampleCommand : BaseCommand {}
}

}
