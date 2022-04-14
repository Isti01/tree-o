namespace Logic.Command {

/// <summary>
/// Base class for all commands.
/// </summary>
/// <typeparam name="T">return type (when the command is issued)</typeparam>
//Disable false warning; the command's return type must be set in the command's definition.
// ReSharper disable once UnusedTypeParameter
public abstract class BaseCommand<T> where T : ICommandResult {}

/// <summary>
/// Shorthand for <code>BaseCommand&lt;BiCommandResult&gt;</code>
/// </summary>
public abstract class BaseCommand : BaseCommand<BiCommandResult> {}

}
