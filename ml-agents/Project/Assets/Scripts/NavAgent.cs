using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NavAgent : Agent
{
    Rigidbody rBody;
    public Transform Target;   // Hedef (mesela bir küp)

    public float moveSpeed = 100f;
    public float turnSpeed = 150f;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Eğer ajan düşerse, sıfırla
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.linearVelocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // İstersen hedefi sabit tut, istersen random spawn yap
        // Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hedefin konumu
        sensor.AddObservation(Target.localPosition);

        // Ajanın kendi konumu
        sensor.AddObservation(this.transform.localPosition);

        // Ajanın hızı
        sensor.AddObservation(rBody.linearVelocity.x);
        sensor.AddObservation(rBody.linearVelocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float move = actionBuffers.ContinuousActions[0];  // -1 ile 1 arası
        float turn = actionBuffers.ContinuousActions[1];  // -1 ile 1 arası

        // Hareket uygula
        rBody.MovePosition(transform.position + transform.forward * move * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turn * turnSpeed * Time.fixedDeltaTime);

        // Ödül mantığı
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Hedefe ulaşınca ödül ver
        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Arena dışına çıkarsa cezalandır
        if (this.transform.localPosition.y < -1 ||
            Mathf.Abs(this.transform.localPosition.x) > 10 ||
            Mathf.Abs(this.transform.localPosition.z) > 10)
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical"); // W-S
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // A-D
    }

    // Duvar veya engelle çarpışma durumunda ceza
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.2f);  // küçük bir ceza
        }
    }
}
