using UnityEngine;
using static Grid<TileMap.TilemapObject>;

public class TileMapVisual : MonoBehaviour
{
    // To asigh/create heatMap u need to set refference grid:
    // heatMap.SetGrid(grid);

    // Mesh parameters
    MyMesh mesh;
    public GameObject meshObject;
    public bool showMines = true;
    bool showMinesOld = false;
    int width, height;
    float tileSize;
    Vector3 meshPos;

    bool frameToUpdateMesh;

    // Refference grid
    Grid<TileMap.TilemapObject> grid;

    private void Grid_OnGridValueChanged(object sender, OnGridValueChangedEventArgs e)
    {
        TileVisualUpdate(grid.GetObject(e.x, e.y));
    }

    private void LateUpdate()
    {
        if(showMinesOld != showMines && !frameToUpdateMesh)
        {
            TileMapVisualUpdate();
            showMinesOld = showMines;
        }

        if (frameToUpdateMesh) 
        { 
            TileMapVisualUpdate(); 
            frameToUpdateMesh = false; 
        }
    }
    public void SetGrid(Grid<TileMap.TilemapObject> grid1)
    {
        grid = grid1;
        width = grid.GetWidth();
        height = grid.GetHeight();
        tileSize = grid.GetTileSize();
        meshPos = grid.getOriginPos();
        grid.OnGridObjectChanged += Grid_OnGridValueChanged;

        mesh = new MyMesh(width, height, tileSize, meshObject, meshPos);
        TileMapVisualUpdate();
    }
    public void TileVisualUpdate(TileMap.TilemapObject gridObject)
    {
        // Material tiling:
        // y\x  1:  2:  3:  4:  5:
        //     ____________________
        // 5: | 1,  2,  3,  4,  5  |
        // 4: | 6,  7,  8,  0,  qm |
        // 3: | ud, mn, fl,        |
        // 2: | wm, dm, wf,        |
        // 1: |                    |

        int index = gridObject.tileXY.x * height + gridObject.tileXY.y;
        Vector2 spriteXY = new(1, 1);

        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum1)
            spriteXY = new(1, 5);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum2)
            spriteXY = new(2, 5);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum3)
            spriteXY = new(3, 5);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum4)
            spriteXY = new(4, 5);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum5)
            spriteXY = new(5, 5);

        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum6)
            spriteXY = new(1, 4);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum7)
            spriteXY = new(2, 4);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum8)
            spriteXY = new(3, 4);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.TileNum0)
            spriteXY = new(4, 4);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.QuestionMark)
            spriteXY = new(5, 4);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.UndiscoveredSquare)
            spriteXY = new(1, 3);

        if (!showMines)
        {
            if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.Mine)
                spriteXY = new(2, 3);
            if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.Flag)
                spriteXY = new(3, 3);
        }

        if (showMines)
        {
            if (gridObject.iM)
                spriteXY = new(2, 3);
            else if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.Flag)
                spriteXY = new(3, 3);
        }

        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.WinningMine)
            spriteXY = new(1, 2);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.DestroyedMine)
            spriteXY = new(2, 2);
        if (gridObject.tmS == TileMap.TilemapObject.TilemapSprites.WinningFlag)
            spriteXY = new(3, 2);

        Vector2 uvValue00 = new((spriteXY.x - (2f/3f)) / 5, (spriteXY.y - (2f/3f)) / 5);
        Vector2 uvValue11 = new(spriteXY.x / 5, spriteXY.y / 5);
        mesh.MeshUvUpdate(uvValue00, uvValue11, index);
}
    public void TileMapVisualUpdate()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                TileVisualUpdate(grid.GetObject(i, j));
    }
}
