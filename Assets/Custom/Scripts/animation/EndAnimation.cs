using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndAnimation : MonoBehaviour
{
   
    void Start()
     {
         Invoke("LoadNextSceneFunction", 10.4f);
       
        
    }

    public void LoadNextSceneFunction()
    {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
     }
   
}

