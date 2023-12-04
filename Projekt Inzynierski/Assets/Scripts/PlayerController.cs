using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;
    
    public Timer timer;

    private void Start()
    {

        timer = GetComponent<Timer>();
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EndCell"))
        {
            Debug.Log("Finish");
            timer.StopStopwatch();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StartCell"))
        {
            Debug.Log("Start");
            timer.StartStopwatch();
        }
    }

}
