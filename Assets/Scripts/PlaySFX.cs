using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool detachOnPlay;

    public void Play()
    {
        if (!audioSource.clip) return;

        if (detachOnPlay)
        {
            audioSource.transform.SetParent(null);
            audioSource.Play();
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }
        else
        {
            audioSource.Play();
        }
    }
}
