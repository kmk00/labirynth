using UnityEngine;

public class CameraFollowPlayer : CameraScript
{
    void Update()
    {
        if(target is not null)
            target = GameObject.Find("Player(Clone)").GetComponent<Transform>();
    }
}
