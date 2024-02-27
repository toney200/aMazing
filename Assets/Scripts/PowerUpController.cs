using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public abstract class PowerUp
{
    public string Name;
    public Sprite icon;

    public abstract void Action();
}
public class PowerUpController : MonoBehaviour
{
    public float counter = 5;
    public TextMeshProUGUI counterText;
    public GameObject pwIcon;
    public PowerUp[] pwArray;

    private bool hasPowerUp = false;
    // Start is called before the first frame update
    void Start()
    {
        counterText.text = counter.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasPowerUp)
        {
            counter -= 1 * Time.deltaTime;
            if (counter <= 0)
            {
                counter = 0;
            }
            counterText.text = counter.ToString("0");

            if (counter == 0)
            {
                ChoosePowerUp();
                hasPowerUp = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                pwIcon.SetActive(false);
                counter = 5;
                counterText.text = counter.ToString("0");
                hasPowerUp = false;
            }
        }
    }

    private void ChoosePowerUp()
    {
        //PowerUp randomPowerUp = pwArray[Mathf.RoundToInt(UnityEngine.Random.Range(0.0f, (pwArray.Length - 1)))];
        counterText.text = "";
        pwIcon.SetActive(true);
    }
}
