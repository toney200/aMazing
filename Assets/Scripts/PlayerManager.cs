using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    [SerializeField] private Collider playerCollider;
    
    private String[] powers = { "speed", "ghosting", "invisibility" };

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
                speedBoosting = true;
                SpeedBoosting();
                break;

            case 1:
                isGhosting = true;
                Ghosting();
                break;

            case 2:
                isInvisible = true;
                SelfInvisibility();
                break;

            default:
                Debug.Log("powerSelect drew a value not present in the powers array");
                break;
        }
        
    }
    
    private IEnumerator SpeedBoosting(){
        Debug.Log("Speed boosted active");  // yes, I'm aware debug.log is a shit way of debugging. Leave me alone
        if (speedBoosting){
            playerMovement.speed *= 2;
            yield return new WaitForSeconds(speedBoostDuration);
            playerMovement.speed /= 2;
        }
        speedBoosting = false;
    
    }
    

    private IEnumerator Ghosting(){
        if (isGhosting){
            foreach (GameObject ga in walls){
                ga.GetComponent<Collider>();
                Physics.IgnoreCollision(playerCollider, ga.GetComponent<Collider>(), true);
            }
        }

        Debug.Log("Ghosting");

        yield return new WaitForSeconds(ghostingDuration);
        isGhosting = false;
       
    }

    private IEnumerator SelfInvisibility()
    {
        if (isInvisible){
            bodyMeshRenderer.enabled = false;
            yield return new WaitForSeconds(invisDuration);
            bodyMeshRenderer.enabled = true;
        }
    }
}
