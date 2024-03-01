using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        int whoWon = -1;
        if(other.GetComponent<Collider>().tag == "Player")
        {
            other.GetComponent<PlayerManager>().playerScore++;
            if(other.name.Contains("Blue"))
            {
                whoWon = 1;
            } else if(other.name.Contains("Green"))
            {
                whoWon = 2;
            } else if(other.name.Contains("Yellow"))
            {
                whoWon = 3;
            }
            gameObject.transform.parent.GetComponent<MazeGeneratorInstant>().NewRound(whoWon);
        }
    }
}