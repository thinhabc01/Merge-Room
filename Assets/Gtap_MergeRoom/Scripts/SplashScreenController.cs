using UnityEngine;

public class SplashScreenController : MonoBehaviour
{
    public bool ScreenActive
    {
        set
        {
            this.gameObject.SetActive(value);
        }
    }
}
