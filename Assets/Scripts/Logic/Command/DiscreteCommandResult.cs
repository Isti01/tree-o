namespace Logic.Command {

/// <summary>
/// Represents a command result that only has a finite amount of states and each state is named.
/// An example is <see cref="BiCommandResult"/>, which only has two states: success and failure.
/// </summary>
public abstract class DiscreteCommandResult : ICommandResult {
	private readonly string _name;

	public abstract bool IsSuccess { get; }

	/// <summary>
	/// Commands can be implicitly converted to boolean values:
	/// the value of <see cref="IsSuccess"/> is used.
	/// </summary>
	/// <param name="result">what to convert (implicitly)</param>
	/// <returns>the value of <see cref="IsSuccess"/></returns>
	//TODO move this to the interface (ICommandResult) if the C# language level allows it
	public static implicit operator bool(DiscreteCommandResult result) {
		return result.IsSuccess;
	}

	/// <summary>
	/// Creates and initializes a new instance.
	/// </summary>
	/// <param name="name">the name of this command result</param>
	protected DiscreteCommandResult(string name) {
		_name = name;
	}

	public override string ToString() {
		return GetType().Name + "." + _name;
	}
}

}
