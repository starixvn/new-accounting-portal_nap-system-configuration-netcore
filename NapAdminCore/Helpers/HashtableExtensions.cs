namespace NapAdminCore.Helpers;

using System.Collections;

public static class HashtableExtensions
{
    public static T SafeGet<T>(this Hashtable hashtable, object key, T defaultValue)
    {
        if (hashtable.ContainsKey(key))
        {
            return (T)hashtable[key];
        }

        return defaultValue;
    }

    public static int SafeGetInt(this Hashtable hashtable, int key, int defaultValue = 0)
    {
        if (hashtable.ContainsKey(key))
        {
            return (int)hashtable[key];
        }

        return defaultValue;
    }
}
