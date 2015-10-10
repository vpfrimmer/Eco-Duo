using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SwissArmyKnife;

public class LevelLoader : Singleton<LevelLoader> {
	
	private Image thisImage;
	
	public float fadeDuration = 1.0f;
	
	// Use this for initialization
	void Awake () {
		thisImage = GetComponent<Image>();
		thisImage.enabled = false;
	}
	
	public IEnumerator FadeAndLaunch(string levelName) {
		float t = 0f;
		thisImage.enabled = true;
		
		while(t < fadeDuration) {
			t += Time.deltaTime * Time.timeScale;
			thisImage.color = new Color(thisImage.color.r,
										thisImage.color.g,
										thisImage.color.b,
										Mathf.Clamp01(Mathf.Lerp(0f,1f, t/fadeDuration)));
			
			yield return new WaitForEndOfFrame();
		}
		
		Application.LoadLevel(levelName);
	}
}
