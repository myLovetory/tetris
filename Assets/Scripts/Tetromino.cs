using UnityEngine.Tilemaps;
using UnityEngine;

//enum để định nghĩa biến mới ở đây là các chữ cái đại diện cho khối đổ
public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;

    //các ô bản chất là các vector2 chiều
    //dùng nhập bằng geter hoặc private set
    public Vector2Int[] cells { get; private set; }

    //bảng vector 2 chiều cho
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }

}