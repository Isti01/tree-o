namespace Logic.Command {

public abstract class AbstractCommandResult : ICommandResult {
	private readonly string _name;

	public abstract bool IsSuccess { get; }

	public static implicit operator bool(AbstractCommandResult result) {
		return result.IsSuccess;
	}

	protected AbstractCommandResult(string name) {
		_name = name;
	}

	public override string ToString() {
		return GetType().Name + "." + _name;
	}
}

}
