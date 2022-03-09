namespace Logic.Command {

/// <summary>
/// Base class for all commands.
/// </summary>
/// <typeparam name="T">return type (when the command is issued)</typeparam>
public abstract class CommandBase<T> where T : ICommandResult {}

/// <summary>
/// Shorthand for <code>CommandBase&lt;BiCommandResult&gt;</code>
/// </summary>
public abstract class CommandBase : CommandBase<BiCommandResult> {}

}
