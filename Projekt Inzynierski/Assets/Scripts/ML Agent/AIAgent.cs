using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AIAgent : Agent
{
    [SerializeField]
    private float moveSpeed;
    private Rigidbody rb;
    private List<GameObject> disabledPoints = new List<GameObject>();
    private float pointReactivationTime = 14;
    private float pointReward = 2f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed * 2.5f, 0f, Space.Self);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point"))
        {
            disabledPoints.Add(other.gameObject);
            StartCoroutine(ReactivatePointAfterDelay(other.gameObject, pointReactivationTime));
            other.gameObject.SetActive(false);
        }
    }

    private IEnumerator ReactivatePointAfterDelay(GameObject point, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (point != null)
        {
            point.SetActive(true);
            disabledPoints.Remove(point);
        }
    }
}
