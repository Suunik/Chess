using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static private int TileCountX = 8;
    static private int TileCountY = 8;

    public Square[,] squares = new Square[TileCountX, TileCountY];

    public Square SquarePrefab;

    public Piece[] WhitePiecePrefab = new Piece[6];
    public Piece[] BlackPiecePrefab = new Piece[6];

    void Awake()
    {
        SpawnSquares();
        SpawnAllWhitePieces();
        SpawnAllBlackPieces();
    }

    private void SpawnSquares()
    {
        for (int row = 0; row < TileCountX; ++row)
        {
            for (int column = 0; column < TileCountY; ++column)
            {
                squares[row,column] = Instantiate(SquarePrefab, new Vector3((-2.1f + column * 0.6f), (-2.1f + row * 0.6f), -1), Quaternion.identity);

                squares[row, column].SetRowAndColumn((char)(97 + row), (char)(49 + column));
                squares[row, column].SetSquareName();
            }
        }
    }

    private void SpawnAllWhitePieces()
    {
        // Spawn the pawn
        for (int i = 0; i < TileCountY; ++i)
        {
            SpawnSingleWhitePiece(0, 1, i);
        }

        SpawnSingleWhitePiece(1, 0, 0);
        SpawnSingleWhitePiece(2, 0, 1);
        SpawnSingleWhitePiece(3, 0, 2);
        SpawnSingleWhitePiece(4, 0, 3);
        SpawnSingleWhitePiece(5, 0, 4);
        SpawnSingleWhitePiece(3, 0, 5);
        SpawnSingleWhitePiece(2, 0, 6);
        SpawnSingleWhitePiece(1, 0, 7);
    }

    private void SpawnAllBlackPieces()
    {
        // Spawn the pawn
        for (int i = 0; i < TileCountY; ++i)
        {
            SpawnSingleBlackPiece(0, 6, i);
        }

        SpawnSingleBlackPiece(1, 7, 0);
        SpawnSingleBlackPiece(2, 7, 1);
        SpawnSingleBlackPiece(3, 7, 2);
        SpawnSingleBlackPiece(4, 7, 3);
        SpawnSingleBlackPiece(5, 7, 4);
        SpawnSingleBlackPiece(3, 7, 5);
        SpawnSingleBlackPiece(2, 7, 6);
        SpawnSingleBlackPiece(1, 7, 7);
    }

    private void SpawnSingleWhitePiece(int PieceNumber, int row, int column)
    {
        Piece WhitePiece =
            GameObject.Instantiate<Piece>(WhitePiecePrefab[PieceNumber], squares[row, column].transform.position, Quaternion.identity);
    }

    private void SpawnSingleBlackPiece(int PieceNumber, int row, int column)
    {
        Piece BlackPiece =
            GameObject.Instantiate<Piece>(BlackPiecePrefab[PieceNumber], squares[row, column].transform.position, Quaternion.identity);
    }
}
