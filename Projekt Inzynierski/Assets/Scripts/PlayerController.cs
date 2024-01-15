using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed;
    [SerializeField]
    Rigidbody _playerRigidbody;


    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        _playerRigidbody.MovePosition(transform.position + movement * _speed * Time.fixedDeltaTime);
    }
}
