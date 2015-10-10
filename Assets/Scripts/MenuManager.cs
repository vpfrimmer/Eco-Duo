using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public string directSceneName = "Game";
    public GameObject rulesPanel;
    public GameObject creditsPanel;
    private Animator rulesAnimator;
    private Animator creditsAnimator;

    public void Start()
    {
        creditsAnimator = creditsPanel.GetComponent<Animator>();
        rulesAnimator = rulesPanel.GetComponent<Animator>();
        rulesPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void DirectLaunch(string sceneName = null) {
		Application.LoadLevel((sceneName == null) ? directSceneName : sceneName);
	}

    public void ShowRules()
    {
        rulesPanel.SetActive(true);
        rulesAnimator.SetBool("Open", true);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        creditsAnimator.SetBool("Open", true);
    }

    public void HideRules()
    {
        rulesAnimator.SetBool("Open", false);
    }

    public void HideCredits()
    {
        creditsAnimator.SetBool("Open", false);
    }
}
