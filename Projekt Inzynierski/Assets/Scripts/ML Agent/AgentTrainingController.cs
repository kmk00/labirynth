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

    private GameObject currentMazeInstance;
    private GameObject startingCell;
    private GameObject endCell;
    private Vector3 startingCellPos;
    private Vector3 endingCellPos;

    private float lastCollisionTime = -1f;
    private float collisionCooldown = 0.5f;
    private GameObject[] wallsPrefabs;

    private Rigidbody rb;

    private List<GameObject> disabledPoints = new List<GameObject>();





    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {

        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);
            StartCoroutine(InitializeStartPosition());

        }


        StartCoroutine(InitializeStartPosition());




    }


    public override void CollectObservations(VectorSensor sensor)
    {

        //endCell = FindChildWithTag(environmentParentGameObject, "EndCell");
        
        //Usunac target position i zmienic ilosc obserwacji

        //if(endCell != null)
       // {
        //    endingCellPos = endCell.transform.localPosition;
        //    sensor.AddObservation(endingCellPos);

       // }

        sensor.AddObservation(transform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed * 2.5f, 0f, Space.Self);

        /*
        Vector3 velocity = new Vector3(moveX,0f,moveZ);
        velocity = velocity.normalized * Time.deltaTime * moveSpeed;
        transform.localPosition += velocity;
         */
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

        if (other.CompareTag("OutsideWall") || other.CompareTag("InnerWall"))
        {
            
            lastCollisionTime = Time.time; // Update last collision time
            SetReward(-10f);
            ResetDisabledPoints();
            EndEpisode();
        }

        if (other.CompareTag("Point"))
        {

            SetReward(1f);
            disabledPoints.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }

        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            SetReward(50f);
            DestroyMazeElementsExceptTag(environmentParentGameObject,"Agent");
            currentMazeInstance = null;
            disabledPoints.Clear();
            EndEpisode();
        }

    }

    //-----------------------------------------------------------------------------------------


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


}
