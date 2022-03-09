namespace Logic.Command {

/// <summary>
/// Base class for the results of <see cref="CommandBase{T}"/> subclasses.
/// </summary>
public interface ICommandResult {
	/// <summary>
	/// Whether this instance represents a successful command execution.
	/// </summary>
	public bool IsSuccess { get; }
}

}
