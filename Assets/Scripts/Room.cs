using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Room : MonoBehaviour {

	public string roomLevelName;
	public int bestTimeScore = 0;

    public Text timeScoreText;

	void Start()
    {
        timeScoreText.text = FormatTimeScore(bestTimeScore);
    }

	public void RoomIsClicked() {
        Application.LoadLevel(roomLevelName);
	}
	
	public void SetBestTime(int t) {
		bestTimeScore = t;
		timeScoreText.text = FormatTimeScore(bestTimeScore);
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
