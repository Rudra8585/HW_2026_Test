using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    private Vector3 movement;

    private void Start()
    {
        // Fetching speed strictly from the JSON GameManager
        speed = GameManager.Instance.ConfigData.player_data.speed;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Stops processing input if the game is over or on start menu
        if(!GameManager.Instance.isGameActive) return;

        // Capture input exactly when the player presses it
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movement = new Vector3(h, 0, v).normalized;

        // Fall detection
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

        //If Doofus slips off the edge (Y position drops below 0.6), 
        //disable horizontal movement.
        if (transform.position.y < 0.6f) 
        {
            return;
        }

        Vector3 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;
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
}