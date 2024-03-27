using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ClickEnDisepear : MonoBehaviour
{

    public GameObject firstScreen;
    public GameObject secondScreen;
    public GameObject thirthScreen;
    public GameObject forthScreen;
    public GameObject MachineCount;
    private int switched = 0;
    [SerializeField] private AudioClip musicSoundClip;

    void Start()
    {
        secondScreen.SetActive(false);
        thirthScreen.SetActive(false);
        forthScreen.SetActive(false);
        MachineCount.SetActive(false);
    }


    void Update()
    {
        ButtonChek();
    }
    public void ButtonChek()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (switched == 0)
            {
                firstScreen.SetActive(false);
                secondScreen.SetActive(true);
            }
            if (switched == 1)
            {
                secondScreen.SetActive(false);
                thirthScreen.SetActive(true);
            }
            if (switched == 2)
            {
                thirthScreen.SetActive(false);
                forthScreen.SetActive(true);
            }
            if (switched == 3)
            {
                forthScreen.SetActive(false);
                MachineCount.SetActive(true);
                EventManager.Invoke(new GameStartedEvent());
                SoundManager.instance.PlaySoundClip(musicSoundClip, transform, 1f);
            }
            switched++;
        }
    }
    }

