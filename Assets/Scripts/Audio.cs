using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{

    public AudioSource soundEffects;
    public PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerManager.hasPowerUp == true)
        {
            soundEffects.Play();
            Debug.Log("Sound Playing");
        }
    }
}
