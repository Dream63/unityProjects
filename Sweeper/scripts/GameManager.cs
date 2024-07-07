using UnityEngine;
using Util;
using UnityEngine.SceneManagement;
using TMPro;
using static SaveSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header ("Tile map")]
    public TileMap tileMap;
    public TileMapVisual tileMapVisual;


    [Header("Menu objects")]
    public GameObject continueLastGamePopUp;
    public GameObject settingsMenu;
    public TMP_InputField widthInputField, heigthInputField, minesPercentageInputField, tileSizeInputField;
    public GameObject statsMenu;
    public TextMeshProUGUI statsText1, statsText2, pageText;
    private int statsPage = 0;

    [Header("Game audio")]
    public GameObject audioObject;
    public AudioSource winSound, deathSound, clickSound, flagPlaceSound, flagRemoveSound;

    [Header("Misc")]
    public bool AdmPow = false;

    private int heigth, width;
    private float minesPercent;
    private float tileSize;
    private Vector3 gridStartPos = new(0, 0, 1);

    private int boardSize;
    private int numberOfMines;
    private int minesPlaced = 0;
    private int flagsPlaced = 0;
    private bool iMGeneral = false;

    private bool firstClick = true;
    private bool isClickInsideTheGrid;
    private bool timeScaleClick = true;
    private Vector2Int clickedTileXY;

    [System.NonSerialized] public float startTimer;
    [System.NonSerialized] public bool playerLost = false;
    [System.NonSerialized] public bool playerWon = false;

    public static int TotalWins;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(tileMapVisual.gameObject);
        DontDestroyOnLoad(tileMapVisual.meshObject);
        DontDestroyOnLoad(audioObject);

        TotalWins = PlayerPrefs.HasKey("TotalWins") ? PlayerPrefs.GetInt("TotalWins") : 0;
        PlayerPrefs.SetInt("TotalWins", TotalWins);
    }

    void Update()
    {
        // Playing
        if (!playerLost && !playerWon)
        {
            // Clicked tile
            if (Input.anyKey)
            {
                clickedTileXY = tileMap.grid.GetTileXY(tileMap.grid.MouseToWorldPoint());
                isClickInsideTheGrid = tileMap.grid.IsClickInsideThGrid(clickedTileXY);
            }

            // Interaction with grid
            if (isClickInsideTheGrid)
            {
                // Left Click
                if (Input.GetMouseButtonUp(0) && tileMap.grid.GetObject(clickedTileXY).tmS != TileMap.TilemapObject.TilemapSprites.Flag && !tileMap.IsTileOpen(clickedTileXY))
                {
                    if (firstClick)
                        FirstClick(clickedTileXY);

                    if (timeScaleClick)
                    {
                        timeScaleClick = false;
                        Time.timeScale = 1f;
                    }


                    tileMap.grid.GetObject(clickedTileXY).OpenTile();
                    PlaySound(clickSound);
                }

                // Right Click
                if (Input.GetMouseButtonUp(1) && !tileMap.IsTileOpen(clickedTileXY) && !BasicCameraMovement.isDragging)
                {
                    Flag();
                }
            }
            // Debug: AutoWin
            if (Input.GetKeyUp(KeyCode.W) && AdmPow)
            {
                tileMap.AutoWin();
            }
        }

        // Exit
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            LoadMenu(); 
        }
    }

    // Start
    public void StartGame()
    {
        SaveSystem.instance.Init();

        // Popup if save exist
        if (IsSaveExist())
            SetActiveContinueLastGamePopUp(true);

        // Start, if not
        else
        {
            LoadScene(1);
            CreateTileMap();
        }
    }
    public void LoadTileMap()
    {
        SceneManager.LoadScene(1);

        gameObject.SetActive(true);
        tileMapVisual.gameObject.SetActive(true);
        tileMapVisual.meshObject.SetActive(true);

        SaveSystem.instance.LoadTileMap();
        timeScaleClick = true;
    }
    public void CreateTileMap()
    {
        // Activating field
        gameObject.SetActive(true);
        tileMapVisual.gameObject.SetActive(true);
        tileMapVisual.meshObject.SetActive(true);
        
        // Getting values
        GetPrefs(out width, out heigth, out minesPercent, out tileSize);

        // Normalizing values
        heigth = Mathf.Clamp(heigth, 3, 1000);
        width = Mathf.Clamp(width, 3, 1000);
        tileSize = Mathf.Clamp(tileSize, 0.1f, 10);
        boardSize = heigth * width;
        minesPercent = Mathf.Clamp(minesPercent, 1f, 99f);
        numberOfMines = Mathf.Clamp(Mathf.FloorToInt(width * heigth * minesPercent / 100), 1, 999999);

        // Setting starting values
        firstClick = true;
        minesPlaced = 0;
        flagsPlaced = 0;
        tileMapVisual.showMines = false;
        playerLost = false;
        playerWon = false;
        isClickInsideTheGrid = false;

        // Creating new board
        if(true) 
        {
            iMGeneral = minesPercent > 50;

            tileMap = new TileMap(width, heigth, tileSize, gridStartPos, iMGeneral, tileMapVisual);

            // Spawning mines
            if (minesPercent <= 50)
            {
                while (minesPlaced < numberOfMines)
                    PlaceMine(width, heigth);
            }
            if (minesPercent > 50)
            {
                minesPlaced = boardSize;
                while (minesPlaced > numberOfMines)
                    PlaceNotMine(width, heigth);
            }
        }
        timeScaleClick = true;
    }

    // Mine position
    private void PlaceMine(int gridWidth, int gridHeight)
    {
        Vector2Int mineXY;
        int iteration = 0;

        do
        {
            iteration++;
            mineXY = new(Random.Range(0, gridWidth), Random.Range(0, gridHeight));
        } while (tileMap.grid.GetObject(mineXY).iM && iteration < 10000);

        tileMap.grid.GetObject(mineXY).iM = true;
        tileMap.grid.TriggerObjectChanged(mineXY);
        minesPlaced++;
    }
    private void PlaceNotMine(int gridWidth, int gridHeight)
    {
        Vector2Int mineXY;
        int iteration = 0;

        do
        {
            iteration++;
            mineXY = new(Random.Range(0, gridWidth), Random.Range(0, gridHeight));
        } while (!tileMap.grid.GetObject(mineXY).iM && iteration < 10000);

        tileMap.grid.GetObject(mineXY).iM = false;
        tileMap.grid.TriggerObjectChanged(mineXY);
        minesPlaced--;
    }
    private void ChangeSpotForMine(Vector2Int mineXY, int gridWidth, int gridHeight, Vector2Int GreenZone00, Vector2Int GreenZone11)
    {

        Vector2Int mineNewXY;
        bool MineIsOutsideGreenZone;
        int iteration = 0;

        do
        {
            iteration++;
            mineNewXY = new(Random.Range(0, gridWidth), Random.Range(0, gridHeight));
            MineIsOutsideGreenZone = mineNewXY.x < GreenZone00.x || mineNewXY.x > GreenZone11.x || mineNewXY.y < GreenZone00.y || mineNewXY.y > GreenZone11.y;
        } while ((tileMap.grid.GetObject(mineNewXY).iM || !MineIsOutsideGreenZone) && iteration < 10000);

        tileMap.grid.GetObject(mineXY).iM = false;
        tileMap.grid.TriggerObjectChanged(mineXY);
        tileMap.grid.GetObject(mineNewXY).iM = true;
        tileMap.grid.TriggerObjectChanged(mineNewXY);

    }

    // Clicks
    private void FirstClick(Vector2Int clickedTileXY)
    {
        firstClick = false;

        int x = clickedTileXY.x, y = clickedTileXY.y;
        bool LessThan50PercentTilesAreMinesAfterFirstClick = numberOfMines / ((float)boardSize - 9) < 0.5f;
        bool notNullAndIsMine = tileMap.grid.GetObject(clickedTileXY) != null && tileMap.grid.GetObject(clickedTileXY).iM;

        if (LessThan50PercentTilesAreMinesAfterFirstClick)
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    notNullAndIsMine = tileMap.grid.GetObject(new Vector2Int(x + i, y + j)) != null && tileMap.grid.GetObject(new Vector2Int(x + i, y + j)).iM;

                    if (notNullAndIsMine)
                        ChangeSpotForMine(new(clickedTileXY.x + i, clickedTileXY.y + j), width, heigth, new(clickedTileXY.x - 1, clickedTileXY.y - 1), new(clickedTileXY.x + 1, clickedTileXY.y + 1));
                }
        else if (notNullAndIsMine)
            ChangeSpotForMine(clickedTileXY, width, heigth, clickedTileXY, clickedTileXY);
    }
    private void Flag()
    {
        if (tileMap.grid.GetObject(clickedTileXY).tmS == TileMap.TilemapObject.TilemapSprites.UndiscoveredSquare)
        {
            tileMap.SetTileMapSprite(clickedTileXY, TileMap.TilemapObject.TilemapSprites.Flag);
            flagsPlaced++;
            PlaySound(flagPlaceSound);
        }
        else
        {
            tileMap.SetTileMapSprite(clickedTileXY, TileMap.TilemapObject.TilemapSprites.UndiscoveredSquare);
            flagsPlaced--;
            PlaySound(flagRemoveSound);
        }
        TopPanelManager.instance.MinesCountUpdate(flagsPlaced);
    }

    // Game state
    public void Lose()
    {
        Time.timeScale = 0f; 
        tileMapVisual.showMines = true;
        playerLost = true;
        PlaySound(deathSound);
    }
    public void Win()
    {
        TotalWins++;

        PlayerPrefs.SetInt("TotalWins", TotalWins);
        SaveWinResult(width, heigth, minesPercent, TopPanelManager.instance.GetTimer());

        Time.timeScale = 0f;
        playerWon = true;
        tileMap.WinSprites();
        PlaySound(winSound);
    }

    // Scene change
    public static void LoadScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
    public void LoadMenu()
    {
        // Save board, if game has started and still going
        if (!playerLost && !playerWon && !firstClick)
            SaveSystem.instance.SaveTileMap(tileMap);
        else SaveSystem.instance.DeleteSave();

        // Forsefully destroy objects
        Destroy(SaveSystem.instance.gameObject);
        Destroy(audioObject);
        Destroy(gameObject);
        Destroy(tileMapVisual.gameObject);
        Destroy(tileMapVisual.meshObject);

        // Load menu scene
        LoadScene(0);
    }

    // Main menu sections
    public void SetActiveSettings(bool value)
    {
        settingsMenu.SetActive(value);
        if (value)
        {
            GetPrefs(out int width, out int heigth, out float minesPercent, out float tileSize);
            widthInputField.text = width.ToString();
            heigthInputField.text = heigth.ToString();
            minesPercentageInputField.text = minesPercent.ToString();
            tileSizeInputField.text = tileSize.ToString();
        }

        int heigth1 = int.Parse(heigthInputField.text);
        int width1 = int.Parse(widthInputField.text);
        float minesPercent1 = float.Parse(minesPercentageInputField.text);
        float tileSize1 = float.Parse(tileSizeInputField.text);

        SetPrefs(width1, heigth1, minesPercent1, tileSize1);
    }
    public void SetActiveContinueLastGamePopUp(bool value)
    {
        continueLastGamePopUp.SetActive(value);
    }

    // Stats logic
    public void SetActiveStats(bool value)
    {
        if (value) StatsPageText();
        statsMenu.SetActive(value);
    }
    public void SetPage(int num)
    {
        statsPage = num;
        StatsPageText();
    }
    public void SetPageFromCurrent(int num)
    {
        int value = Mathf.Clamp(statsPage + num, 0, Mathf.CeilToInt(WinsInFile / 40));
        SetPage(value);
    }
    private void StatsPageText()
    {
        pageText.text = "Page " + (statsPage + 1);
        statsText1.text = GetStatsText(40 * statsPage, 40 * statsPage + 20);
        statsText2.text = GetStatsText(40 * statsPage + 20, 40 * statsPage + 40);
    }
    private string GetStatsText(int fromIncludive, int toExcludive)
    {
        System.Text.StringBuilder resultString = new();
        List<WinObject> winObjects = RequestWinResults();

        fromIncludive = Mathf.Clamp(fromIncludive, 0, WinsInFile);
        toExcludive = Mathf.Clamp(toExcludive, fromIncludive, WinsInFile);

        for(int i = fromIncludive; i < toExcludive; i++)
        {
            WinObject obj = winObjects[i];
            resultString.Append("#" + obj.Id + " || " + obj.x + "x" + obj.y + " - " + obj.mP + "% || " + (obj.t100/100).ToString("0.00") + "\n");
        }

        return resultString.ToString();
    }

    // Utils
    public static void Quit()
    {
        Application.Quit();
    }
    public static void PlaySound(AudioSource soundSource)
    {
        soundSource.Play();
    }

    
    // Get / Set
    public TileMap GetTileMap()
    {
        return tileMap;
    }
    public void GetGameValues(out int width, out int heigth, out int minesPlaced, out int flagsPlaced, out bool firstClick)
    {
        heigth = this.heigth;
        width = this.width;
        minesPlaced = this.minesPlaced;
        flagsPlaced = this.flagsPlaced;
        firstClick = this.firstClick;
    }
    public void SetGameValues( int width,  int heigth,  int minesPlaced,  int flagsPlaced,  bool firstClick)
    {
        this.heigth = heigth;
        this.width = width;
        this.minesPlaced = minesPlaced;
        this.flagsPlaced = flagsPlaced;
        this.firstClick = firstClick;
    }
    public static void GetPrefs(out int width, out int heigth, out float minesPercent, out float tileSize)
    {
        if(!PlayerPrefs.HasKey("width"))
            SetPrefs(10, 10, 16f, 2f);

        width = PlayerPrefs.GetInt("width");
        heigth = PlayerPrefs.GetInt("heigth");
        minesPercent = PlayerPrefs.GetFloat("minesPercent");
        tileSize = PlayerPrefs.GetFloat("tileSize");
    }
    public static void SetPrefs( int width,  int heigth,  float minesPercent,  float tileSize)
    {
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("heigth", heigth);
        PlayerPrefs.SetFloat("minesPercent", minesPercent);
        PlayerPrefs.SetFloat("tileSize", tileSize);
    }

}
