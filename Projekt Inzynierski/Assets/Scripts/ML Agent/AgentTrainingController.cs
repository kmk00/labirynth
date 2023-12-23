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

    private GameObject currentMazeInstance;
    private GameObject startingCell;
    private GameObject endCell;
    private Vector3 startingCellPos;
    private Vector3 endingCellPos;
    private float lastCollisionTime = -1f;
    private float collisionCooldown = 0.5f;
    private GameObject[] wallsPrefabs;
    public override void OnEpisodeBegin()
    {
        Debug.Log("EpisodeBegin");

        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);
            StartCoroutine(InitializeStartPosition());

        }


        StartCoroutine(InitializeStartPosition());


    }


    public override void CollectObservations(VectorSensor sensor)
    {

        endCell = FindChildWithTag(environmentParentGameObject, "EndCell");
        
        if(endCell != null)
        {
            endingCellPos = endCell.transform.localPosition;
            sensor.AddObservation(endingCellPos);

        }

        sensor.AddObservation(transform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = .7f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
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
            EndEpisode();
        }

        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            SetReward(50f);
            DestroyMazeElementsExceptTag(environmentParentGameObject,"Agent");
            currentMazeInstance = null;
            Debug.Log("End Found");
            EndEpisode();
        }

    }

    //-----------------------------------------------------------------------------------------


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

        if (startingCell!= null)
        {
            // Child with the specified tag is found
            Debug.Log("Found child: " + startingCell.name);
        }
        else
        {
            // Child with the specified tag is not found
            Debug.Log("No child with the specified tag found.");
        }

        if (startingCell != null)
        {
            startingCellPos = startingCell.transform.localPosition;
            transform.localPosition = new Vector3(startingCellPos.x, .7f, startingCellPos.z);

        }
        else
        {
            Debug.LogError("StartCell with the tag 'StartCell' not found.");
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
