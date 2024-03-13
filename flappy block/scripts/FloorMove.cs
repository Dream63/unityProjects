using UnityEngine;

public class FloorMove : MonoBehaviour
{
    [Header("Objects")]

    public GameObject floor;
    public GameObject floor1;
    public GameObject bg, bg1;
    bool floorToMove = true;
    bool bgToMove = false;
    float speed = Spawner.speed;

    [Header("Size")]
    public float floorSize = 30;
    public float bgSize = 27;
    float timeTillMove;
    float timerMove;
    float timeTillMoveBg;
    float timerMoveBg;

    [Header("Bg Speed")]
    public float bgSpeedKoef = 2;

    private void Start()
    {
        timeTillMove = floorSize / speed;
        timerMove = timeTillMove;
        timeTillMoveBg = timeTillMove * bgSpeedKoef / floorSize * bgSize;
        timerMoveBg = timeTillMoveBg;
    }
    void Update()
    {
        // moves floor and bg if its asighned
        if (floor)
        {
            floor.transform.Translate(speed * Time.deltaTime * Vector2.left);
            floor1.transform.Translate(speed * Time.deltaTime * Vector2.left);
        }
        if (bg)
        {
            bg1.transform.Translate(speed / bgSpeedKoef * Time.deltaTime * Vector2.left);
            bg.transform.Translate(speed / bgSpeedKoef * Time.deltaTime * Vector2.left);
        }

        if (timerMove < 0)
        {
            if (floorToMove)
            {
                floorToMove = false;
                floor.transform.position = new Vector2(floor.transform.position.x + floorSize * 2, floor.transform.position.y);
            }
             else if (!floorToMove)
            {
                floorToMove = true;
                floor1.transform.position = new Vector2(floor1.transform.position.x + floorSize * 2, floor1.transform.position.y);
            }
            timerMove = timeTillMove;
        }
        // timer goes only if floor asighned
        else if (floor)
        {
            timerMove -= Time.deltaTime;
        }

        if (timerMoveBg < 0)
        {
            if (bgToMove)
            {
                bgToMove = false;
                bg.transform.position = new Vector2(bg.transform.position.x + bgSize * 2, bg.transform.position.y);
            }
            else if (!bgToMove)
            {
                bgToMove = true;
                bg1.transform.position = new Vector2(bg1.transform.position.x + bgSize * 2, bg1.transform.position.y);
            }
            timerMoveBg = timeTillMoveBg;
        }
        // timer goes only if bg asighned
        else if (bg)
        {
            timerMoveBg -= Time.deltaTime;
        }
    }
}
