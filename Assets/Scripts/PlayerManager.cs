using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement; 
    private Player2Movement player2Movement;
    private Player3Movement player3Movement;
    public TextMeshProUGUI textCounter;
    public Sprite[] powerIcons;
    public GameObject icon;

    private UnityEngine.UI.Image pwImage;
    
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    [SerializeField] private Collider playerCollider;

    public float counter = 5; //Power up timer
    private bool keepingPowerUp = false; //if player has power up and has not used it
    public int playerScore = 0; //the player's score
   

    // Power-Up variables
    private int powers = 3;             // represents the number of power-ups currently in the game
    public bool hasPowerUp = false;        // denotes whether a player has a power-up ability
    public int powerSelect = 0;            // represents the power-up to be chosen at runtime 
    public float powerUpTimer = 5f;       // indicates the length of time between the expiry of one power-up and the acquisition of another

    

    // Ghosting-specific variables
    //[SerializeField] private GameObject[] walls = {};
    private bool isGhosting = false;
    private float ghostingDuration = 3f;

    // Speed-boost  variables
    private bool speedBoosting = false;
    private float speedBoostDuration = 5f;
    private float boostTimer = 0;

    // Self-invis variables
    private bool isInvisible = false;
    private float invisDuration = 6f;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>(); ;
        pwImage = icon.GetComponent<UnityEngine.UI.Image>();
       
        name = gameObject.name;

        if (gameObject.name.Contains("Blue Player"))
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        else if (gameObject.name.Contains("Green Player"))
        {
            player2Movement = GetComponent<Player2Movement>();
        }
        else if (gameObject.name.Contains("Yellow Player"))
        {
            player3Movement = GetComponent<Player3Movement>();
        }
    }


    // Update is called once per frame
    void Update(){
        if (!hasPowerUp) {
            if (counter <= 0)
            {
                hasPowerUp=true;
                textCounter.text = "";
                icon.SetActive(true);
            }
            else
            {
                counter -= 1 * Time.deltaTime;
                textCounter.text = counter.ToString("0");
            }
        }
        if (hasPowerUp)
        {
            if (!keepingPowerUp)
            {
                powerSelect = EnablePowerUp();
            }
        }
    }


    private void OnTriggerEnter(Collider other){

    }

    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Walls" && isGhosting == true)
        {
            Physics.IgnoreCollision(playerCollider, collision.gameObject.GetComponent<Collider>(), true);
        }
    }


    /*
     * Waits for a number of seconds equal to powerUpTimer before calling the method EnablePowerUp(). To be called when an instance's hasPowerUp is set to false by any means. 
     */
    private IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(powerUpTimer);
        hasPowerUp = true;
    }


    /*
     * Provides functionality to the PowerUp action within the playerInput action map. On activation, starts the coroutine of the relevant power-up based on the chosen power up selected by 
     * EnablePowerUp() utilising the integer, powerSelect, as the means to select a coroutine.
     */
    public void OnPowerUp()
    {
        Debug.Log("Power-Up button pressed; hasPowerUp = " + hasPowerUp);
        if (hasPowerUp)
        {  
            switch (powerSelect)
            {
                case 1:                 
                        speedBoosting = true;
                        StartCoroutine(SpeedBoosting());       
                    break;

                case 2:                                     
                        isGhosting = true;
                        StartCoroutine(Ghosting());                    
                    break;

                case 3:                                     
                        isInvisible = true;
                        StartCoroutine(SelfInvisibility());                  
                    break;

                default:
                    Debug.Log("powerSelect drew a value that does not have a corresponsing power-up");
                    break;
            }
        }


    }


    /*
     * Returns a random integer value representing the power to be selected. Sets the corresponding power-up boolean to true to enable to corresponding power-up methods.
     */
    private int EnablePowerUp(){
        
        powerSelect = Mathf.RoundToInt(UnityEngine.Random.Range(1, powers+1));
        pwImage.sprite = powerIcons[powerSelect - 1];
        switch (powerSelect)
        {
            case 1:               
                speedBoosting = true;
                break;

            case 2:              
                isGhosting = true;  
                break;

            case 3:                
                isInvisible = true;              
                break;

            default:
                Debug.Log("Death at powerSelect. See EnablePowerUp() :(");
                break;
        }
        hasPowerUp = true;
        keepingPowerUp = true;
        return powerSelect;
    }
    
    /*
     * Doubles the speed of the instance of the object that has this script attached to it. Once the speed is increased and subsequently decreased after the speedBoostDuration, then 
     * the speedBoosting and hasPowerUp booleans are set to false, enabling the player to make use of power-ups once again. 
     */
    private IEnumerator SpeedBoosting(){

        if (gameObject.name.Contains("Blue Player"))
        {
            Debug.Log("Pressed blue speed");
            playerMovement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            playerMovement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
            counter = powerUpTimer;
        }

        if(gameObject.name.Contains("Green Player"))
        {
            player2Movement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            player2Movement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
            counter = powerUpTimer;
        }

        if (gameObject.name.Contains("Yellow Player"))
        {
            player3Movement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            player3Movement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
            counter = powerUpTimer;
        }
        keepingPowerUp = false;
    }
    
    /*
     * Ignores collisions of the player and all instances of the maze wall prefab for the length of ghostingDuration. Afterwards the isGhosting and hasPowerUp values are set to false so 
     * as to enable power-up selection once more.
     */
    private IEnumerator Ghosting(){

        /*if (isGhosting)
        {
            foreach (GameObject ga in walls)
            {
                Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), true);
            }
        }
        */
        Debug.Log("Ghosting");

        yield return new WaitForSeconds(ghostingDuration);
        isGhosting = false;
        /*
         * foreach (GameObject ga in walls)
        {
            Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), false);
        }
        hasPowerUp = false;
       */
        counter = 5;
        keepingPowerUp = false;
    }


    /*
     * Disables the Mesh Renderer inserted into the SerializedField at the beginning of the class. After execution isInvisible and hasPowerUp are disabled to enable other methods to function.
     */
    private IEnumerator SelfInvisibility()
    {

        bodyMeshRenderer.enabled = false;
        yield return new WaitForSeconds(invisDuration);
        bodyMeshRenderer.enabled = true;
        isInvisible = false;
        hasPowerUp = false;
        keepingPowerUp = false;
        counter = 5;
    }
}