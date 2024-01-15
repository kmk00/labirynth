using UnityEngine;

public class Timer : MonoBehaviour
{
    private  float timeStart = 0;
    private static bool finish = false;

    private void Start()
    {
        finish = false;
    }

    private void Update()
    {
        if(finish)
            return;

        timeStart += Time.deltaTime;
        SharedData.Time = timeStart;
        Debug.Log(SharedData.Time);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Agent"))
        {
            finish = true;
        }
    }
}
