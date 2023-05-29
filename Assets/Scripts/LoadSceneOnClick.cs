using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public string sceneName;

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}