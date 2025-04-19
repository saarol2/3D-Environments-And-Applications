using UnityEngine;

public class GrandpaQuest : MonoBehaviour
{
    public Transform player;
    public Transform grandpaTransform;
    public AudioSource grandpaAudioSource;

    public AudioClip initialVoiceLine;
    public AudioClip daughterReturnedVoiceLine;

    public float activationDistance = 6f;
    public float rotationSpeed = 2f;

    public Transform daughterTransform;
    public float daughterDistanceThreshold = 5f;

    private bool hasInteracted = false;
    private bool daughterReturned = false;

    void Update()
    {
        float playerDistance = Vector3.Distance(grandpaTransform.position, player.position);
        float daughterDistance = daughterTransform != null
            ? Vector3.Distance(grandpaTransform.position, daughterTransform.position)
            : Mathf.Infinity;

        if (daughterReturned && daughterDistance <= daughterDistanceThreshold)
        {
            FaceTarget(daughterTransform.position);
        }
        else if (playerDistance <= activationDistance)
        {
            FaceTarget(player.position);
        }

        if (!daughterReturned && daughterDistance <= daughterDistanceThreshold)
        {
            daughterReturned = true;
            hasInteracted = false;
        }
        if ((playerDistance <= activationDistance || daughterReturned) && !hasInteracted)
        {
            hasInteracted = true;
            TriggerInteraction();
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = -(targetPosition - grandpaTransform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            grandpaTransform.rotation = Quaternion.Slerp(grandpaTransform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TriggerInteraction()
    {
        if (grandpaAudioSource != null)
        {
            if (daughterReturned && daughterReturnedVoiceLine != null)
            {
                grandpaAudioSource.clip = daughterReturnedVoiceLine;
            }
            else if (initialVoiceLine != null)
            {
                grandpaAudioSource.clip = initialVoiceLine;
            }

            grandpaAudioSource.Play();
        }
    }
}
