using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    private Vector3 movement;
    private Transform lastPulpitTouched;

    private void Start()
    {
        // Fetching speed strictly from the JSON GameManager
        speed = GameManager.Instance.ConfigData.player_data.speed;
        
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Capture input exactly when the player presses it
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movement = new Vector3(h, 0, v).normalized;

        // Fall detection
        if (transform.position.y < -2f)
        {
            // I will uncomment this after building the Game Over UI!
            // GameManager.Instance.TriggerGameOver();
            Debug.Log("Doofus fell! Game Over.");
        }
    }

    private void FixedUpdate()
    {
        //If Doofus slips off the edge (Y position drops below 0.6), 
        //disable horizontal movement.
        if (transform.position.y < 0.6f) 
        {
            return;
        }

        Vector3 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}