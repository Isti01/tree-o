using System;
using System.Collections.Generic;

namespace Logic.Command {

/// <summary>
/// Class through which <see cref="BaseCommand{T}"/> instances can be issued
/// and command handlers can be registered.
/// </summary>
public class CommandDispatcher {
	private readonly IDictionary<Type, Delegate> _consumers =
		new Dictionary<Type, Delegate>();

	/// <summary>
	/// Sets the handler of a command. The command mustn't already have a handler.
	/// </summary>
	/// <param name="consumer">the (new) handler</param>
	/// <typeparam name="TC">the command</typeparam>
	/// <typeparam name="TR">the command's return type</typeparam>
	/// <exception cref="IllegalHandlerStateException">if a handler is already set</exception>
	public void RegisterConsumer<TC, TR>(Consumer<TC, TR> consumer)
		where TC : BaseCommand<TR> where TR : ICommandResult {
		if (_consumers.TryGetValue(typeof(TC), out Delegate old))
			throw new IllegalHandlerStateException($"{typeof(TC)} already has a handler: {old}");

		_consumers[typeof(TC)] = consumer;
	}

	/// <summary>
	/// Unsets (clears) the handler of a command.
	/// The command must have the specified value as its handler.
	/// </summary>
	/// <param name="consumer">the (current) handler</param>
	/// <typeparam name="TC">the command</typeparam>
	/// <typeparam name="TR">the command's return type</typeparam>
	/// <exception cref="IllegalHandlerStateException">if the passed parameter isn't the current handler</exception>
	public void RemoveConsumer<TC, TR>(Consumer<TC, TR> consumer)
		where TC : BaseCommand<TR> where TR : ICommandResult {
		if (!_consumers.Remove(new KeyValuePair<Type, Delegate>(typeof(TC), consumer))) {
			throw new IllegalHandlerStateException($"{typeof(TC)} isn't associated with handler: {consumer}");
		}
	}

	/// <summary>
	/// Executes (invokes) the specified command.
	/// </summary>
	/// <param name="command">the command to execute</param>
	/// <typeparam name="T">the return type of the command</typeparam>
	/// <returns>the command's result</returns>
	/// <exception cref="IllegalHandlerStateException">if the command doesn't have a handler</exception>
	/// <exception cref="CommandInvocationException">if the command's execution fails exceptionally</exception>
	public T Issue<T>(BaseCommand<T> command) where T : ICommandResult {
		if (!_consumers.TryGetValue(command.GetType(), out Delegate consumer)) {
			throw new IllegalHandlerStateException($"Command '{command}' doesn't have a handler");
		}

		try {
			return (T) consumer.DynamicInvoke(command);
		} catch (Exception e) {
			throw new CommandInvocationException($"Error issuing {command} with handler {consumer}", e);
		}
	}

	/// <summary>
	/// Consumes a command and returns a value of the command's return type.
	/// In other words, it consumes/executes the command it receives.
	/// </summary>
	/// <typeparam name="TC">type of command being handled</typeparam>
	/// <typeparam name="TR">return type of command being handled</typeparam>
	public delegate TR Consumer<in TC, out TR>(TC command)
		where TC : BaseCommand<TR> where TR : ICommandResult;

	/// <summary>
	/// Exception that signals that handlers aren't in the expected state.
	/// </summary>
	public class IllegalHandlerStateException : Exception {
		public IllegalHandlerStateException(string message) : base(message) {}
	}

	/// <summary>
	/// Exception that signals that a command's execution failed.
	/// </summary>
	public class CommandInvocationException : Exception {
		public CommandInvocationException(string message, Exception e) : base(message, e) {}
	}
}

}
