
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class PlayAudioSprite : MonoBehaviour
{
    public static float lastActionTime ;
    public bool isTest = false;
    [SerializeField] 
    private SoundClip[] soundClips; 
    [SerializeField] 
    private string clipToPlayOnStart; 
    [SerializeField] 
    private bool isLoop; 
    
    
    [SerializeField] 
    private GameObject database; 
    [SerializeField] 
    private GameObject dotnet; 
    [SerializeField] 
    private GameObject go; 
    [SerializeField] 
    private GameObject graalvm; 
    [SerializeField] 
    private GameObject helidon; 
    [SerializeField] 
    private GameObject javascript; 
    [SerializeField] 
    private GameObject micronaut; 
    [SerializeField] 
    private GameObject python; 
    [SerializeField] 
    private GameObject rust; 
    [SerializeField] 
    private GameObject springboot;

    public void Start()
    {
        lastActionTime = Time.time;
    }
    // public void Start()
    // {
    //     if (isTest)
    //     {
    //         foreach (SoundClip soundClip in soundClips)
    //         {
    //             Debug.Log("play clip:"+soundClip.name);
    //             StartCoroutine(PlayClip(soundClip.startValue, soundClip.endValue));
    //         }
    //     }
    //     if (clipToPlayOnStart != null)
    //     {
    //         Play(clipToPlayOnStart);
    //     }
    // }

    public void Stop()
    {
        Play(clipToPlayOnStart);
    }
    public void Play()
    {
        Play(clipToPlayOnStart);
    }

    public void Play(string objectName)
    {

        if (Time.time - lastActionTime < 2f) return;
        lastActionTime = Time.time;
        AudioSource audioSource = null;
            switch (objectName)
            {
                case "database":
                    audioSource = database.GetComponent<AudioSource>();
                    break;
                case "dotnet":
                    audioSource = dotnet.GetComponent<AudioSource>();
                    break;
                case "go":
                    audioSource = go.GetComponent<AudioSource>();
                    break;
                case "graalvm":
                    audioSource = graalvm.GetComponent<AudioSource>();
                    break;
                case "helidon":
                    audioSource = helidon.GetComponent<AudioSource>();
                    break;
                case "javascript":
                    audioSource = javascript.GetComponent<AudioSource>();
                    break;
                case "micronaut":
                    audioSource = micronaut.GetComponent<AudioSource>();
                    break;
                case "python":
                    audioSource = python.GetComponent<AudioSource>();
                    break;
                case "rust":
                    audioSource = rust.GetComponent<AudioSource>();
                    break;
                case "springboot":
                    audioSource = springboot.GetComponent<AudioSource>();
                    break;
                default:
               //     dotnet.GetComponent<AudioSource>().Play();
                    break;
            }

            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.8f, 1.2f);  // for -3 to 3, default at 1
                audioSource.panStereo = Random.Range(-1.0f, 1.0f);
                audioSource.volume = .5f;
                audioSource.Play();
            }
    }
    
    public void Play0(string objectName)
    {
        SoundClip objectByName = GetObjectByName(objectName);
        Debug.Log("play objectByName:"+objectByName);
        StartCoroutine(PlayClip(objectByName.startValue, objectByName.endValue));
    }
    
    SoundClip GetObjectByName(string objectByName)
    {
        foreach (SoundClip obj in soundClips)
        {
            if (obj.name == objectByName)
            {
                return obj; 
            }
        }
        Debug.Log("stack coming... object not found for objectByName:"+objectByName);
        StackTrace stackTrace = new StackTrace(); 
        Debug.Log(stackTrace.ToString()); 
        return null; 
    }

    //not the best way to do this but doesn't seem to have an impact and addresses a bug where the whole audio clip is played
    private bool isActiveSound; 
    IEnumerator PlayClip(float start, float end)
    {
        Debug.Log("play start_end:"+start+" "+end + " isActiveSound:" + isActiveSound);
        int soundActiveCount = 0;
        while (isActiveSound)
        {
            if (soundActiveCount++ > 3) isActiveSound = false;
            yield return new WaitForSeconds(1f); 
        }
        isActiveSound = true;
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.8f, 1.2f);  // for -3 to 3, default at 1
      //  audioSource.reverbZoneMix = Random.Range(0f, 1.1f);
        if (end <= start || start < 0 || end > audioSource.clip.length)
        {
            Debug.LogError("Invalid start or stop time.");
            yield break;
        }
        audioSource.time = start;
        audioSource.Play();
        yield return new WaitUntil(() => audioSource.time >= end || !audioSource.isPlaying);
        // audioSource.SetScheduledEndTime(AudioSettings.dspTime + (end - start));
        audioSource.Stop();
        isActiveSound = false;
    }
    
    [System.Serializable] 
    public class SoundClip
    {
        public string name;
        public float startValue;
        public float endValue;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

}
