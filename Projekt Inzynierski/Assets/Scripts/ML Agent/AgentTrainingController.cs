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
    private Vector3 startingCellPos;
    public override void OnEpisodeBegin()
    {

        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);
            StartCoroutine(InitializeStartPosition());

        }


        StartCoroutine(InitializeStartPosition());

        Debug.Log("InitPos");

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

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }


}
