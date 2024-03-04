using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    MusicMainMenu music;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("FirstDemo");
        Debug.Log("Button clicked");
        music.musicSource.Stop();

    }

   /* public void OnApplicationQuit()
    {
       OnApplicationQuit();
    }*/

    public void Rules()
    {
        
    }
}
