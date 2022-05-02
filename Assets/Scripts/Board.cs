using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static private int TileCountX = 8;
    static private int TileCountY = 8;

    public Square[,] squares = new Square[TileCountX, TileCountY];

    public Square SquarePrefab;

    private Piece[] WhitePieces = new Piece[16];
    private Piece[] BlackPieces = new Piece[16];

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
                squares[row,column] = Instantiate(SquarePrefab, new Vector3((-3.5f + column), (-3.5f + row ), -1), Quaternion.identity);

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
            SpawnSingleWhitePiece(i, 0, 1, i);
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
        for (int i = 0; i < TileCountY; ++i)
        {
            SpawnSingleBlackPiece(i, 0, 6, i);
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

    private void SpawnSingleWhitePiece(int PieceNumber, int PiecePrefab, int row, int column)
    {
        WhitePieces[PieceNumber] = Instantiate(WhitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        WhitePieces[PieceNumber].currentSquare = squares[row, column];
    }

    private void SpawnSingleBlackPiece(int PieceNumber, int PiecePrefab, int row, int column)
    {
        BlackPieces[PieceNumber] = Instantiate(BlackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        BlackPieces[PieceNumber].currentSquare = squares[row, column];
    }
}