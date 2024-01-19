using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform Target; 
    public float smoothSpeed = 0.125f;
    Vector3 upCamera = new Vector3(0, 8.0f, 0);
    void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 desiredPosition = Target.position + upCamera; 
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}