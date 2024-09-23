using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
   
    private void Start()
    {
      
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject gmrGameObject = GameObject.FindGameObjectWithTag("GMR");

            if(gmrGameObject != null)
            {
                GMR gmr = gmrGameObject.GetComponent<GMR>();

                if(gmr != null)
                {
                    gmr.Points();
                }
            }

            

        }
        Destroy(gameObject);
    }




}
