using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;

namespace CqrsExample.Domain.BaseAbstractions.Commands;

public abstract class Command
{
}

public enum CommandResultFailureType
{
    Validation = 1,
    NotFound = 2,
    InternalError = 3
}

public abstract class CommandResult
{
}

public sealed class CommandResult<A>
    where A : CommandResult
{
    public A? SuccessResult { get; private set; }
    public CommandResultFailureType FailureType { get; private set; }
    public Error[]? Errors { get; private set; }
    public bool IsSuccess => Errors == null || Errors.Length == 0;

    private CommandResult(A successResult) => SuccessResult = successResult;
    private CommandResult(CommandResultFailureType failureType, Error[] errors)
    {
        FailureType = failureType;
        Errors = errors;
    }

    public static CommandResult<A> Success(A successResult) => new(successResult);
    public static CommandResult<A> Fail(CommandResultFailureType failureType, Error[] errors) => new(failureType, errors);
    public static CommandResult<A> Fail(CommandResultFailureType failureType, Error error) => Fail(failureType, new[] { error });
}

public interface ICommandHandler<C, R>
    where C : Command
    where R : CommandResult
{
    Task<CommandResult<R>> HandleAsync(C cmd);
}