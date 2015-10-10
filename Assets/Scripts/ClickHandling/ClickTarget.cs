using UnityEngine;

public class ClickTarget : MonoBehaviour, IClickTarget
{
    public delegate void ClickAction();
    public static event ClickAction OnClicked;

    public GameObject[] toHide;
    public GameObject[] toShow;
    private bool clickable = true;

    void Start()
    {
        SetAllActive(toShow, false);
    }

    public void OnTargetClicked()
    {
        if (clickable && SceneController.state == SceneController.SceneState.game)
        {
            SetAllActive(toHide, false);
	        SetAllActive(toShow, true);
	        
	        SceneController.Instance.foundObjects++;
	        
            if (OnClicked != null)
            {
                OnClicked();
            }
            clickable = false;
        }
    }

    private void SetAllActive(GameObject[] objects, bool state)
    {
        foreach (GameObject obj in objects)
        {
            if (obj)
            {
                obj.SetActive(state);
            }
        }
    }
}