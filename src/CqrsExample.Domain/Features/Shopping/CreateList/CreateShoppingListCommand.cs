using CqrsExample.Domain.BaseAbstractions.Commands;

namespace CqrsExample.Domain.Features.Shopping.CreateList;

public sealed class CreateShoppingListCommand : Command
{
    public string? Title { get; set; }
}