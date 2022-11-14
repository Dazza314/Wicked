using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void onPlay()
    {
        SceneManager.LoadScene(Scene.Game);
    }
}
