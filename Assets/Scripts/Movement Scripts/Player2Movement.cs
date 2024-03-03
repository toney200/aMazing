using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Movement : MonoBehaviour
{
    Animator animator;
    PlayerHandler input;
    Vector2 currentmovement;
    

    bool movementPressed;
    int isRunningHash;

    public float speed = 1;


    void Awake()
    {
        input = new PlayerHandler();

        //Set player input values using listeners.
        input.Player2.Movement.performed += ctx =>
        {
            currentmovement = ctx.ReadValue<Vector2>();
            movementPressed = currentmovement.x != 0 || currentmovement.y != 0;
        };

        input.Player2.PowerUp.performed += ctx =>
        {
            Debug.Log(" shoulder button pressed");
            
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
        if (movementPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        //Stop moving
        if (!movementPressed && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        //calculate Movement 
        Vector3 movementDirection = new Vector3(currentmovement.x, 0, currentmovement.y);

        //Apply speed to the movement
        Vector3 movement = movementDirection * speed * Time.deltaTime;

        // Move the Player
        transform.position += movement;

    }

    void OnEnable()
    {
        //Enable action map
        input.Player2.Enable();
    }

    void OnDisable()
    {
        //Disabel action map 
        input.Player2.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx) => currentmovement = ctx.ReadValue<Vector2>();
}
