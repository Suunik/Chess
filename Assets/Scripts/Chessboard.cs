using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    public static Chessboard instance;
    static private int TILE_COUNT_X = 8;
    static private int TILE_COUNT_Y = 8;
    public Square[,] squares = new Square[TILE_COUNT_X, TILE_COUNT_Y];

    //Piece array for chessboard
    public ChessPiece[] whitePieces = new ChessPiece[16];
    public ChessPiece[] blackPieces = new ChessPiece[16];
    //For piece sprites
    public ChessPiece[] whitePiecePrefab = new ChessPiece[6];
    public ChessPiece[] blackPiecePrefab = new ChessPiece[6];

    public Square squarePrefab;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Create all tile objects and assign them a value e.g. d3
        SpawnSquares();

        //Create all white piece objects in their correct location
        SpawnAllWhitePieces();

        //Create all black piece objects in their correct location
        SpawnAllBlackPieces();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void SpawnSquares()
    {
        for (int row = 0; row < TILE_COUNT_X; ++row)
        {
            for (int column = 0; column < TILE_COUNT_Y; ++column)
            {
                squares[column, row] = Instantiate(squarePrefab, new Vector3((-3.5f + column), (-3.5f + row), -1), Quaternion.identity);

                squares[column, row].SetRowAndColumn((char)(97 + column), (char)(49 + row));
                squares[column, row].SetSquareName();
            }
        }
    }

    private void SpawnAllWhitePieces()
    {
        //Spawns the pawns
        for (int i = 0; i < TILE_COUNT_Y; ++i)
        {
            //SpawnSingleWhitePiece(i, 0, 1, i);
        }
        
        SpawnSingleWhitePiece(8, 1, 0, 0);
        SpawnSingleWhitePiece(9, 2, 0, 1);
        SpawnSingleWhitePiece(10, 3, 0, 2);
        SpawnSingleWhitePiece(11, 4, 0, 3);
        SpawnSingleWhitePiece(12, 5, 0, 4);
        SpawnSingleWhitePiece(13, 3, 0, 5);
        SpawnSingleWhitePiece(14, 2, 0, 6);
        SpawnSingleWhitePiece(15, 1, 0, 7);
        
    }
    private void SpawnAllBlackPieces()
    {
        // Spawns the pawns
        for (int i = 0; i < TILE_COUNT_Y; ++i)
        {
            //SpawnSingleBlackPiece(i, 0, 6, i);
        }

        SpawnSingleBlackPiece(8, 1, 7, 0);
        SpawnSingleBlackPiece(9, 2, 7, 1);
        SpawnSingleBlackPiece(10, 3, 7, 2);
        SpawnSingleBlackPiece(11, 4, 7, 3);
        SpawnSingleBlackPiece(12, 5, 7, 4);
        SpawnSingleBlackPiece(13, 3, 7, 5);
        SpawnSingleBlackPiece(14, 2, 7, 6);
        SpawnSingleBlackPiece(15, 1, 7, 7);
    }
    private void SpawnSingleWhitePiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        whitePieces[PieceNumber] = Instantiate(whitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        whitePieces[PieceNumber].currentSquare = squares[row, column];
        whitePieces[PieceNumber].team = 1;
        whitePieces[PieceNumber].currentSquare.team = 1;
    }
    private void SpawnSingleBlackPiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        blackPieces[PieceNumber] = Instantiate(blackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        blackPieces[PieceNumber].currentSquare = squares[row, column];
        blackPieces[PieceNumber].team = -1;
        blackPieces[PieceNumber].currentSquare.team = -1;
    }
}
