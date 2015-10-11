using UnityEngine;
using UnityEngine.UI;
using SwissArmyKnife;
using System.Collections;


public class PopupWin : Singleton<PopupWin>
{
    public Button nextLevel;

    public Animator popup;
    public Animator[] stars;
    public int[] coinToStar;
    public Animator[] coins;

    public float interval = 0.2f;
    public float delay = 1f;

    public AudioClip coinSound;
    public AudioClip starSound;

    private int starIndex = 0;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void Enable(int found, int total)
    {
        gameObject.SetActive(true);
        //if (found < coinToStar[0])
        //{
          //  nextLevel.interactable = false;
        //}

        StartCoroutine(WinCoroutine(found));
    }

    IEnumerator WinCoroutine(int found)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < found; i++)
        {
            AudioManager.Instance.Play(coinSound, 0.2f, 0.15f);
            coins[i].SetTrigger("Start");

            if (i >= coinToStar[starIndex] - 1)
            {
                yield return new WaitForSeconds(interval * 2f);

                AudioManager.Instance.Play(starSound, 0.2f, 0.15f);
                stars[starIndex].SetTrigger("Start");
                starIndex++;

                yield return new WaitForSeconds(interval * 2f);
            }
            else
            {
                yield return new WaitForSeconds(interval);
            }
        }
        if (found == coins.Length)
        {
            for (int i = 0; i < found; i++)
            {
                AudioManager.Instance.Play(coinSound, 0.2f, 0.15f);
                coins[i].SetTrigger("Jump");
                yield return new WaitForSeconds(interval * 0.5f);
            }
        }
    }
}