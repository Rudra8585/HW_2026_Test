using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    private Vector3 movement;

    //My character's animator and how fast he spins around
    public Animator characterAnimator;
    public float turnSpeed = 15f;

    //Audio variables for footsteps and landing
    public AudioSource audioSource;
    public AudioClip[] stepSounds;

    [Header("Coyote Time")]
    //The grace period allowed after slipping off an edge
    public float coyoteTimeDuration = 0.2f; 
    private float coyoteTimeCounter;

    private void Start()
    {
        //Fetching speed strictly from the JSON GameManager
        speed = GameManager.Instance.ConfigData.player_data.speed;

        rb = GetComponent<Rigidbody>();

        //This prevents the player from falling at the start of the game
        rb.useGravity = false;
    }

    private void Update()
    {
        //Stops processing input if the game is over or on start menu
        if(!GameManager.Instance.isGameActive)
        {
            if(rb.useGravity)
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;

                //Stopping the animations when the game ends
                if (characterAnimator != null)
                {
                    characterAnimator.speed = 0f;
                }
            }
            return;
        }

        //Turns gravity back on when the game starts
        if(!rb.useGravity)
        {
            rb.useGravity = true;

            if (characterAnimator != null)
            {
                characterAnimator.speed = 1f;

                //This will force the spawn animation to play from the very beginning
                characterAnimator.Play("Spawn_Air", -1, 0f);
            }
        }

        //Capture input exactly when the player presses it
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movement = new Vector3(h, 0, v).normalized;

        //Updating the animator variables and rotation
        if (characterAnimator != null)
        {
            //Passing the movement magnitude to trigger the walk animation
            characterAnimator.SetFloat("Speed", movement.magnitude);
            
            //Only trigger the fall animation if he slips below the top of the pulpit
            bool hasFallenOff = transform.position.y < 0.4f;
            characterAnimator.SetBool("IsFalling", hasFallenOff);

            //Making the character actually look where he is going
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                characterAnimator.transform.rotation = Quaternion.Slerp(characterAnimator.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        //Fall detection
        if (transform.position.y < -2f)
        {
            GameManager.Instance.TriggerGameOver();
            Debug.Log("Doofus fell! Game Over.");
        }
    }

    private void FixedUpdate()
    {
        //Stops processing physics movement if the game is over or on start menu
        if(!GameManager.Instance.isGameActive) return;

        //Ground check using a raycast
        float laserLength = 0.6f;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, laserLength);

        //Coyote time logic
        if (isGrounded) 
        {
            //Reset the timer if standing on solid ground
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else 
        {
            //Start ticking down if the player walked off the edge
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        //Only completely freeze his horizontal movement if he has fully fallen to his death
        if (transform.position.y < 0f) 
        {
            return;
        }

        //Calculate the next step
        Vector3 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;

        //Floating logic
        //Allow the player to walk on air if the Coyote timer is greater than 0
        if (!isGrounded && coyoteTimeCounter > 0f)
        {
            //Lift the player slightly to clear the sharp edge
            newPosition.y = 0.75f; 
            
            //Kill any downward falling momentum while floating
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }

        //Apply movement
        rb.MovePosition(newPosition);
    }

    //Calling the AddScore function on collision with a new pulpit
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Pulpit"))
        {   
            //This grabs the pulpit script from the pulpit we just hit
            Pulpit pulpitScript = collision.gameObject.GetComponent<Pulpit>();

            //If it hasnt been scored yet, a point is added
            if(pulpitScript != null && !pulpitScript.hasBeenScored)
            {
                pulpitScript.hasBeenScored = true;
                GameManager.Instance.AddScore();
            }
        }
    }

    //This plays a random step sound
    public void PlayStepSound()
    {
        if (stepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, stepSounds.Length);
            audioSource.PlayOneShot(stepSounds[randomIndex], 0.08f);
        }
    }

    //This plays a random land sound
    public void PlayLandSound()
    {
        if (stepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, stepSounds.Length);
            audioSource.PlayOneShot(stepSounds[randomIndex], 0.2f);
        }
    }
}