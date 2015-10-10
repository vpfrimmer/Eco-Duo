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

            float fillAmount = 1 - (Mathf.Floor(ProgressionSaver.Instance.timer) / SceneController.Instance.timeLimit);

            fillAmount = Mathf.Clamp01(fillAmount);

            thisImage.fillAmount = fillAmount;

			countdownText.text = (SceneController.Instance.timeLimit - (int)ProgressionSaver.Instance.timer).ToString();	
		}
	}
}
