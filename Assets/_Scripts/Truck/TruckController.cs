using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TruckController : NetworkBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 25f; // Units per second
    public float acceleration = 8f;
    public float reverseSpeed = 8f;
    public float turnSpeed = 40f;
    public float brakeForce = 15f;

    [Header("Auto Transmission")]
    public float reverseThreshold = 1f; // Must be below this speed to reverse

    [Header("References")]
    public Transform driverSeat;
    public Transform[] passengerSeats;

    [SyncVar] public float currentSpeed = 0f;
    [SyncVar] public bool isStopped = false;
    [SyncVar] public uint currentDriverNetId;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 2000f; // Heavy truck feel
        rb.drag = 1f;
        rb.angularDrag = 5f;
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Breakdown)
        {
            ApplyBrakes();
            currentSpeed = 0f;
            isStopped = true;
            return;
        }

        if (currentDriverNetId == 0)
        {
            // No driver - idle deceleration
            ApplyBrakes();
            isStopped = Mathf.Abs(rb.velocity.magnitude) < 0.5f;
            currentSpeed = rb.velocity.magnitude;
            return;
        }

        HandleMovement();
    }

    [Server]
    private void HandleMovement()
    {
        bool wantsReverse = verticalInput < -0.1f;
        bool wantsForward = verticalInput > 0.1f;
        float forwardVel = Vector3.Dot(rb.velocity, transform.forward);

        if (wantsReverse && Mathf.Abs(forwardVel) > reverseThreshold)
        {
            // Holding reverse while moving forward = BRAKE, not reverse
            ApplyBrakes();
        }
        else if (wantsReverse && Mathf.Abs(forwardVel) <= reverseThreshold)
        {
            // Actually reversing
            rb.AddForce(-transform.forward * reverseSpeed * Mathf.Abs(verticalInput), ForceMode.Acceleration);
        }
        else if (wantsForward)
        {
            rb.AddForce(transform.forward * acceleration * verticalInput, ForceMode.Acceleration);
        }
        else
        {
            ApplyBrakes();
        }

        // Steering only when moving
        if (rb.velocity.magnitude > 1f)
        {
            float steerFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, horizontalInput * turnSpeed * steerFactor * Time.fixedDeltaTime, 0));
        }

        // Cap speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        currentSpeed = rb.velocity.magnitude;
        isStopped = currentSpeed < 0.5f;
    }

    [Server]
    private void ApplyBrakes()
    {
        rb.AddForce(-rb.velocity * brakeForce, ForceMode.Acceleration);
    }

    // Called by the Driver player
    [Command(requiresAuthority = false)]
    public void CmdSetInputs(float h, float v, uint driverNetId)
    {
        horizontalInput = h;
        verticalInput = v;
        currentDriverNetId = driverNetId;
    }

    [Command(requiresAuthority = false)]
    public void CmdReleaseDriver(uint driverNetId)
    {
        if (currentDriverNetId == driverNetId)
        {
            currentDriverNetId = 0;
            horizontalInput = 0;
            verticalInput = 0;
        }
    }

    public bool IsDriver(uint netId) => currentDriverNetId == netId;
}