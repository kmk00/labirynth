using UnityEngine;

public class CameraFollowBFS : CameraScript
{
    void Update()
    {
        if(target is not null)
            target = GameObject.Find("AIBFS(Clone)").GetComponent<Transform>();
    }
}
