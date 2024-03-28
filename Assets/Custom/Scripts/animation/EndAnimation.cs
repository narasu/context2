using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndAnimation : MonoBehaviour
{

    public float time = 10.4f;
    void Start()
     {
         Invoke("LoadNextSceneFunction", time);
       
        
    }

    public void LoadNextSceneFunction()
    {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
     }
   
}

