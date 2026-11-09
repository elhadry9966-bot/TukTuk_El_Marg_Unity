using UnityEngine;

/// <summary>
/// Controls the player Tuk-Tuk / Vehicle:
/// - Physics & steering with authentic arcade feel
/// - Dynamic chassis roll on turns and pitch on acceleration/braking
/// - Front fork steering pivot and spinning wheels
/// - Collision detection with buildings and traffic vehicles with bounce pushback
/// - Horn audio and toast messages
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Vehicle Dynamics")]
    public float currentSpeed = 0.0f;
    public float heading = 0.0f; // In degrees
    public float steeringInput = 0.0f;

    [Header("References")]
    public ProceduralTuktukBuilder proceduralTuktuk;
    public GameObject customModelHolder;
    public Transform customModelInner;
    public AudioSource hornAudio;

    private float crashCooldown = 0.0f;
    private const float MapBound = 196.0f;

    void Start()
    {
        if (proceduralTuktuk == null)
        {
            proceduralTuktuk = gameObject.AddComponent<ProceduralTuktukBuilder>();
            proceduralTuktuk.BuildTuktuk();
        }

        // Add AudioSource for horn if missing
        if (hornAudio == null)
        {
            hornAudio = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        float dt = Mathf.Min(Time.deltaTime, 0.04f);
        if (crashCooldown > 0f) crashCooldown -= dt;

        // Gather Inputs (Keyboard + UI Hold Buttons)
        bool gasIn = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool brakeIn = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool leftIn = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightIn = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (UIController.Instance != null)
        {
            gasIn |= UIController.Instance.IsGasPressed();
            brakeIn |= UIController.Instance.IsBrakePressed();
            leftIn |= UIController.Instance.IsLeftPressed();
            rightIn |= UIController.Instance.IsRightPressed();
        }

        // Upgrade levels
        int engineLvl = GameManager.Instance != null ? GameManager.Instance.engineLevel : 1;
        int brakeLvl = GameManager.Instance != null ? GameManager.Instance.brakeLevel : 1;

        float maxSpeed = 20.0f + (engineLvl - 1) * 3.5f;
        float accel = 14.0f + (engineLvl - 1) * 2.8f;
        float brakeForce = 20.0f + (brakeLvl - 1) * 4.5f;

        // Acceleration & Braking
        if (gasIn)
        {
            currentSpeed += accel * dt;
        }
        if (brakeIn)
        {
            currentSpeed -= brakeForce * dt;
        }
        else
        {
            currentSpeed -= Mathf.Sign(currentSpeed) * 3.8f * dt;
        }

        if (!gasIn && !brakeIn && Mathf.Abs(currentSpeed) < 0.18f)
        {
            currentSpeed = 0.0f;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -7.0f, maxSpeed);

        // Steering
        steeringInput = (leftIn ? -1.0f : 0.0f) + (rightIn ? 1.0f : 0.0f);
        float steerAgility = 1.48f + (brakeLvl - 1) * 0.12f;
        if (Mathf.Abs(currentSpeed) > 0.2f)
        {
            heading += steeringInput * Mathf.Rad2Deg * dt * (steerAgility + Mathf.Abs(currentSpeed) * 0.052f) * Mathf.Sign(currentSpeed);
        }

        // Movement with Collision Detection
        Vector3 prevPos = transform.position;
        float rad = heading * Mathf.Deg2Rad;
        Vector3 movement = new Vector3(Mathf.Sin(rad) * currentSpeed * dt, 0, Mathf.Cos(rad) * currentSpeed * dt);
        Vector3 targetPos = transform.position + movement;

        targetPos.x = Mathf.Clamp(targetPos.x, -MapBound, MapBound);
        targetPos.z = Mathf.Clamp(targetPos.z, -MapBound, MapBound);

        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(0, heading, 0);

        // Visual tilts & rotations
        if (proceduralTuktuk != null)
        {
            proceduralTuktuk.SetBraking(brakeIn);

            // Wheel spinning
            foreach (var w in proceduralTuktuk.wheels)
            {
                if (w != null) w.Rotate(Vector3.right, currentSpeed * dt * 120.0f, Space.Self);
            }

            // Front wheel pivot
            if (proceduralTuktuk.frontWheelPivot != null)
            {
                float targetSteerAngle = steeringInput * 24.0f;
                float currentY = proceduralTuktuk.frontWheelPivot.localEulerAngles.y;
                if (currentY > 180f) currentY -= 360f;
                float newY = Mathf.Lerp(currentY, targetSteerAngle, 0.18f);
                proceduralTuktuk.frontWheelPivot.localRotation = Quaternion.Euler(0, newY, 0);
            }

            // Chassis roll tilt & pitch
            if (proceduralTuktuk.chassisGroup != null)
            {
                float targetRoll = -steeringInput * Mathf.Abs(currentSpeed) * 0.45f;
                float targetPitch = brakeIn ? -2.3f : (gasIn ? 1.4f : 0.0f);
                Quaternion targetRot = Quaternion.Euler(targetPitch, 0, targetRoll);
                proceduralTuktuk.chassisGroup.localRotation = Quaternion.Lerp(proceduralTuktuk.chassisGroup.localRotation, targetRot, 0.15f);
            }
        }

        // Handle Collisions
        HandleCollisions(prevPos);
    }

    private void HandleCollisions(Vector3 prevPos)
    {
        Vector2 pt = new Vector2(transform.position.x, transform.position.z);

        // Building collision
        foreach (var r in CitySetup.BuildingRects)
        {
            Rect expanded = new Rect(r.x - 1.6f, r.y - 1.6f, r.width + 3.2f, r.height + 3.2f);
            if (expanded.Contains(pt))
            {
                transform.position = prevPos;
                currentSpeed = -currentSpeed * 0.45f;
                if (crashCooldown <= 0f)
                {
                    if (UIController.Instance != null)
                        UIController.Instance.ShowToast("💥 حاسب المباني يا أسطى!");
                    crashCooldown = 1.2f;
                }
                return;
            }
        }

        // Traffic collision
        var trafficMgr = FindObjectOfType<TrafficManager>();
        if (trafficMgr != null)
        {
            foreach (var car in trafficMgr.activeCars)
            {
                if (car == null) continue;
                float dx = transform.position.x - car.transform.position.x;
                float dz = transform.position.z - car.transform.position.z;
                float d = Mathf.Sqrt(dx * dx + dz * dz);
                if (d < 3.3f)
                {
                    float nx = d > 0.001f ? dx / d : Mathf.Sin(heading * Mathf.Deg2Rad);
                    float nz = d > 0.001f ? dz / d : Mathf.Cos(heading * Mathf.Deg2Rad);
                    transform.position = car.transform.position + new Vector3(nx * 3.5f, 0, nz * 3.5f);
                    currentSpeed = -Mathf.Sign(currentSpeed != 0 ? currentSpeed : 1.0f) * 4.5f;

                    if (crashCooldown <= 0f)
                    {
                        if (UIController.Instance != null)
                            UIController.Instance.ShowToast("🚗 حاسب العربيات في الشارع!");
                        crashCooldown = 1.2f;
                    }
                    return;
                }
            }
        }
    }

    public void HonkHorn()
    {
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowToast("📯 بيب بيب! وسع لتوك توك المرج يا أسطى");
        }
        if (hornAudio != null)
        {
            // Play procedural beep tone if no audio clip assigned
            hornAudio.Play();
        }
    }
}
