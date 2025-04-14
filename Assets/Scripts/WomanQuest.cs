using UnityEngine;

public class WomanQuest : MonoBehaviour
{
    public Transform player;
    public Animator npcAnimator;
    public AudioSource npcAudioSource;
    public AudioClip voiceLine;

    public float activationDistance = 6f;
    public float rotationSpeed = 2f;
    public float followDistance = 2f;
    public float moveSpeed = 2f;

    private bool hasInteracted = false;
    private bool isFollowing = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (!hasInteracted && distance <= activationDistance)
        {
            FacePlayer();

            hasInteracted = true;
            TriggerInteraction();
        }

        if (isFollowing)
        {
            FollowPlayer();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TriggerInteraction()
    {
        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger("Wave");
        }

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
    }

    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
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
