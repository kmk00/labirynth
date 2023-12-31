using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentTrainingController : Agent
{
    [SerializeField]
    private GameObject mazeGeneratorObject;
    [SerializeField]
    private GameObject environmentParentGameObject;
    [SerializeField]
    private float moveSpeed = 4f;
    [SerializeField]
    private float timeForEp;
    [SerializeField]
    private float pointReactivationTime = 5f;


    //Rewards
    [SerializeField]
    private float wallReward;
    [SerializeField]
    private float pointReward;
    [SerializeField]
    private float timePenaltyReward;
    [SerializeField]
    private float goalReward;
    [SerializeField]
    private float OneSecondPenalty;

    private GameObject currentMazeInstance;
    private GameObject startingCell;
    private Vector3 startingCellPos;

    private float lastCollisionTime = -1f;
    private float collisionCooldown = 0.4f;
    private GameObject[] wallsPrefabs;

    private Rigidbody rb;
    private Renderer agentRenderer;

    private List<GameObject> disabledPoints = new List<GameObject>();

    private float timeLeft;

    private float lastPenaltyTime;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        agentRenderer = GetComponent<Renderer>();
        lastPenaltyTime = Time.time;
    }

    private void Update()
    {
        CheckRemainingTime();
    }

    public override void OnEpisodeBegin()
    {

        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);
            StartCoroutine(InitializeStartPosition());

        }


        StartCoroutine(InitializeStartPosition());
        EpisodeTimerNew();
        lastPenaltyTime = Time.time;



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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastCollisionTime < collisionCooldown)
            return; // Skip if we are in cooldown
        /*
        if (other.CompareTag("OutsideWall") || other.CompareTag("InnerWall"))
        {
            
            lastCollisionTime = Time.time; // Update last collision time
            SetReward(wallReward);
            ResetDisabledPoints();
            agentRenderer.material.color = Color.red;
            EndEpisode();
        }
         */
         

        if (other.CompareTag("Point"))
        {

            SetReward(pointReward);
            disabledPoints.Add(other.gameObject);
            StartCoroutine(ReactivatePointAfterDelay(other.gameObject, pointReactivationTime));
            other.gameObject.SetActive(false);
        }

        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            SetReward(goalReward);
            DestroyMazeElementsExceptTag(environmentParentGameObject,"Agent");
            currentMazeInstance = null;
            disabledPoints.Clear();
            agentRenderer.material.color = Color.green;
            EndEpisode();
        }

    }

    //-----------------------------------------------------------------------------------------

    private IEnumerator ReactivatePointAfterDelay(GameObject point, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (point != null)
        {
            point.SetActive(true);
            disabledPoints.Remove(point);
        }
    }

    private void ResetDisabledPoints()
    {
        foreach (var point in disabledPoints)
        {
            point.SetActive(true);
        }
        disabledPoints.Clear();
    }

    private void DestroyMazeElementsExceptTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (!child.CompareTag(tag))
            {
                Destroy(child.gameObject);
            }
        }

    }

    private IEnumerator InitializeStartPosition()
    {
        yield return null;

        startingCell = FindChildWithTag(environmentParentGameObject, "StartCell");

        if (startingCell != null)
        {
            startingCellPos = startingCell.transform.localPosition;
            transform.localPosition = new Vector3(startingCellPos.x, .4f, startingCellPos.z);
            rb.angularVelocity = Vector3.zero; 
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.identity;

        }
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null; // Return null if no child with the tag is found
    }

    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEp;

    }

    private void CheckRemainingTime()
    {
        if(Time.time >= timeLeft)
        {
            AddReward(timePenaltyReward);
            agentRenderer.material.color = Color.blue;
            ResetDisabledPoints();
            EndEpisode();
        }
        else if (Time.time - lastPenaltyTime > 1.0f) // Na przyk³ad co 1 sekundê
        {
            AddReward(-OneSecondPenalty); // Sta³a, niewielka kara za czas
            lastPenaltyTime = Time.time;
        }
    }

}
