namespace NapAdminCore.Helpers;

using System.Collections;

public static class HashtableExtensions
{
    public static int SafeGetInt(this Hashtable hashtable, int key, int defaultValue = 0)
    {
        if (hashtable.ContainsKey(key))
        {
            return (int)hashtable[key];
        }

        return defaultValue;
    }
}
