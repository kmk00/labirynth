using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class FindDestination : Agent
{

    [SerializeField] private Transform destination;
    [SerializeField] private GameObject startingPos;
    [SerializeField] private GameObject MazeGenerationObject;
    private GameObject currentMazeInstance;
    private GameObject startingCell;
    private Vector3 startingCellPos;
    private GameObject[] wallsPrefabs;
    private bool hasCollided = false;

    public override void OnEpisodeBegin()
    {

        
        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(MazeGenerationObject);
            StartCoroutine(InitializeStartPosition());

        }

        hasCollided = false;
        transform.localPosition = new Vector3(startingCellPos.x, .7f, startingCellPos.z);


    }

    private IEnumerator InitializeStartPosition()
    {
        yield return null;

        startingCell = GameObject.FindWithTag("StartCell");
        if (startingCell != null)
        {
            startingCellPos = startingCell.transform.position;
            transform.localPosition = new Vector3(startingCellPos.x, .7f, startingCellPos.z);

        }
        else
        {
            Debug.LogError("StartCell with the tag 'StartCell' not found.");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(destination.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuosActions = actionsOut.ContinuousActions;
        continuosActions[0] = Input.GetAxisRaw("Horizontal");
        continuosActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {


        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = .7f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

    }

    private void OnTriggerEnter(Collider other)
    {

        switch (other.gameObject.name)
        {
            case "Back Wall":
                Debug.Log("Back Wall"); 
                break;
            case "Front Wall":
                Debug.Log("F Wall");
                break;
            case "Right Wall":
                Debug.Log("R Wall");
                break;
            case "Left Wall":
                Debug.Log("L Wall");
                break;

        }

        if (hasCollided) return;

        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            SetReward(1f);
            DestroyElements();           
            Debug.Log("End Found");
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            hasCollided = true;
            EndEpisode();
        }
    }

    private void DestroyElements()
    {
        wallsPrefabs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject obj in wallsPrefabs) { Destroy(obj); }
        Destroy(GameObject.FindGameObjectWithTag("EndCell"));
        Destroy(GameObject.FindGameObjectWithTag("StartCell"));
        Destroy(currentMazeInstance);
        currentMazeInstance = null;
    }

}
