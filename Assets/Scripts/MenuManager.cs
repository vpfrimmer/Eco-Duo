using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public string directSceneName = "Game";
	
	public void DirectLaunch(string sceneName = null) {
		Application.LoadLevel((sceneName == null) ? directSceneName : sceneName);
	}
}
