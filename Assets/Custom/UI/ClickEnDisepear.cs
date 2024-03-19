using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public GameObject firstscreen;
    



public class ClickEnDisepear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ButtonChek();
    }
    public void ButtonChek()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            firstscreen.gameObject.SetActive(false);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.gameObject.SetActive(false);


            }

        }
            
    }
}
