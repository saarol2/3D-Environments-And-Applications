using UnityEngine;

public class GrandpaQuest : MonoBehaviour
{
    public Transform player;
    public Transform grandpaTransform;
    public Animator grandpaAnimator;
    public AudioSource grandpaAudioSource;
    public AudioClip grandpaVoiceLine;

    public float activationDistance = 6f;
    public float rotationSpeed = 2f;

    private bool hasInteracted = false;

    void Update()
    {
        float distance = Vector3.Distance(grandpaTransform.position, player.position);

        if (distance <= activationDistance)
        {
            Vector3 direction = (player.position - grandpaTransform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(-direction);
                grandpaTransform.rotation = Quaternion.Slerp(grandpaTransform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            if (!hasInteracted)
            {
                hasInteracted = true;
                TriggerInteraction();
            }
        }
    }

    void TriggerInteraction()
    {
        if (grandpaAnimator != null)
        {
            grandpaAnimator.SetTrigger("Wave");
        }

        if (grandpaAudioSource != null && grandpaVoiceLine != null)
        {
            grandpaAudioSource.clip = grandpaVoiceLine;
            grandpaAudioSource.Play();
        }
    }
}
