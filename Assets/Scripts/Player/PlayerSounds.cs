using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds Instance;

    public AudioClip hitSoundClip;
    public AudioClip laserShotClip;
    public AudioClip reloadClip;
    public AudioClip emptyMagazineClip;
    public AudioClip swordAttackClip;
    public AudioClip wooshDashClip;

    private float ogVolume;
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
        ogVolume = playerAudioSource.volume;
    }

    public void PlayHitSound()
    {
        playerAudioSource.volume = 1;
        playerAudioSource.clip = hitSoundClip;
        playerAudioSource.Play();
        playerAudioSource.volume = ogVolume;
    }

    public void PlayLaserShot()
    {
        playerAudioSource.clip = laserShotClip;
        playerAudioSource.Play();
    }

    public void PlayReloadSound()
    {
        playerAudioSource.volume = 1;
        playerAudioSource.clip = reloadClip;
        playerAudioSource.Play();
        playerAudioSource.volume = ogVolume;
    }

    public void PlayEmptyMgazineSound()
    {
        playerAudioSource.volume = 1;
        playerAudioSource.clip = emptyMagazineClip;
        playerAudioSource.Play();
        playerAudioSource.volume = ogVolume;
    }

    public void PlaySwordAttackSound()
    {
        playerAudioSource.clip = swordAttackClip;
        playerAudioSource.Play();
    }

    public void PlayDashSound()
    {
        playerAudioSource.clip = wooshDashClip;
        playerAudioSource.Play();
    }

    public void StopActualSoundClip()
    {
        playerAudioSource.Stop();
        playerAudioSource.clip = null;
    }
}
