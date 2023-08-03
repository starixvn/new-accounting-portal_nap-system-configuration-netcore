namespace NapAdminCore.Helpers;

using System.Collections;

public static class HashtableExtensions
{
    public static Hashtable ToHashtable(object obj)
    {
        var dict = obj.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(obj));

        return new Hashtable(dict);
    }

    public static Hashtable ToHashtable(object first, object second)
    {
        var firstHash = ToHashtable(first);
        var secondHash = ToHashtable(second);

        var dict = firstHash.Cast<DictionaryEntry>()
            .Union(secondHash.Cast<DictionaryEntry>())
            .ToDictionary(e => e.Key, e => e.Value);

        return new Hashtable(dict);
    }

    public static Hashtable ToHashtable(object first, object second, object third)
    {
        var firstHash = ToHashtable(first);
        var secondHash = ToHashtable(second);
        var thirdHash = ToHashtable(third);

        var dict = firstHash.Cast<DictionaryEntry>()
            .Union(secondHash.Cast<DictionaryEntry>())
            .Union(thirdHash.Cast<DictionaryEntry>())
            .ToDictionary(e => e.Key, e => e.Value);

        return new Hashtable(dict);
    }
}