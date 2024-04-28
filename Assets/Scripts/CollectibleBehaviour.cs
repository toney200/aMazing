using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehaviour : MonoBehaviour
{
    private GameObject parent;
    private void Start()
    {
        parent = transform.parent.gameObject;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.tag == "Player")
            {
                parent.GetComponent<SingleplayerGenerator>().IncreaseScore();
                parent.GetComponent<SingleplayerGenerator>().goalSFX.Play();
                Destroy(gameObject);
            }
        }
        catch
        {
            Destroy(gameObject);
        }

    }
}
