using UnityEngine;

public class AICar : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 10f;
    public float rotationSpeed = 5f;

    [Header("Wheels")]
    public Transform[] Lwheels;
    public Transform[] Rwheels;
    public float wheelRotationSpeed = 360f;

    [Header("Yield Settings")]
    public float yieldCheckDistanceLong = 10f;
    public float yieldCheckDistanceShort = 7f;
    public LayerMask carLayer; // Anna Inspectorissa esim. "AI Car" layer
    public bool debugDraw = true;
    public Transform raycastOrigin; // Yksi tyhjä GameObject lähtökohdaksi

    [Header("Character Detection")]
    public Transform character;
    public Transform radarPoint;
    public float slowDownDistance = 8f;
    public float stopDistance = 4f;

    void Start()
    {
        HideWaypoints();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        float speedModifier = 1f;

        if (ShouldYieldToRight()) return;

        float distanceToCharacter;

        if (IsCharacterInFront(out distanceToCharacter))
        {
            if (distanceToCharacter < stopDistance)
            {
                return; // pysähdy
            }
            else if (distanceToCharacter < slowDownDistance)
            {
                speedModifier = 0.4f; // hidasta
            }
        }

        MoveTowardsWaypoint(speedModifier);
        RotateWheels();
        RotateTowardsDirection();
        UpdateWaypointIndex();
    }

    void HideWaypoints()
    {
        foreach (Transform waypoint in waypoints)
        {
            MeshRenderer meshRenderer = waypoint.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    void MoveTowardsWaypoint(float speedModifier)
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.position += transform.forward * speed * speedModifier * Time.deltaTime;
    }

    void RotateWheels()
    {
        foreach (Transform wheel in Lwheels)
        {
            wheel.Rotate(Vector3.back * wheelRotationSpeed * Time.deltaTime);
        }

        foreach (Transform wheel in Rwheels)
        {
            wheel.Rotate(Vector3.forward * wheelRotationSpeed * Time.deltaTime);
        }
    }

    void RotateTowardsDirection()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateWaypointIndex()
    {
        Transform target = waypoints[currentWaypointIndex];
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    bool ShouldYieldToRight()
    {
        Vector3[] longDirections = {
            (raycastOrigin.right + raycastOrigin.forward * 0.5f).normalized,
            (raycastOrigin.right + raycastOrigin.forward * 0.75f).normalized,
            (raycastOrigin.forward + raycastOrigin.right * 0.75f).normalized,
            raycastOrigin.forward.normalized,
        };
        Vector3[] shortDirections = {
            (raycastOrigin.forward + raycastOrigin.right * 0.5f).normalized,
            (raycastOrigin.forward + raycastOrigin.right * 0.25f).normalized,
        };

        return CheckRaycasts(longDirections, yieldCheckDistanceLong) || CheckRaycasts(shortDirections, yieldCheckDistanceShort);
    }

    bool CheckRaycasts(Vector3[] directions, float distance)
    {
        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(raycastOrigin.position, direction);
            if (debugDraw)
            {
                Debug.DrawRay(raycastOrigin.position, direction * distance, Color.red);
            }

            if (Physics.Raycast(ray, distance, carLayer))
            {
                return true;
            }
        }
        return false;
    }

    bool IsCharacterInFront(out float distanceToCharacter)
{
        distanceToCharacter = Vector3.Distance(radarPoint.position, character.position);
        Vector3 directionToCharacter = (character.position - radarPoint.position).normalized;

        if (distanceToCharacter < slowDownDistance && Vector3.Dot(radarPoint.forward, directionToCharacter) > 0.5f)
        {
            if (debugDraw)
            {
                Debug.DrawLine(radarPoint.position, character.position, Color.blue);
            }
            return true;
        }

        return false;
    }
}