using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public int finalScore;
    public int highScore;
     

    // Start is called before the first frame update
    void Start()
    {
        finalScore = PlayerPrefs.GetInt("Points", 0);
        finalScoreText.text = "Your Score: " + finalScore.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        

    }

  
}
