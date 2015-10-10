using UnityEngine;

public class MouseClickController : MonoBehaviour
{
    Animation mouseClickAnim;
    void Start()
    {
        mouseClickAnim = this.GetComponent<Animation> ();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            PlayMouseClickEffect();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IClickTarget clickHandler = hit.collider.GetComponent<IClickTarget>();

                if (clickHandler != null)
                {
                    clickHandler.OnTargetClicked();
                    print("Click");
                }
            }
                
        }
    }

    void PlayMouseClickEffect()
    {
        var mouseOnScreen = Input.mousePosition;
        mouseOnScreen.z = Camera.main.transform.position.z;
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mouseOnScreen);
        mouseClickAnim.Play();
    }
}