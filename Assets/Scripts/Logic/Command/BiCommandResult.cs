namespace Logic.Command {
/// <summary>
/// A general implementation of <see cref="ICommandResult"/> that only has two
/// instances, <see cref="Success"/> and <see cref="Failure"/>,
/// corresponding to the possible values of <see cref="ICommandResult.IsSuccess"/>.
/// </summary>
public sealed class BiCommandResult : AbstractCommandResult {
	public static readonly BiCommandResult Success = new BiCommandResult("Success");
	public static readonly BiCommandResult Failure = new BiCommandResult("Failure");

	public override bool IsSuccess => this == Success;

	private BiCommandResult(string name) : base(name) {}
}
}
