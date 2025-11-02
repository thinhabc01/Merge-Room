using UnityEngine;

public static class Extension
{
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static bool RandomBool(this int chance, int options = 101)                          
    {
        int result = Random.Range(1, options);
        //Debug.Log("шанс: " + chance + " из " + (options-1) + ". Выпало: " + result);
        return (result < chance);
    }
}
