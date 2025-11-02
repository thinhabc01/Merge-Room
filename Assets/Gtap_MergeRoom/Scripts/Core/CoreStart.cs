using UnityEngine;
using UnityEngine.SceneManagement;

public static class CoreStart
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad()
    {
        var coreSettings = Resources.Load("CoreSettings") as CoreSettings;

        if (coreSettings == null)
        {
            Debug.LogError("asset core settings does not exist");
            return;
        }

#if UNITY_EDITOR
        coreSettings.IsCoreStart = coreSettings.CoreScene == SceneManager.GetActiveScene().name;

        if (coreSettings.IsCoreStart == false)
        {
            SceneManager.LoadSceneAsync(coreSettings.CoreScene, LoadSceneMode.Additive);
        }
#else
        coreSettings.IsCoreStart = true;
#endif

    }
}