using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {
	
	private Image thisImage;
	private Text countdownText;
	
	void Awake() {
		countdownText = GetComponentInChildren<Text>();
		countdownText.text = SceneController.Instance.timeLimit.ToString();
		thisImage = GetComponent<Image>();
	}
	
	void Update () {
		if(ProgressionSaver.Instance != null && SceneController.Instance != null) {
			thisImage.fillAmount = Mathf.Clamp01(Mathf.Lerp(0,1, 1 - (ProgressionSaver.Instance.timer/SceneController.Instance.timeLimit)));
			countdownText.text = (SceneController.Instance.timeLimit - (int)ProgressionSaver.Instance.timer).ToString();	
		}
	}
}
