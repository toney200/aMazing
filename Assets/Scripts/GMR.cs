using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.SearchService;


using UnityEngine.SceneManagement;

public class GMR : MonoBehaviour
{
    
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI timeText;
    public int points;
    public float time = 120;
    public float timeleft;

    private void Start()
    {
        points = PlayerPrefs.GetInt("Points", 0);
        UpdatePointsText(); 
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            PlayerPrefs.Save();
            SceneManager.LoadScene("Game Over");
        }
        Timer();
    }
    public void Points()
    {
        
        points += 10;
        UpdatePointsText();

        PlayerPrefs.SetInt("Points", points);
        PlayerPrefs.Save();
        
        
    }

    void Timer()
    {
        if(time > 0 )
        {
            time -= 1f * Time.deltaTime;
            timeText.text = "Time Left: " + time.ToString("0");
        }
        else
        {
            SceneManager.LoadScene("Game Over");
        }
        

    }

    void UpdatePointsText()
    {
        pointsText.text = "Points: " + points.ToString("0");
    }

    



}
