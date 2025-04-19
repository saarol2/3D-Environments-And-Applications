using UnityEngine;

public class WomanQuest : MonoBehaviour
{
    public Transform player;
    public Transform grandpaTransform;
    public Animator npcAnimator;
    public AudioSource npcAudioSource;
    public AudioClip voiceLine;

    public float activationDistance = 6f;
    public float rotationSpeed = 2f;
    public float followDistance = 2f;
    public float moveSpeed = 2f;
    public float stopNearGrandpaDistance = 2.5f;

    private bool hasInteracted = false;
    private bool isFollowing = false;
    private bool isNearGrandpa = false;

    private Transform followTarget;

    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (!hasInteracted && playerDistance <= activationDistance)
        {
            hasInteracted = true;
            TriggerInteraction();
        }

        if (hasInteracted && !isFollowing && !isNearGrandpa)
        {
            FaceTarget(player);
        }

        if (isFollowing)
        {
            CheckIfNearGrandpa();
            if (!isNearGrandpa)
            {
                FollowTarget();
            }
        }

        if (isNearGrandpa)
        {
            FaceTarget(grandpaTransform);
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("IsWalking", false);
            }
        }
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TriggerInteraction()
    {
        if (npcAudioSource != null && voiceLine != null)
        {
            npcAudioSource.clip = voiceLine;
            npcAudioSource.Play();
            Invoke(nameof(StartFollowing), voiceLine.length);
        }
        else
        {
            StartFollowing();
        }
    }

    void StartFollowing()
    {
        isFollowing = true;
        followTarget = player;
    }

    void CheckIfNearGrandpa()
    {
        if (grandpaTransform != null)
        {
            float distanceToGrandpa = Vector3.Distance(transform.position, grandpaTransform.position);

            if (distanceToGrandpa <= stopNearGrandpaDistance)
            {
                isNearGrandpa = true;
                isFollowing = false;
                followTarget = null;
            }
        }
    }

    void FollowTarget()
    {
        if (followTarget == null) return;

        float distance = Vector3.Distance(transform.position, followTarget.position);

        if (distance > followDistance)
        {
            Vector3 direction = (followTarget.position - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * moveSpeed * Time.deltaTime;

            Quaternion lookRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (npcAnimator != null)
            {
                npcAnimator.SetBool("IsWalking", true);
            }
        }
        else
        {
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("IsWalking", false);
            }
        }
    }
}
