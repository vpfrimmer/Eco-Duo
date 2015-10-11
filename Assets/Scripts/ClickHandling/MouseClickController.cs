using UnityEngine;

public class MouseClickController : MonoBehaviour
{
    public GameObject mouseClickGizmo;
    public AudioClip coinSound;
    private Animator mouseClickAnimator;

    void Start()
    {
        mouseClickAnimator = mouseClickGizmo.GetComponent<Animator> ();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SceneController.state == SceneController.SceneState.game)
        {
            PlayMouseClickEffect();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IClickTarget clickHandler = hit.collider.GetComponent<IClickTarget>();

                if (clickHandler != null)
                {
                    if (clickHandler.OnTargetClicked())
                        AudioManager.Instance.Play(coinSound, 1f, 0.3f); ;
                    print("Click");
                }
            }
        }
    }

    void PlayMouseClickEffect()
    {
        var mouseOnScreen = Input.mousePosition;

        RectTransform rectTransform = mouseClickGizmo.transform as RectTransform;

        rectTransform.anchoredPosition = mouseOnScreen;

        mouseClickAnimator.SetTrigger("Start");
    }
}