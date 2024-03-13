using UnityEngine;

public class PipeMove : MonoBehaviour
{
    public GameObject pipe;
    public float speed = Spawner.speed;
    public float pipeLeftTime = Spawner.timeTillPipeDestroyed;

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.left);

        if (pipeLeftTime < 0)
            Destroy(pipe);
        else
            pipeLeftTime -= Time.deltaTime;
    }
}
