using UnityEngine;

public class LittleBrotherController : MonoBehaviour
{
    public Animator animator;
    public GameObject[] path;
    public float speed = 1.0f;
    private int pathIndex = 1;
    private bool isMoving = true;

    void Start()
    {
        if (path.Length < 1)
        {
            isMoving = false;
            return;
        }
        transform.position = path[0].transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        if (pathIndex >= path.Length)
        {
            isMoving = false;
            animator.SetFloat("Speed", 0f);
            SceneController.state = SceneController.SceneState.game;
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = path[pathIndex].transform.position;
        Vector3 difference = targetPos - currentPos;
        Vector3 movement = difference.normalized * speed * Time.deltaTime;

        if (movement.magnitude > difference.magnitude)
        {
            transform.position = targetPos;
            pathIndex++;

            animator.SetFloat("Speed", difference.magnitude / movement.magnitude * speed);

            return;
        }

        animator.SetFloat("Speed", speed);

        transform.position += movement;

        Quaternion rotation = new Quaternion();
        rotation.SetLookRotation(movement);
        transform.rotation = rotation;
    }
}