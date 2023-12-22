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
    private float collisionCooldown = 0.5f; // 0.5 seconds cooldown
    private float lastCollisionTime = -1f;
    private int maxMazeSize = 2;
    private HashSet<GameObject> visitedCells = new HashSet<GameObject>();
    private float maxRayDistance = 4f;


    public override void OnEpisodeBegin()
    {

        
        if (currentMazeInstance == null)
        {

            currentMazeInstance = Instantiate(MazeGenerationObject);
            StartCoroutine(InitializeStartPosition());

        }

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
        Vector3 normalizedAgentPos = transform.localPosition / maxMazeSize;
        Vector3 normalizedDestinationPos = destination.localPosition / maxMazeSize;
        float distanceToTarget = Vector3.Distance(destination.position, transform.position);


        sensor.AddObservation(normalizedAgentPos);
        sensor.AddObservation(normalizedDestinationPos);
        sensor.AddObservation(distanceToTarget);

        AddRaycastSensorData(sensor, transform.forward);
        AddRaycastSensorData(sensor, -transform.forward);
        AddRaycastSensorData(sensor, transform.right);
        AddRaycastSensorData(sensor, -transform.right);

    }

    private void AddRaycastSensorData(VectorSensor sensor, Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxRayDistance))
        {
            sensor.AddObservation(hit.distance / maxRayDistance);
        }
        else
        {
            sensor.AddObservation(1.0f); // Maksymalna odleg³oœæ
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuosActions = actionsOut.ContinuousActions;
        continuosActions[0] = Input.GetAxisRaw("Horizontal");
        continuosActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        // Dodaj karê za ka¿dy krok, aby zniechêciæ do niepotrzebnych ruchów.
       // AddReward(-0.01f);


        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = .7f;


        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;



    }

    private void OnTriggerEnter(Collider other)
    {


        if (Time.time - lastCollisionTime < collisionCooldown)
            return; // Skip if we are in cooldown



        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            SetReward(50f);
            DestroyElements();
            Debug.Log("End Found");
            visitedCells.Clear();
            EndEpisode();
        }
        
        if (other.CompareTag("OutsideWall") || other.CompareTag("InnerWall"))
        {
            SetReward(-2f);
            lastCollisionTime = Time.time; // Update last collision time
            visitedCells.Clear();
            EndEpisode();
        }

        if (other.CompareTag("Point"))
        {
            if (!visitedCells.Contains(other.gameObject))
            {
                // Dodaj do odwiedzonych i przyznaj wiêksz¹ nagrodê
                AddReward(0.5f);
                visitedCells.Add(other.gameObject);
                Debug.Log("Pierwszy Raz");
            }
            else
            {
                // Mniejsza nagroda za ponowne odwiedzenie
                //AddReward(.2f);
                Debug.Log("Visited");
            }
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
