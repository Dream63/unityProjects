using UnityEngine;

public class TileMap
{
    public Grid<TilemapObject> grid;
    private int width, heigth;

    public TileMap(int width, int heigth, float tileSize, Vector3 startPos, bool iMGeneral, TileMapVisual tileMapVisual)
    {
        grid = new Grid<TilemapObject>(width, heigth, tileSize, startPos, iMGeneral , (Grid<TilemapObject> g, int x, int y, bool iM) => new TilemapObject(g, x, y, iM));
        tileMapVisual.SetGrid(grid);
        this.width = width;
        this.heigth = heigth;
    }

    public void SetTileMapSprite(Vector3 worldPos, TilemapObject.TilemapSprites newTileMapSprite)
    {
        TilemapObject tileMapObject = grid.GetObject(worldPos);
        tileMapObject?.TilemapSetSprite(newTileMapSprite);
    }
    public void SetTileMapSprite(Vector2Int tileXY, TilemapObject.TilemapSprites newTileMapSprite)
    {
        TilemapObject tileMapObject = grid.GetObject(tileXY);
        tileMapObject?.TilemapSetSprite(newTileMapSprite);
    }
    public void WinSprites()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < heigth; j++)
            {
                TilemapObject tmO = Tile(i, j);
                if (!tmO.iO)
                {
                    if (tmO.tmS == TilemapObject.TilemapSprites.UndiscoveredSquare)
                        tmO.tmS = TilemapObject.TilemapSprites.WinningMine;
                    if (tmO.tmS == TilemapObject.TilemapSprites.Flag)
                        tmO.tmS = TilemapObject.TilemapSprites.WinningFlag;
                    grid.TriggerObjectChanged(new(i, j));
                }
            }
    }
    public void AutoWin()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < heigth; j++)
            {
                TilemapObject tmO = Tile(i, j);

                if (!tmO.iO && !tmO.iM)
                    tmO.OpenTileF();
            }
    }
    public bool IsTileOpen(Vector2Int tileXY)
    {
        return grid.GetObject(tileXY).iO;
    }
    public bool IsTileOpen(int x, int y)
    {
        return Tile(x, y).iO;
    }
    public void OpenTilesAround(Vector2Int tileXY)
    {
        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
                Tile(tileXY.x + i, tileXY.y + j)?.OpenTileF();
    }
    public bool OnlyMinesLeft()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < heigth; j++)
                if (!IsTileOpen(i, j))
                    if (!Tile(i, j).iM)
                        return false;

        return true;
    }
    public TilemapObject Tile(int x, int y)
    {
        return grid.GetObject(new Vector2Int(x, y));
    }
    public TilemapObject Tile(Vector2Int tileXY)
    {
        return grid.GetObject(tileXY);
    }
    public Grid<TilemapObject> GetGrid()
    {
        return grid;
    }

    public class TilemapObject
    {
        public enum TilemapSprites
        {
            UndiscoveredSquare,
            Flag,
            WinningFlag,
            QuestionMark,
            Mine,
            WinningMine,
            DestroyedMine,
            TileNum1, 
            TileNum2, 
            TileNum3, 
            TileNum4, 
            TileNum5, 
            TileNum6, 
            TileNum7, 
            TileNum8, 
            TileNum0
        }
        public TilemapSprites tmS; // Tile map sprite

        Grid<TilemapObject> grid;
        public Vector2Int tileXY;
        
        public bool iM;         // is Mine
        public bool iO = false; // is Open
        int numOfMinesAround;

        public TilemapObject(Grid<TilemapObject> grid1, int x1, int y1, bool iM1) 
        {
            iM = iM1;
            grid = grid1; 
            tileXY = new Vector2Int(x1, y1);
        }
        public void TilemapSetSprite(TilemapSprites tileMapSprite)
        {
            tmS = tileMapSprite;
            grid.TriggerObjectChanged(tileXY);
        }
        public void OpenTile()
        { 
            if(tmS == TilemapSprites.UndiscoveredSquare)
            {
                iO = true;
                numOfMinesAround = NumOfMinesAroundTile(tileXY);
                if (numOfMinesAround == 0)
                {
                    TilemapSetSprite(TilemapSprites.TileNum0);
                    GameManager.instance.tileMap.OpenTilesAround(tileXY);
                }
                if (numOfMinesAround == 1)
                    TilemapSetSprite(TilemapSprites.TileNum1);
                if (numOfMinesAround == 2)
                    TilemapSetSprite(TilemapSprites.TileNum2);
                if (numOfMinesAround == 3)
                    TilemapSetSprite(TilemapSprites.TileNum3);
                if (numOfMinesAround == 4)
                    TilemapSetSprite(TilemapSprites.TileNum4);
                if (numOfMinesAround == 5)
                    TilemapSetSprite(TilemapSprites.TileNum5);
                if (numOfMinesAround == 6)
                    TilemapSetSprite(TilemapSprites.TileNum6);
                if (numOfMinesAround == 7)
                    TilemapSetSprite(TilemapSprites.TileNum7);
                if (numOfMinesAround == 8)
                    TilemapSetSprite(TilemapSprites.TileNum8);
                if (numOfMinesAround == -1)
                {
                    TilemapSetSprite(TilemapSprites.DestroyedMine);
                    GameManager.instance.Lose();
                }
                if (GameManager.instance.tileMap.OnlyMinesLeft())
                    GameManager.instance.Win();
            }

        } // Opens tile if its UndiscoveredSquare only
        public void OpenTileF()
        {
            if (!iO)
            {
                iO = true;
                numOfMinesAround = NumOfMinesAroundTile(tileXY);

                if (numOfMinesAround == 0)
                {
                    TilemapSetSprite(TilemapSprites.TileNum0);
                    GameManager.instance.tileMap.OpenTilesAround(tileXY);
                }
                if (numOfMinesAround == 1)
                    TilemapSetSprite(TilemapSprites.TileNum1);
                if (numOfMinesAround == 2)
                    TilemapSetSprite(TilemapSprites.TileNum2);
                if (numOfMinesAround == 3)
                    TilemapSetSprite(TilemapSprites.TileNum3);
                if (numOfMinesAround == 4)
                    TilemapSetSprite(TilemapSprites.TileNum4);
                if (numOfMinesAround == 5)
                    TilemapSetSprite(TilemapSprites.TileNum5);
                if (numOfMinesAround == 6)
                    TilemapSetSprite(TilemapSprites.TileNum6);
                if (numOfMinesAround == 7)
                    TilemapSetSprite(TilemapSprites.TileNum7);
                if (numOfMinesAround == 8)
                    TilemapSetSprite(TilemapSprites.TileNum8);
                if (numOfMinesAround == -1)
                {
                    TilemapSetSprite(TilemapSprites.DestroyedMine);
                    GameManager.instance.Lose();
                }
                if (GameManager.instance.tileMap.OnlyMinesLeft() && GameManager.instance.playerWon)
                    GameManager.instance.Win();
            }


        } // Opens tile if its closed, no matter what the tile is
        public int  NumOfMinesAroundTile(Vector2Int tileXY)
        {
            int x = tileXY.x, y = tileXY.y;
            int numOfMinesAroundTile = 0;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (grid.GetObject(new Vector2Int(x + i, y + j)) != null && grid.GetObject(new Vector2Int(x + i, y + j)).iM)
                        numOfMinesAroundTile++;

            if (grid.GetObject(tileXY) != null && grid.GetObject(tileXY).iM)
                numOfMinesAroundTile = -1;

            return numOfMinesAroundTile;
        }
        public void LoadTileValues(Vector2Int loadTileXY, bool loadIsMine, TilemapSprites loadTilemapSprite, bool loadIsOpen)
        {
            tileXY = loadTileXY;
            iM = loadIsMine;
            tmS = loadTilemapSprite;
            iO = loadIsOpen;
        }

        public override string ToString()
        {
            return iM.ToString();
        }
    }
}
