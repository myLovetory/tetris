
using UnityEngine;
//piece là lo các phẩn riêng lẻ còn board lo tổng thể trò chơi
public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }

    //vì tile map dùng vector3int
    public Vector3Int position { get; private set; }

    public Vector3Int[] cells { get; private set; }

    //biến lưu vòng quay
    public int rotationIndex { get; private set; }

    //tăng độ khó
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;
        this.rotationIndex = 0;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;


        //tham chiếu cells từ data
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    //hàm update nhe
    public void Update()
    {
        this.board.Clear(this);
        this.lockTime += Time.deltaTime;

        //hai phím quay
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }else if(Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }


        //movement nếu án A
        if(Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if(Time.time >= this.stepTime) 
        {
            Step();
        }

        this.board.Set(this) ;
    }

    //rơi tự động
    public void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }    

    //khóa khi ở dòng cuối
    public void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }


    //ko thể đi xuống được nữa,cái này cho rơi xuống nhanh hơn
    public void HardDrop()
    {
        while(Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    //hàm move để thao tác khi di chuyển
    private bool Move(Vector2Int translation)
    {
        //check vị trí mới bằng vị trí cũ cộng thêm
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }

    //hàm quay
    public void Rotate(int direction)
    {
        int originalRotaion = rotationIndex;
        //bản chất là xoay ma trận
        this.rotationIndex += direction;

        ApplyRotationMatrix(direction);

        if(!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotaion;
            ApplyRotationMatrix(-direction);
        }
        
    }

    public void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            //tọa độ sau xoay
            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }
    }
    //test va chamj vowis tuwowngf
    public bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int WallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        //
        for(int i = 0; i < data.wallKicks.GetLength(1);i++)
        {
            Vector2Int translation = data.wallKicks[WallKickIndex,i];

            if(Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    public int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int WallKickIndex = rotationIndex * 2;
        
        if(rotationIndex < 0) 
        {
            WallKickIndex--;
        }

        return Wrap(WallKickIndex, 0, data.wallKicks.GetLength(0));
    }
    
    //quay nì
    public int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}
