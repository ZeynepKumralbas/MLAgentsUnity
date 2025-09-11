using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RL : Agent
{
    private Rigidbody rbody;
    public Transform target;
    public float moveMultiplier = 5f;
    public float jumpForce = 5f;
    private bool isGrounded = true;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Eğer ajan düşerse resetle
    //    if (transform.localPosition.y < 0)
    //    {
        rbody.angularVelocity = Vector3.zero;
        rbody.linearVelocity = Vector3.zero;
    //    transform.localPosition = new Vector3(0, 0.5f, 0f);
        transform.localPosition = new Vector3(-3.6f, 0.8f, -14f);
    //    }

        // Hedefi rastgele bir noktaya koy
    //    target.localPosition = new Vector3(Random.value * 8.5f - 4, 0.5f, Random.value * 8.5f - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hedef ve ajan pozisyonu
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Ajanın hızı
        sensor.AddObservation(rbody.linearVelocity.x);
        sensor.AddObservation(rbody.linearVelocity.z);

        // Yerde mi? (bool → 0 veya 1)
        sensor.AddObservation(isGrounded ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rbody.AddForce(controlSignal * moveMultiplier);

        // Zıplama aksiyonu (Discrete)
        if (actions.DiscreteActions[0] == 1 && isGrounded)
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        // Hedefe ulaşırsa
        if (distanceToTarget < 3f)
        {
            SetReward(5.0f);
            EndEpisode();
        }

        // Ajan sahadan düşerse
        if (transform.localPosition.y < -1f)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Yatay hareket
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        // Zıplama (space basılıysa 1 gönder)
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Yere temas ettiğinde tekrar zıplayabilir
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Eğer duvara çarparsa küçük negatif ödül
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f);
        }
    }
}
