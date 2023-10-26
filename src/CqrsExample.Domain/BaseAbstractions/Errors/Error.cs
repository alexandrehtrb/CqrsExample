using System;

namespace CqrsExample.Domain.BaseAbstractions.Errors;

public sealed class Error
{
    public string Code { get; private set; }
    public string Message { get; private set; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public Error((string, string) codeMessage, params string?[] messageFormatArgs)
        : this(codeMessage.Item1, string.Format(codeMessage.Item2, messageFormatArgs))
    {
    }

    public override bool Equals(object? obj) =>
        obj is Error error &&
        Code == error.Code &&
        Message == error.Message;

    public override int GetHashCode() =>
        HashCode.Combine(Code, Message);
}