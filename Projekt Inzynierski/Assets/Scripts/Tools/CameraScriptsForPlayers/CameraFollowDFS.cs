using UnityEngine;

public class CameraFollowDFS : CameraScript
{
    void Update()
    {
        if(target is not null)
            target = GameObject.Find("AIDFS(Clone)").GetComponent<Transform>();
    }
}
