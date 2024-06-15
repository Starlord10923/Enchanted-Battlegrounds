using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip startClip;
    public AudioClip loopClip;
    public bool isPlay = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startClip;
        audioSource.Play();
        Invoke("PlayLoop", audioSource.clip.length);

    }
    void PlayLoop(){
        isPlay = true;
        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    
}
