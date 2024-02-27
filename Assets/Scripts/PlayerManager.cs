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
    

    private int powers = 3;

    private bool hasPowerUp = false;

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

        //InvokeRepeating("EnablePowerUp", 5f, 20f);
    }

    // Update is called once per frame
    void Update(){
        hasPowerUp = true;
    }

    private void OnTriggerEnter(Collider other){

    }

    private void OnCollisionEnter(Collision collision){
        
    }

    private void OnPowerUp()
    {
        Debug.Log("Button pressed");
        if (hasPowerUp)
        {
            int powerSelect = Mathf.RoundToInt(UnityEngine.Random.Range(1, powers));
            
            switch (powerSelect)
            {
                case 1:
                    
                    
                        speedBoosting = true;
                        StartCoroutine(SpeedBoosting());
                    
                    break;

                case 2:
                  
                    
                        isGhosting = true;
                        StartCoroutine( Ghosting());
                    
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

    
    private void EnablePowerUp(){
        
        int powerSelect = Mathf.RoundToInt(UnityEngine.Random.Range(1, powers));
        
        switch (powerSelect)
        {
            case 1:
                if (hasPowerUp == false)
                {
                    speedBoosting = true;
                    //SpeedBoosting();
                }
                break;

            case 2:
                if (hasPowerUp == false)
                {
                    isGhosting = true;
                    //Ghosting();
                }
                break;

            case 3:
                if (hasPowerUp == false)
                {
                    isInvisible = true;
                    //SelfInvisibility();
                }
                break;

            default:
                Debug.Log("powerSelect drew a value not present in the powers array");
                break;
        }
        
    }
    
    
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
    }
    

    private IEnumerator Ghosting(){
        
        foreach (GameObject ga in walls){
            Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), true);
        }
        

        Debug.Log("Ghosting");

        yield return new WaitForSeconds(ghostingDuration);
        isGhosting = false;
        hasPowerUp=false;
       
    }

    private IEnumerator SelfInvisibility()
    {
        bodyMeshRenderer.enabled = false;
        yield return new WaitForSeconds(invisDuration);
        bodyMeshRenderer.enabled = true;
        isInvisible = false;
        hasPowerUp = false;
    }
}
