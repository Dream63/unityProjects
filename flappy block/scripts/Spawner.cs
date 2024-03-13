using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    public GameObject pipePrefab; 
    public static int speed = 5;
    public int difficultySpeed = 50; 
    public float timeToSpawn = 3f; 
    public float minY = -16, maxY = 16; 
    private float timer;
    public static float timeTillPipeDestroyed = 6;

    public void Start()
    {
        timer = 0;
    }
    private void Update()
    {
        if (timer < 0) 
        {
            timer = timeToSpawn;
            GameObject pipe = Instantiate(pipePrefab, transform.position, Quaternion.identity);
            float rand = Random.Range(minY, maxY)/10;
            pipe.transform.position = new Vector2(pipe.transform.position.x, rand);
        }
        else
            timer -= Time.deltaTime;

        if (timeToSpawn > 1)
            timeToSpawn -= Time.deltaTime/difficultySpeed;
    }
}
