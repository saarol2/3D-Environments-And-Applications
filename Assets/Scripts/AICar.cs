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

    void Start()
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

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        transform.position += transform.forward * speed * Time.deltaTime;

        foreach (Transform wheel in Lwheels)
        {
            wheel.Rotate(Vector3.back * wheelRotationSpeed * Time.deltaTime);
        }

        foreach (Transform wheel in Rwheels)
        {
            wheel.Rotate(Vector3.forward * wheelRotationSpeed * Time.deltaTime);
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
