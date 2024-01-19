using UnityEngine;

public class CameraFollowQLearning : CameraScript
{
    void Update()
    {
        if(target is not null)
            target = GameObject.Find("Agentura(Clone)").GetComponent<Transform>();
    }
}
