using UnityEngine;
using UnityEngine.SceneManagement;

public class DDO : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextScene(string NextSceneName)
    {
        SceneManager.LoadScene(NextSceneName);
    }
}
