using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds Instance;

    public AudioSource hitSoundAudioSource;
    public AudioSource laserShotAudioSource;
    public AudioSource reloadAudioSource;
    public AudioSource emptyMagazineAudioSource;
    public AudioSource swordAttackAudioSource;
    public AudioSource wooshDashAudioSource;
    public AudioSource pickUpWeaponAudioSource;
    public AudioSource pickUpCoinAudioSource;
    public AudioSource collectableUsedAudioSource;
    public AudioSource playerDeathAudioSource;

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

    public void PlayHitSound()
    {
        hitSoundAudioSource.Play();
    }

    public void PlayLaserShot()
    {
        laserShotAudioSource.Play();
    }

    public void PlayReloadSound()
    {
        reloadAudioSource.Play();
    }

    public void PlayEmptyMgazineSound()
    {
        emptyMagazineAudioSource.Play();
    }

    public void PlaySwordAttackSound()
    {
        swordAttackAudioSource.Play();
    }

    public void PlayDashSound()
    {
        wooshDashAudioSource.Play();
    }

    public void PlayPickUpWeaponSound()
    {
        pickUpWeaponAudioSource.Play();
    }

    public void PlayPickUpCoinSound()
    {
        pickUpCoinAudioSource.Play();
    }

    public void PlayCollectableItemUsedSound()
    {
        collectableUsedAudioSource.Play();
    }

    public void PlayPlayerDeathAudioSound()
    {
        playerDeathAudioSource.Play();
    }
}
