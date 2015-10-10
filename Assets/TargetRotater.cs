using UnityEngine;

public class TargetRotater : MonoBehaviour
{
    public Transform rotatee;
    public float speed = 1.0f;


    void Update()
    {
        Quaternion rot = rotatee.rotation;

        rot *= Quaternion.Euler(new Vector3(0, 0, -speed * Time.deltaTime));

        rotatee.rotation = rot;
    }
}