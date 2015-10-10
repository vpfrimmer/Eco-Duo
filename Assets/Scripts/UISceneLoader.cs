using UnityEngine;

/// <summary>
/// Called by GUI to control scene switching.
/// </summary>
public class UISceneLoader:MonoBehaviour
{
    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadLevel(int index)
    {
        Application.LoadLevel(index);
    }

    public void LoadLevel(string name)
    {
        Application.LoadLevel(name);
    }

    public void NextLevel()
    {
        Application.LoadLevel(Application.loadedLevel + 1);
    }

    public void PrevLevel()
    {
        Application.LoadLevel(Application.loadedLevel - 1);
    }

    public void ResetLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}


