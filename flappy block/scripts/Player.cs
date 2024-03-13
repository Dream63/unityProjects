using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public Rigidbody2D rb;
    public float speed = 5;
    public float jump_forse = 10;
    public float stop_forse = 5;
    bool godmode = false;
    public static bool isAlive = true;

    [Header("Sprite changes")]
    public SpriteRenderer sr;
    public Sprite sprite1, sprite2;
    bool characterToLeft = true;

    private void Update()
    {
        // Control
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            if (!characterToLeft)
            {
                characterToLeft = true;
                sr.sprite = sprite1;
            }
        }
        if (Input.GetKey(KeyCode.A)) {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            if (characterToLeft)
            {
                characterToLeft = false;
                sr.sprite = sprite2;
            }
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

        }
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isAlive)
        {  
            rb.velocity = new Vector2(rb.velocity.x, jump_forse);
            AudioManager.instance.JumpSound();
        }

        // Xtra
        if (Input.GetKeyDown(KeyCode.G))
        {
            godmode = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.Lost();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isAlive)
        {
            GameManager.instance.LoadMenu();
        }


        // Air resistance
        if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x -speed * Time.deltaTime * stop_forse, rb.velocity.y);
        }
        if (rb.velocity.x < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x + speed * Time.deltaTime * stop_forse, rb.velocity.y);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("kill") && !godmode)
        {
            AudioManager.instance.DeathSound();
            GameManager.instance.Lost();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PipeMove>(out _) && isAlive && !GameManager.startInput)
        {
            ScoreManager.Instance.SetScore(1);
            AudioManager.instance.PointSound();
        }
    }
}
