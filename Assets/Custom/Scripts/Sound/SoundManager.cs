using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//tutoriol https://www.youtube.com/watch?v=DU7cgVsU2rM
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource soundObject;

    private void Awake()
    {
       if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundClip(AudioClip audioClip,Transform spawnTransform, float volume)
    {
        //spawn Object
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);
        // assing audioClip
        audioSource.clip = audioClip;
        // assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //lenght of audio;
        float clipLength = audioSource.clip.length;
        //destroy object
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRandomSoundClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        // random index
        int rand = Random.Range(0, audioClip.Length);
        //spawn Object
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);
        // assing audioClip
        audioSource.clip = audioClip[rand];
        // assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //lenght of audio;
        float clipLength = audioSource.clip.length;
        //destroy object
        Destroy(audioSource.gameObject, clipLength);
    }
    //ClickEnDissapear is muziek
    //MovementController is stap?
    // ViewCone = guard hey/stop
    // [SerializeField] private AudioClip guardSoundClip;

    // SoundManager.instance.PlaySoundClip(guardSoundClip, transform, 1f);
}
