using UnityEngine;

public class Timer : MonoBehaviour
{
    private  float timeStart = 0;
    private static bool finish = false;
    private static string _winner = "";

    private void Start()
    {
        _winner = "";
        SharedData.Winner = _winner;
        finish = false;
    }

    private void Update()
    {
        if(finish)
            return;

        timeStart += Time.deltaTime;
        SharedData.Time = timeStart;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !finish)
        {
            finish = true;
            _winner = "Win: Player";
            SharedData.Winner = _winner;
        }
        if(collision.gameObject.CompareTag("Agent") && !finish)
        {
            finish = true;
            _winner = "Win: AI";
            SharedData.Winner = _winner;
        }
    }
}
