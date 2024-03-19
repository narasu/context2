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
    private int switched = 0;

    void Start()
    {
        secondScreen.SetActive(false);
        thirthScreen.SetActive(false);
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
                switched = 0;
            }
            switched++;
        }
    }
    }

