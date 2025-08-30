using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ILAgent : Agent
{
    private Rigidbody rbody;
    public Transform target;
    public float multiplier = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbody = GetComponent<Rigidbody>();    
    }

    public override void OnEpisodeBegin(){
        if (transform.localPosition.y < 0){
            rbody.angularVelocity = Vector3.zero;
            rbody.linearVelocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0f);
        }

        target.localPosition = new Vector3(Random.value * 8.5f - 4, 0.5f, Random.value * 8.5f - 4);
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(rbody.linearVelocity.x);
        sensor.AddObservation(rbody.linearVelocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions){
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rbody.AddForce(controlSignal * multiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
    /*    if(distanceToTarget < 1.5f){
            SetReward(1.0f);
            EndEpisode();
        }
        if(transform.localPosition.y < 0f){
            SetReward(-1f);
            EndEpisode();
        }
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");   // W-S
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // A-D
    }

}
