using UnityEngine;

public class MouseClickController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IClickTarget clickHandler = hit.collider.GetComponent<IClickTarget>();

                if (clickHandler != null)
                {
                    clickHandler.OnTargetClicked();
                }
            }
        }
    }
}