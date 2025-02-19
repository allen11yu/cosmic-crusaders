using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    [Header("Input Smoothing")]
    public float inputSmoothTime = 0.1f;

    private float boostDepletionRate = 1f;

    private Rigidbody rb;
    private PlayerStats playerStats;

    private float rollInput;
    private Vector2 screenCenter;
    private Vector2 currentMouseDistance;
    private Vector2 mouseDistanceVelocity;

    private float activeForwardSpeed;
    private bool isBoosting;
    private bool wasBoosting;

    public AudioClip boostSound;
    private AudioSource boostAudioSource;

    public AudioClip movementSound;
    private AudioSource movementAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerStats = GetComponent<PlayerStats>();

        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        boostAudioSource = GetComponent<AudioSource>();
        boostAudioSource = gameObject.AddComponent<AudioSource>();
        boostAudioSource.clip = boostSound;

        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.clip = movementSound;
        movementAudioSource.loop = true;

        wasBoosting = false;
    }

    void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;

        float turnSpeed = HandleMovement();
        HandleRotation(turnSpeed);
        HandleMovementSound();
    }
    
    private float HandleMovement()
    {
        isBoosting = Input.GetKey(KeyCode.LeftShift);
        if (isBoosting && !wasBoosting)
        {
            StartBoostSound();
        }
        else if (!isBoosting && wasBoosting)
        {
            StopBoostSound();
        }

        float verticalInput = Input.GetAxisRaw("Forwards");
        float turnSpeed = this.playerStats.lookRotateSpeed;
        float targetSpeed = 0f;

        if (isBoosting)
        {
            if (playerStats.currentBoost > 0)
            {
                targetSpeed = playerStats.boostSpeed;
                turnSpeed = playerStats.lookRotateSpeed * 0.75f;

                playerStats.AddBoost(Mathf.FloorToInt(-boostDepletionRate * Time.fixedDeltaTime));
            }
            else
            {
                isBoosting = false;
                StopBoostSound();
            }
        }
        else if (verticalInput > 0)
        {
            targetSpeed = playerStats.speed;
        }
        else if (verticalInput == 0)
        {
            targetSpeed = playerStats.idleSpeed;
            turnSpeed = this.playerStats.lookRotateSpeed * 1.15f;
        }
        else if (verticalInput < 0)
        {
            targetSpeed = 1f;
            turnSpeed = this.playerStats.lookRotateSpeed * 1.25f;
        }

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, targetSpeed * playerStats.idleSpeed, playerStats.forwardAcceleration * Time.fixedDeltaTime);

        Vector3 movement = transform.forward.normalized * activeForwardSpeed;
        Vector3 idleMovement = transform.forward.normalized * playerStats.idleSpeed;

        rb.velocity = movement + idleMovement;

        wasBoosting = isBoosting;

        return turnSpeed;
    }

    private void HandleRotation(float turnSpeed)
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector2 mouseDistance = new Vector2(
            (mousePosition.x - screenCenter.x) / screenCenter.y,
            (mousePosition.y - screenCenter.y) / screenCenter.y
        );

        // Doesnt allow for more rotation on the x-axis than the y
        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        currentMouseDistance = Vector2.SmoothDamp(currentMouseDistance, mouseDistance, ref mouseDistanceVelocity, inputSmoothTime);

        float targetRoll = Input.GetAxisRaw("Roll");
        rollInput = Mathf.Lerp(rollInput, targetRoll, playerStats.rollAcceleration * Time.fixedDeltaTime);

        // Calculate rotation
        float pitch = -currentMouseDistance.y * turnSpeed;
        float yaw = currentMouseDistance.x * turnSpeed;
        float roll = rollInput * playerStats.rollSpeed;

        Vector3 rotation = new Vector3(pitch, yaw, roll);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation * Time.fixedDeltaTime));
    }

    private void HandleMovementSound()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();
            }
        }
        else
        {
            if (movementAudioSource.isPlaying)
            {
                movementAudioSource.Stop();
            }
        }
    }


    private void StartBoostSound()
    {
        if (boostSound != null && boostAudioSource != null && !boostAudioSource.isPlaying)
        {
            boostAudioSource.Play();
        }
    }

    private void StopBoostSound()
    {
        if (boostAudioSource != null && boostAudioSource.isPlaying)
        {
            boostAudioSource.Stop();
        }
    }
}
