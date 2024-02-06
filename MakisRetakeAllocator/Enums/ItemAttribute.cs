using CounterStrikeSharp.API.Modules.Entities.Constants;

public class ItemAttribute : Attribute {
    public CsItem theItem { get; private set; }

    public ItemAttribute(CsItem anItem) {
        theItem = anItem;
    }
}