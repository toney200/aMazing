using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agroZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Collectable"))
        {
            Debug.Log("Collided with Collectable");
        }
    }
}
