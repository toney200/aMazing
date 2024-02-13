using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Collider playerCollider;
    
    // Ghosting-specific variables
    private bool isGhosting = false;
    private float ghostingDuration = 3f;

    // Speed boost variables
    private bool speedBoosting = false;
    private float speedBoostDuration = 5f;
    private float boostTimer = 0;

    // Start is called before the first frame update
    void Start(){
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Speed Boost")){
            speedBoosting = true;
            SpeedBoosting();
        }

        if (other.CompareTag("Ghost")){
            isGhosting = true;
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
    }
    
    private void Ghosting(){
        if (!isGhosting){
            Physics.IgnoreCollision(playerCollider, true);
        }
    }

}
