
namespace CqrsExample.Domain.Features.Shopping;

public static class ShoppingListErrors
{
    #region Validation

    public static readonly (string, string) InvalidId =
        ("SHOPPING_LIST_INVALID_ID", "The shopping list id is invalid, must be a non-empty Guid.");
    public static readonly (string, string) BlankTitle =
        ("SHOPPING_LIST_INVALID_BLANK_TITLE", "The shopping list title must not be blank.");
    public static readonly (string, string) RepeatedItems =
        ("SHOPPING_LIST_NO_REPEATED_ITEMS", "The shopping list must not have repeated items.");
    public static readonly (string, string) ItemsNull =
        ("SHOPPING_LIST_ITEMS_NULL", "The items array cannot be null.");
    public static readonly (string, string) BlankItemName =
        ("SHOPPING_LIST_BLANK_ITEM_NAME", "The item name cannot be blank.");
    public static readonly (string, string) ItemQuantityZeroOrLess =
        ("SHOPPING_LIST_ITEM_QTY_ZERO_OR_LESS", "The item quantity must be greater than zero (item name: {0}).");

    #endregion

    #region Get/update list

    public static readonly (string, string) ShoppingListNotFound =
        ("SHOPPING_LIST_NOT_FOUND", "Shopping list not found.");

    #endregion

    #region Internal error

    public static readonly (string, string) InternalError =
        ("SHOPPING_LIST_INTERNAL_ERROR", "An internal error happened, try again later.");

    #endregion
}