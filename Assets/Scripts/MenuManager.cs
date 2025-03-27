using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SoundController.Instance.PlayBGM("MenuBGM");
        SoundController.Instance.SetBGMVolume(0.2f);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string scenename)
    {
        SoundController.Instance.SetSFXVolume(1.0f);
        SoundController.Instance.PlaySfx("Mechanical", transform.position, 1.0f);
        SceneManager.LoadScene(scenename);
    }
}
