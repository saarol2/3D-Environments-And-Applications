using UnityEngine;

public class CityMusicTrigger : MonoBehaviour
{
    public AudioSource music;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            music.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            music.Stop();
        }
    }       
}
