using UnityEngine;
using UnityEngine.UI;
using SwissArmyKnife;
using System.Collections;


public class PopupWin : Singleton<PopupWin>
{
    public Button nextLevel;

    public Animator[] stars;
    public int[] coinToStar;
    public Animator[] coins;

    public float delay = 0.1f;

    private int starIndex = 0;

    public void Start()
    {
        Enable(9, 9);
    }

    public void Enable(int found, int total)
    {
        gameObject.SetActive(true);
        if (found < coinToStar[0])
        {
            nextLevel.interactable = false;
        }

        StartCoroutine(WinCoroutine(found));
    }

    IEnumerator WinCoroutine(int found)
    {
        for (int i = 0; i < found; i++)
        {
            coins[i].SetTrigger("Start");

            if (i >= coinToStar[starIndex] - 1)
            {
                stars[starIndex].SetTrigger("Start");
                starIndex++;

                yield return new WaitForSeconds(delay * 3f);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
}