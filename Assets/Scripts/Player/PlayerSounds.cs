using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds Instance;

    public AudioClip hitSoundClip;
    public AudioClip laserShotClip;

    private AudioSource playerAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void PlayHitSound()
    {
        playerAudioSource.clip = hitSoundClip;
        playerAudioSource.Play();
    }

    public void PlayLaserShot()
    {
        playerAudioSource.clip = laserShotClip;
        playerAudioSource.Play();
    }
}
