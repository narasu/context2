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
    //ClickEnDissapear is muziek
    //MovementController is stap?
    // BTDetect = guard hey/stop
   // [SerializeField] private AudioClip guardSoundClip;

   // SoundManager.instance.PlaySoundClip(guardSoundClip, transform, 1f);
}
