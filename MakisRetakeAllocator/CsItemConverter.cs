using CounterStrikeSharp.API.Modules.Entities.Constants;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

public class CsItemConverter : ValueConverter<CsItem?, string> {

    public CsItemConverter() : base(
        aCsItem => CsItemSerializer(aCsItem),
        aString => CsItemDeserializer(aString)
    ) {
    }

    public static string CsItemSerializer(CsItem? anItem) {
        return JsonSerializer.Serialize(anItem);
    }

    public static CsItem? CsItemDeserializer(string? aString) {
        if (aString == null) {
            return null;
        }

        return JsonSerializer.Deserialize<CsItem>(aString);
    }
}