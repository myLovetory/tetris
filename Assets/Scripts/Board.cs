
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    
    public Tilemap tilemap { get; private set; }
    public Piece activePiece {get; private set; }

    public TetrominoData[] tetrominoes;

    public Vector3Int spawnPosition;
    //kích thước bảng
    public Vector2Int boardSize = new Vector2Int(10,20);

    /*
     Phương thức Bounds được định nghĩa để trả về một hình chữ nhật (RectInt)
    đại diện cho giới hạn của bảng trong trò chơi.
    Phương thức này sử dụng từ khóa get để định nghĩa phần thân của phương thức.
     */
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    //first spawning block
    public void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        //check piece đã đc active chưa
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    public void Start()
    {
        SpawnPiece();
    }
    //spawm từng miểng 1
    public void SpawnPiece()
    {
        //use random :))) từ min là 0 đến max là chiều dài của mảng tetrominoes
        int random = Random.Range(0, tetrominoes.Length);

        TetrominoData data = tetrominoes[random];

        this.activePiece.Initialize(this, this.spawnPosition, data);

        //check hợp lệ
       if(IsValidPosition(this.activePiece, this.spawnPosition))
       {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        this.tilemap.ClearAllTiles();

    }
    
    //set để đặt các mảnh piece treenbanr đoof
    public void Set(Piece piece)
    {
        for(int i = 0;i < piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }

    }

    //hàm clear
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }

    }

    //kiểm tra vị trí của 1 khối block là hợp lệ
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }
    //xử lý clear hàng nếu đầy

    public void ClearLines()
    {
        //check nì
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax) 
        {
            if(IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                //đến dòng sau
                row++;
            }
        }
    }

    //check dòng nào full
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    //xoá dòng này
    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
         
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }    
}

