using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {
    private Image blackBackground;
    public float blackScreenLastingTime = 2f;
    // Use this for initialization
    void Start () {
        blackBackground = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if (blackBackground.color.a > 0.01)
        {
            blackScreenLastingTime -= Time.deltaTime;
            if (blackScreenLastingTime < 0)
            {
                var alpha = Mathf.Lerp(blackBackground.color.a, 0, Time.deltaTime);
                blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.b, blackBackground.color.g, alpha);
            }
        }
    }
}
