using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Collider>().tag == "Player")
        {
            RoundFinished();
        }
    }

    private void RoundFinished()
    {
        Transform parent = gameObject.transform.parent;
        parent.GetComponent<MazeGeneratorInstant>().NewRound();
    }
}