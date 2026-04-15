using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDataBaseSO audioDB;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [Space]
    private Transform player;

    private AudioClip lastMusicPlayed;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmShouldPlay;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (bgmSource.isPlaying == false && bgmShouldPlay)
        {
            if (string.IsNullOrEmpty(currentBgmGroupName) == false)
                NextBGM(currentBgmGroupName);
        }

        if (bgmSource.isPlaying && bgmShouldPlay == false)
            StopBGM();
    }

    public void StartBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName)
            return;

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
        
        
        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
       
    }

    public void StopBGM()
    {
        bgmShouldPlay = false;

        StartCoroutine(FadeVolumeCo(bgmSource, 0, 1));

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        //Debug.Log(musicGroup +  "   music");
        AudioClipData data = audioDB.Get(musicGroup);
     

        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("No audio found for group" + musicGroup);
            yield break;
        }

        AudioClip nextMusic = data.GetRandomClip();
        if (data.clips.Count > 1)
        {
            while (nextMusic == lastMusicPlayed)
                nextMusic = data.GetRandomClip();
        }

        if (bgmSource.isPlaying)
            yield return FadeVolumeCo(bgmSource, 0, 1f);

        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 1f));

    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHearSound = 5)
    {
 

        var data = audioDB.Get(soundName);
        if (data == null)
        {
            Debug.LogError("PlaySFX ERROR: No data for soundName: " + soundName);
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
        {
            Debug.LogError("PlaySFX ERROR: Clip for " + soundName + " is NULL!");
            return;
        }

        if (player == null)
            player = Player.instance.transform;

 
        if (data == null)
        {
            Debug.Log("Attempt to play sound - " + soundName);
            return;
        }
        if (clip == null) return;

        float maxVolume = data.maxVolume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));

        sfxSource.pitch = Random.Range(.95f, 1.1f);
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t); // exponential falloff
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        Debug.Log("Played audio " + soundName);
        sfxSource.pitch = Random.Range(.95f, 1.1f);
        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }
}
