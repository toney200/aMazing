using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VictoryScreen : MonoBehaviour
{
    private int blueScore;
    private int greenScore;
    private int yellowScore;

    public GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        blueScore = PlayerPrefs.GetInt("Blue Score", 0);
        greenScore = PlayerPrefs.GetInt("Green Score", 0);
        yellowScore = PlayerPrefs.GetInt("Yellow Score", 0);

      

        if (blueScore == 5) { 
            
            gameObjects[0].SetActive(true);
            gameObjects[0].GetComponent<Animator>().Play("Dance");
        }
        else if(greenScore == 5)
        {
            gameObjects[1].SetActive(true);
            gameObjects[1].GetComponent<Animator>().Play("Dance");
        }
        else if (yellowScore == 5)
        {
            gameObjects[2].SetActive(true);
            gameObjects[2].GetComponent<Animator>().Play("Dance");
        }

        PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
