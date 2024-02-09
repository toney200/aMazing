using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    PlayerInput input;
    Vector3 currentmovement;

    
    bool movementPressed;
    int isRunningHash;

    void Awake()
    {
        input = new PlayerInput();

        //Set player input values using listeners.
        input.CharacterControls.Movement.performed += ctx =>
        {
            currentmovement = ctx.ReadValue<Vector2>();
            movementPressed = currentmovement.x != 0 || currentmovement.y != 0;
        };

        

    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        rotation();
    }

    void rotation()
    {
        //Current Position of the Player. 
        Vector3 currentPosition = transform.position;

        //The change in position our character should point to.
        Vector3 newPosition = new Vector3(currentmovement.x, 0, currentmovement.y);

        //Combine positions to give a position to look at.
        Vector3 positionToLookAt = currentPosition + newPosition;

        transform.LookAt(positionToLookAt);
    }

    void movement()
    {
        bool isRunning = animator.GetBool(isRunningHash);

        //Start moving 
        if(movementPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        //Stop moving
        if (!movementPressed && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

    }

    void OnEnable()
    {
        //Enable action map
        input.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //Disabel action map 
        input.CharacterControls.Disable();
    }

}
