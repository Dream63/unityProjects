using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    public GameObject loseWindow;
    public GameObject startText;
    public static bool startInput = true;


    [Header("Borders")]
    public Camera cam;
    public GameObject leftWall, rightWall;

    public void Lost()
    {
        Player.isAlive = false;
        ScoreManager.Instance.BestScore();
        loseWindow.SetActive(true);
        Time.timeScale = 0;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        Player.isAlive = false;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        Player.isAlive = true;
        Time.timeScale = 0;
        startInput = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        startText.SetActive(true);
        instance = this;

        Vector2 leftCorner = cam.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 rightCorner = cam.ScreenToWorldPoint(new Vector2(Screen.width, 0));
        leftWall.transform.position = new Vector2(leftCorner.x - 0.5f, 0);
        rightWall.transform.position = new Vector2(rightCorner.x + 0.5f, 0);
    }

    [System.Obsolete]
    public void Update()
    {
            // Start text
        if (startInput && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetMouseButtonDown(0)))
        {   
            startInput = false;
            Time.timeScale = 1;
            startText.SetActive(false);
        }

            // End screen skip by button
        if (loseWindow.active && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W)))
        {
            LoadGame();
        }
    }
}
