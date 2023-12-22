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

    public override void OnEpisodeBegin()
    {

        currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);

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
