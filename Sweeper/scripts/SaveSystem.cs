using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Util;

public class SaveSystem : MonoBehaviour
{
    public static readonly string DataFolder = Application.dataPath + "/Data/";
    public static readonly string SaveFilePath = DataFolder + "TileMapSave.json";
    public static readonly string ResultsFilePath = DataFolder + "Results.json";
    public static SaveSystem instance;

    public static int WinsInFile => IsResultFileExist() ? PlayerPrefs.GetInt("WinsInFile") : 0;

    public void Awake()
    {
        instance = this;
        if (WinsInFile == 0)
            PlayerPrefs.SetInt("WinsInFile", 0);
    }

    public void Init()
    {
        DontDestroyOnLoad(instance);
        if (!Directory.Exists(DataFolder))
            Directory.CreateDirectory(DataFolder);
    }
    public static bool IsSaveExist()
    {
        return File.Exists(DataFolder + "TileMapSave.json");
    }

    public void SaveTileMap(TileMap tileMap)
    {
        List<string> tileObjectList = new List<string>();

        GameManager.instance.GetGameValues(out int width, out int heigth, out int minesPlaced, out int flagsPlaced, out bool firstClick);
        
        for (int i = 0; i < width; i++)
            for (int j = 0; j < heigth; j++)
                tileObjectList.Add(JsonUtility.ToJson(tileMap.grid.GetObject(new Vector2Int(i, j))));

        TileMapSaveObject saveObject = new TileMapSaveObject 
        {    
            tileObjectArray = tileObjectList.ToArray(),

            heigth = heigth,
            width = width,
            minesPlaced = minesPlaced, 
            flagsPlaced = flagsPlaced,
            firstClick = firstClick,
            timer = TopPanelManager.instance.GetTimer(),
        };

        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(saveObject));

    }
    public void LoadTileMap()
    {
        if (File.Exists(SaveFilePath))
        {
            string saveString = File.ReadAllText(SaveFilePath);
            TileMapSaveObject saveObject = JsonUtility.FromJson<TileMapSaveObject>(saveString);

            GameManager.instance.SetGameValues(saveObject.width, saveObject.heigth, saveObject.minesPlaced, saveObject.flagsPlaced, saveObject.firstClick);

            GameManager.instance.tileMap = new TileMap(saveObject.heigth, saveObject.width, float.Parse(GameManager.instance.tileSizeInputField.text), Vector3.zero, false, GameManager.instance.tileMapVisual);

            for (int i = 0; i < saveObject.width; i++)
                for (int j = 0; j < saveObject.heigth; j++)
                {
                    TileMap.TilemapObject savedObjectValues = JsonUtility.FromJson<TileMap.TilemapObject>(saveObject.tileObjectArray[i * saveObject.heigth + j]);
                    GameManager.instance.tileMap.grid.GetObject(new Vector2Int(i, j))?.LoadTileValues(savedObjectValues.tileXY, savedObjectValues.iM, savedObjectValues.tmS, savedObjectValues.iO);
                }
            GameManager.instance.tileMapVisual.TileMapVisualUpdate();
        }
    }
    public void DeleteSave()
    {
        File.Delete(DataFolder + "TileMapSave.json");
    }

    public static bool IsResultFileExist()
    {
        return File.Exists(ResultsFilePath);
    }
    public static void SaveWinResult(int width, int height, float minesPercent, float timer)
    {
        // Array of JSON win objects
        List<string> savedWinsStringArray = new();
        
        // If resultFile exists, fills array with saved JSON win objects
        if (IsResultFileExist())
            savedWinsStringArray = JsonUtility.FromJson<WinsObjectString>(File.ReadAllText(ResultsFilePath)).savedWinsStringArray;

        // Creates new win object and adds it to JSON Array
        WinObject winObject = new WinObject
        {
            x = width,
            y = height,
            mP = minesPercent,
            t100 = Mathf.RoundToInt(timer * 100),
            Id = GameManager.TotalWins
        };
        savedWinsStringArray.Add(JsonUtility.ToJson(winObject));
        PlayerPrefs.SetInt("WinsInFile", savedWinsStringArray.Count);
        WinsObjectString winsObject = new WinsObjectString
        {
            savedWinsStringArray = savedWinsStringArray
        };
        // Loads JSON file with updated JSON Array.
        // NOTE: double layer JSON
        File.WriteAllText(ResultsFilePath, JsonUtility.ToJson(winsObject));
    }
    public static List<WinObject> RequestWinResults()
    {
        List<string> savedWinsStringArray = new();
        List<WinObject> savedWinsArray = new();

        if (IsResultFileExist())
        {
            savedWinsStringArray = JsonUtility.FromJson<WinsObjectString>(File.ReadAllText(ResultsFilePath)).savedWinsStringArray;

            for (int i = 0; i < savedWinsStringArray.Count; i++)
                savedWinsArray.Add(JsonUtility.FromJson<WinObject>(savedWinsStringArray[i]));

        }
        return savedWinsArray;
    }
    public static void RemoveWinRelustsFromSaveFile(int fromIncludive, int toIncludive)
    {
        if (IsResultFileExist())
        {
            List<string> savedWinsStringArray = new();
            savedWinsStringArray = JsonUtility.FromJson<List<string>>(File.ReadAllText(ResultsFilePath));

            if (savedWinsStringArray.Count != 0)
            {
                fromIncludive = Utils.Clamp(fromIncludive, 0, savedWinsStringArray.Count);
                toIncludive = Utils.Clamp(toIncludive, fromIncludive, savedWinsStringArray.Count);

                for (int i = fromIncludive; i <= toIncludive; i++)
                    savedWinsStringArray.RemoveAt(fromIncludive);
            }

            PlayerPrefs.SetInt("WinsInFile", savedWinsStringArray.Count);
            File.WriteAllText(ResultsFilePath, JsonUtility.ToJson(savedWinsStringArray));
        }
    }

    public class WinsObjectString
    {
        public List <string> savedWinsStringArray;
    }
    public class WinObject
    {
        public int x;
        public int y;
        public float mP;
        public float t100;
        public int Id;
    }
    public class TileMapSaveObject
    {
        public string[] tileObjectArray;
        public int minesPlaced, flagsPlaced, width, heigth;
        public float timer;
        public bool firstClick;
    }

}



