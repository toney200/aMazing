using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private Collider playerCollider;
    
    // Ghosting-specific variables
    [SerializeField] private GameObject[] walls = {};
    private String[] powers = {"speed", "ghost"};
    private bool isGhosting = false;
    private float ghostingDuration = 3f;

    // Speed boost variables
    private bool speedBoosting = false;
    private float speedBoostDuration = 5f;
    private float boostTimer = 0;

    // Start is called before the first frame update
    IEnumerator Start(){
        playerMovement = GetComponent<PlayerMovement>();
        //playerCollider = GetComponent<Collider>();

        yield return EnablePowerUp();
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void OnTriggerEnter(Collider other){
        /*
        if (other.CompareTag("Speed Boost")){
            speedBoosting = true;
            SpeedBoosting();
        }

        if (other.CompareTag("Ghost")){
            isGhosting = true;
        }
        */


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

            default:
                Debug.Log("powerSelect drew a value not present in the powers array");
                break;
        }
        
    }
    
    private void SpeedBoosting(){
        if (speedBoosting){
            boostTimer += Time.deltaTime;
            playerMovement.speed *= 2;
            if (boostTimer > speedBoostDuration){
                playerMovement.speed /= 2;
                speedBoosting = false;
            }
        }

        Debug.Log("Speeding");

        EnablePowerUp();
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
        EnablePowerUp();
    }

}
