using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Room : MonoBehaviour {

	public string roomLevelName;
	public int bestScore = 0;

    public Text timeScoreText;

	void Start()
    {
	    timeScoreText.text = bestScore.ToString();
    }

	public void RoomIsClicked() {
        Application.LoadLevel(roomLevelName);
	}
	
	public void SetBestScore(int s) {
		bestScore = s;
		timeScoreText.text = bestScore.ToString();
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
