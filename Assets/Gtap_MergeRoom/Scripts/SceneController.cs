using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private List<string> _scenes = new List<string>();
    private string _currentScene;
    private string _initialScene;
    private SplashScreenController _splashScreen;

    public void Setup(List<string> scenes, SplashScreenController splashScreen, string initialScene)
    {
        _splashScreen = splashScreen;
        _scenes.AddRange(scenes);
        _initialScene = initialScene;
    }

    public async void LoadingScene()
    {
        PoolManager.ClearPool();
        _splashScreen.ScreenActive = true;

        _currentScene = SceneManager.GetActiveScene().name;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_initialScene));
 
        if(_currentScene != _initialScene)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_currentScene);

            while (!asyncUnload.isDone)
            {
                await Task.Yield();
            }
        }

        _currentScene = _scenes[0];
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);
 
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentScene));
        
        _splashScreen.ScreenActive = false;
    }
}