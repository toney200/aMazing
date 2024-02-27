using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement; 
    
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private PlayerInput playerInput;

    private String[] powers = { "speed", "ghosting", "invisibility" };

    private bool hasPowerUp = false;

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

        InvokeRepeating("EnablePowerUp", 5f, 20f);
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void OnTriggerEnter(Collider other){

    }

    private void OnCollisionEnter(Collision collision){
        
    }

    private IEnumerator EnablePowerUp(){
        yield return new WaitForSeconds(1f);
        int powerSelect = Mathf.RoundToInt(UnityEngine.Random.Range(0, powers.Length-1));
        switch (powerSelect)
        {
            case 0:
                if (hasPowerUp == false)
                {
                    speedBoosting = true;
                    //SpeedBoosting();
                }
                break;

            case 1:
                if (hasPowerUp == false)
                {
                    isGhosting = true;
                    //Ghosting();
                }
                break;

            case 2:
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

       
        playerMovement.speed *= 2;
        yield return new WaitForSeconds(speedBoostDuration);
        playerMovement.speed /= 2;
      
        speedBoosting = false;
        hasPowerUp = false;
    
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
        hasPowerUp = false;
    }
}
