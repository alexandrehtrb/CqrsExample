using System.Threading.Tasks;
using CqrsExample.Domain.BaseAbstractions.Errors;

namespace CqrsExample.Domain.BaseAbstractions.Commands
{
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
        public bool IsSuccess => this.Errors == null || this.Errors.Length == 0;

        private CommandResult(A successResult) => this.SuccessResult = successResult;
        private CommandResult(CommandResultFailureType failureType, Error[] errors)
        {
            this.FailureType = failureType;
            this.Errors = errors;
        }

        public static CommandResult<A> Success(A successResult) => new CommandResult<A>(successResult);
        public static CommandResult<A> Fail(CommandResultFailureType failureType, Error[] errors) => new CommandResult<A>(failureType, errors);
        public static CommandResult<A> Fail(CommandResultFailureType failureType, Error error) => Fail(failureType, new[] { error });
    }

    public interface ICommandHandler<C, R>
        where C : Command
        where R : CommandResult
    {
        Task<CommandResult<R>> HandleAsync(C cmd);
    }
}