using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement; 
    private Player2Movement player2Movement;
    private Player3Movement player3Movement;
    
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    [SerializeField] private Collider playerCollider;

    // Power-Up variables
    private int powers = 3;             // represents the number of power-ups currently in the game
    private int powerSelect = 0;            // represents the power-up to be chosen at runtime 
    private bool hasPowerUp = false;        // denotes whether a player has a power-up ability
    private float powerUpTimer = 10f;       // indicates the length of time between the expiry of one power-up and the acquisition of another

    public String name;

    // Ghosting-specific variables
    [SerializeField] private GameObject[] walls = {};
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
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>(); ;
       
        name = gameObject.name;

        hasPowerUp = true;
    }


    // Update is called once per frame
    void Update(){
        if (!hasPowerUp) { 
            StartCoroutine(PowerUpTimer());
        }
        if (hasPowerUp)
        {
            powerSelect = EnablePowerUp();
        }
    }


    private void OnTriggerEnter(Collider other){

    }


    private void OnCollisionEnter(Collision collision){
        
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
    private void OnPowerUp()
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
        
        switch (powerSelect)
        {
            case 1:               
                    speedBoosting = true;
                    return 1;
                //break;

            case 2:              
                    isGhosting = true;
                    return 2;
                
                //break;

            case 3:                
                    isInvisible = true;
                    return 3;                
               // break;

            default:
                Debug.Log("Death at powerSelect. See EnablePowerUp() :(");
                break;
        }
        hasPowerUp = true;
        return powerSelect;
    }
    
    /*
     * Doubles the speed of the instance of the object that has this script attached to it. Once the speed is increased and subsequently decreased after the speedBoostDuration, then 
     * the speedBoosting and hasPowerUp booleans are set to false, enabling the player to make use of power-ups once again. 
     */
    private IEnumerator SpeedBoosting(){

        if (gameObject.name == "Blue Player")
        {
            Debug.Log("Pressed blue speed");
            playerMovement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            playerMovement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
        }

        if(gameObject.name == "Green Player")
        {
            player2Movement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            playerMovement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
        }

        if (gameObject.name == "Yellow Player")
        {
            player3Movement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            player3Movement.speed /= 2;

            speedBoosting = false;
            hasPowerUp = false;
        }
    }
    
    /*
     * Ignores collisions of the player and all instances of the maze wall prefab for the length of ghostingDuration. Afterwards the isGhosting and hasPowerUp values are set to false so 
     * as to enable power-up selection once more.
     */
    private IEnumerator Ghosting(){

        if (isGhosting)
        {
            foreach (GameObject ga in walls)
            {
                Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), true);
            }
        }

        Debug.Log("Ghosting");

        yield return new WaitForSeconds(ghostingDuration);
        isGhosting = false;
        foreach (GameObject ga in walls)
        {
            Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), false);
        }
        hasPowerUp = false;
       
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
    }
}
