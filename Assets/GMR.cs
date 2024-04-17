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
       
    }

    private void Update()
    {
        Timer();
    }
    public void Points()
    {
        points += 10;
        pointsText.text ="Points: " + points.ToString("0");
    }
    IEnumerator RoundTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene("Game Over");
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
            //SceneManager.LoadScene("Game Over");
        }
        
    }

}
