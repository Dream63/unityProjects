using CodeMonkey.Utils;
using System;
using UnityEngine;

// To create grid u need to make new with all values needed:
// grid = new Grid<TGridObject>(10, 10, 5f, gridStartPos, (Grid<TGridObject> g, int x, int y) => new TGridObject(g, x, y));
                                                     /* Func<Grid<TGridObject>,   int,   int, TGridObject> */


public class Grid<TGridObject> 
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridObjectChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    int height;
    int width;
    float tileSize;
    public TGridObject[,] gridArray;
    Vector3 originPos;
    TextMesh[,] debugTextArray;

    // center grid on map (0,0)
    bool centerGridX = true;
    bool centerGridY = true;

    public Grid(int width, int height, float tileSize, Vector3 originPosition, bool iMGeneral, Func<Grid<TGridObject>, int, int, bool, TGridObject> createGridObject) // Func return types depends on object constructor 
    {
        this.height = height;
        this.width = width;
        this.tileSize = tileSize;
        this.originPos = originPosition;

        if (centerGridX) originPos = new Vector3(-width * tileSize / 2, originPos.y,            originPos.z);
        if (centerGridY) originPos = new Vector3(originPos.x,           -height * tileSize / 2, originPos.z);

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) 
            for (int y = 0; y < gridArray.GetLength(1); y++)
                gridArray[x, y] = createGridObject(this, x, y, iMGeneral);

        bool showDebug = !true;
        if (showDebug)
        {
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(tileSize, tileSize) * .5f, 5, Color.black, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() { return width; }
    public int GetHeight() { return height; }
    public float GetTileSize() { return tileSize; }
    public Vector3 getOriginPos() { return originPos; }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * tileSize + originPos.x, y * tileSize + originPos.y);
    }
    public Vector2Int GetTileXY(Vector3 tilePos)
    {
        return new(Mathf.FloorToInt((tilePos.x - originPos.x) / tileSize), Mathf.FloorToInt((tilePos.y - originPos.y) / tileSize));
    }

    public Vector3 MouseToWorldPoint(Vector3 cameraPos)
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition + cameraPos));
    }
    public Vector3 MouseToWorldPoint()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public TGridObject GetObject(Vector2Int tileXY)
    {
        if (tileXY.y < height && tileXY.y >= 0 && tileXY.x < width && tileXY.x >= 0)
            return gridArray[tileXY.x, tileXY.y];
        else return default;
    }
    public TGridObject GetObject(int x, int y)
    {
        if (y < height && y >= 0 && x < width && x >= 0)
            return gridArray[x, y];
        else return default;
    }
    public TGridObject GetObject(Vector3 worldPos)
    {
        Vector2Int posInt = GetTileXY(worldPos);
        return GetObject(posInt);
    }
    public void SetObject(Vector3 worldPos, TGridObject newObject)
    {
        Vector2Int posInt = GetTileXY(worldPos);
        SetObject(posInt, newObject);
    }
    public void SetObject(Vector2Int tileXY, TGridObject newObject)
    {
        if (tileXY.y < height && tileXY.y >= 0 && tileXY.x < width && tileXY.x >= 0)
        {
            gridArray[tileXY.x, tileXY.y] = newObject;
            OnGridObjectChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = tileXY.x, y = tileXY.y });
        }
    }
    public bool IsClickInsideThGrid(Vector3 worldPos)
    {
        bool clickIsInsideTheGrid = worldPos.x >= originPos.x && worldPos.x < (originPos.x + width * tileSize) && worldPos.y >= originPos.y && worldPos.y < (originPos.y + height * tileSize);
        return clickIsInsideTheGrid;
    }
    public bool IsClickInsideThGrid(Vector2Int tileXY)
    {
        bool clickIsInsideTheGrid = tileXY.x >= 0 && tileXY.x < width && tileXY.y >= 0 && tileXY.y < height;
        return clickIsInsideTheGrid;
    }
    public void TriggerObjectChanged(Vector2Int tileXY)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = tileXY.x, y = tileXY.y });
    }
}
