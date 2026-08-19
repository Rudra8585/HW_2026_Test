using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;

    private void Start()
    {
        // Fetching speed strictly from the JSON GameManager
        speed = GameManager.Instance.ConfigData.player_data.speed;
    }

    private void Update()
    {
        // Input controlls (WASD & ArrowKeys)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(h, 0, v).normalized;
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}