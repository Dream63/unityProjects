using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicPlayer;
    public AudioSource audioSource;
    public AudioClip clickSFX, deathSFX, pointSFX;
    public float musicVolume = 1;
    public float sfxVolume = 1;

    public void JumpSound()
    {
        audioSource.clip = clickSFX;
        audioSource.Play();
    }
    public void DeathSound()
    {
        audioSource.clip = deathSFX;
        audioSource.Play();
    }
    public void PointSound()
    {
        audioSource.clip = pointSFX;
        audioSource.Play();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        musicPlayer.volume = musicVolume;
        audioSource.volume = sfxVolume;
        musicPlayer.Play();
    }
}
