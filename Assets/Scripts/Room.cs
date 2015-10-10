using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Room : MonoBehaviour {

	public string roomLevelName;
	public int bestScore = 0;
	
	private const float FadingDuration = 1.0f;

	private Text scoreText;
	
	void Awake() {
		scoreText = GetComponentInChildren<Text>();
	}
	
	void Start()
    {
	    scoreText.text = bestScore.ToString();
    }

	public void RoomIsClicked() {
		StartCoroutine(LevelLoader.Instance.FadeAndLaunch(roomLevelName));
	}
	
	public void SetBestScore(int s) {
		bestScore = s;
		scoreText.text = bestScore.ToString();
	}

	public static string FormatTimeScore(int score)
    {
        string res = "";

        int seconds = score % 60;
        int minutes = score / 60;

        if (minutes != 0)
        {
            res += minutes + "m ";
        }

        res += seconds + "s";

        return res;
    }
}
