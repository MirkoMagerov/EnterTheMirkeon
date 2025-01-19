using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] music;
    public AudioClip bossMusic;
    private AudioSource musicSource;
    private int previousSongIndex = -1;
    private bool isPlayingBossMusic = false;

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        PlayRandomMusic();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    private void PlayRandomMusic()
    {
        if (music.Length == 0) return;

        int newSongIndex;
        do
        {
            newSongIndex = Random.Range(0, music.Length);
        } while (newSongIndex == previousSongIndex);

        previousSongIndex = newSongIndex;
        musicSource.clip = music[newSongIndex];
        musicSource.Play();
    }

    public void PlayBossMusic()
    {
        if (bossMusic == null || isPlayingBossMusic) return;

        isPlayingBossMusic = true;
        musicSource.clip = bossMusic;
        musicSource.Play();
    }

    public void StopBossMusic()
    {
        if (!isPlayingBossMusic) return;

        isPlayingBossMusic = false;
        PlayRandomMusic();
    }
}
