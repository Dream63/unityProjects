using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    public static TopPanelManager instance;

    public GameObject smileyFace;
    public TextMeshProUGUI minesCounterText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI minesCounterTextBg;
    public TextMeshProUGUI timerTextBg;

    private int minesDisplayed;
    private int minesTotal = 0;
    private float timer = 0;
    private int timerDisplayed;

    Image smileyFaceSpriteRenderer;
    public Sprite smileyFaceDefault;
    public Sprite smileyFaceActive;
    public Sprite smileyFaceDead;
    public Sprite smileyFaceCool;

    void Awake()
    {
        instance = this;
        GameManager.instance.GetGameValues(out _, out _, out int minesPlaced, out int flagsPlaced, out _);
        SetBasics(minesPlaced, flagsPlaced);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (Mathf.FloorToInt(timer) > timerDisplayed && !(GameManager.instance.playerLost || GameManager.instance.playerWon))
        {
            timerDisplayed = Mathf.FloorToInt(timer);
            timerText.text = timerDisplayed.ToString();
        }

        Sprite currentSmileySprite = smileyFaceSpriteRenderer.sprite;

        // Sprite change on clicks
        if (!GameManager.instance.playerLost && !GameManager.instance.playerWon)
        {
            if (currentSmileySprite != smileyFaceActive && Input.anyKey)
                smileyFaceSpriteRenderer.sprite = smileyFaceActive;
            if (currentSmileySprite != smileyFaceDefault && !Input.anyKey)
                smileyFaceSpriteRenderer.sprite = smileyFaceDefault;
        }

        // Sprite change on death
        if (GameManager.instance.playerLost && currentSmileySprite != smileyFaceDead) 
        {
            smileyFaceSpriteRenderer.sprite = smileyFaceDead;
        }

        // Sprite change on win
        if (GameManager.instance.playerWon && currentSmileySprite != smileyFaceCool)
        {
            smileyFaceSpriteRenderer.sprite = smileyFaceCool;
        }
    }

    public void SetBasics(int minesTotal, int flagsPlaced)
    {
        this.minesTotal = minesTotal;
        minesDisplayed = minesTotal;

        smileyFaceSpriteRenderer = smileyFace.GetComponent<Image>();

        SetTimer(GameManager.instance.startTimer);
        MinesCountUpdate(flagsPlaced);
    }
    public void MinesCountUpdate(int flagsPlaced)
    {
        minesDisplayed = Mathf.Clamp(Mathf.FloorToInt(minesTotal - flagsPlaced), 0, 99999);
        minesCounterText.text = minesDisplayed.ToString();
    }
    public void SetTimer()
    {
        timer = 0;
        timerDisplayed = 0;
        timerText.text = timerDisplayed.ToString();
    }
    public void SetTimer(float time)
    {
        timer = time;
        timerDisplayed = Mathf.Clamp(Mathf.FloorToInt(time), 0, 99999);
        timerText.text = timerDisplayed.ToString();
    }
    public void RestartGame()
    {
        GameManager.instance.CreateTileMap();
        GameManager.instance.GetGameValues(out _, out _, out int minesPlaced, out int flagsPlaced, out _);
        SetBasics(minesPlaced, flagsPlaced);

    }
    public float GetTimer()
    {
        return timer;
    }
}
