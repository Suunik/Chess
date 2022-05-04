using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    static private int TileCountX = 8;
    static private int TileCountY = 8;

    public Square[,] squares = new Square[TileCountX, TileCountY];

    public Square SquarePrefab;

    private Piece[] WhitePieces = new Piece[16];
    private Piece[] BlackPieces = new Piece[16];

    //Nupu spritede jaoks
    public Piece[] WhitePiecePrefab = new Piece[6];
    public Piece[] BlackPiecePrefab = new Piece[6];

    void Awake()
    {
        Instance = this;

        SpawnSquares();
        SpawnAllWhitePieces();
        SpawnAllBlackPieces();

    }

    private void Update()
    {
        CheckForBlackPieceCapture();
        CheckForWhitePieceCapture();

        AvailableMoves(Pieceheld());
    }

    //Spawning pieces and squares
    private void SpawnSquares()
    {
        for (int row = 0; row < TileCountX; ++row)
        {
            for (int column = 0; column < TileCountY; ++column)
            {
                squares[row, column] = Instantiate(SquarePrefab, new Vector3((-3.5f + column), (-3.5f + row), -1), Quaternion.identity);

                squares[row, column].SetRowAndColumn((char)(97 + row), (char)(49 + column));
                squares[row, column].SetSquareName();
            }
        }
    }

    private void SpawnAllWhitePieces()
    {
        // Spawns the pawns
        for (int i = 0; i < TileCountY; ++i)
        {
            SpawnSingleWhitePiece(i, 0, 1, i);
        }

        SpawnSingleWhitePiece(8, 1, 0, 0);
        SpawnSingleWhitePiece(9, 2, 0, 1);
        SpawnSingleWhitePiece(10, 3, 0, 2);
        SpawnSingleWhitePiece(11, 4, 0, 3);
        SpawnSingleWhitePiece(12, 5, 3, 4);
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

    //Movement and capture

    private void AvailableMoves(Piece Pieceheld)
    {   
        if (Pieceheld != null && Pieceheld.PieceNumber == 5)
        {
            for(int x = 0; x < TileCountX; ++x)
            {
                for (int y = 0; y < TileCountY; ++y)
                {
                    if (Pieceheld.currentSquare.ReturnSquare() == squares[x, y].ReturnSquare())
                    {
                        squares[x, y].HighlightSquare();
                        squares[x + 1, y - 1].HighlightSquare();
                        squares[x , y - 1].HighlightSquare();
                    }
                }
            }           
        }
    }

    private Piece Pieceheld()
    {
        for(int i = 0; i < WhitePieces.Length; ++i)
        {
            if(WhitePieces[i].PieceHeld)
            {
                return WhitePieces[i];
            }
        }

        for (int i = 0; i < BlackPieces.Length; ++i)
        {
            if (BlackPieces[i].PieceHeld)
            {
                return BlackPieces[i];
            }
        }
        return null;
    }
    private void CheckForWhitePieceCapture()
    {
        for (int i = 0; i < WhitePieces.Length; ++i)
        {
            if (WhitePieces[i].PieceWasMoved)
            {
                for (int y = 0; y < BlackPieces.Length; ++y)
                {
                    if (WhitePieces[i].currentSquare == BlackPieces[y].currentSquare)
                    {
                        BlackPieces[y].Active = false;
                    }
                }
                WhitePieces[i].PieceWasMoved = false;
            }
        }
    }
    private void CheckForBlackPieceCapture()
    {
        for (int i = 0; i < BlackPieces.Length; ++i)
        {
            if (BlackPieces[i].PieceWasMoved)
            {
                for (int y = 0; y < WhitePieces.Length; ++y)
                {
                    if (BlackPieces[i].currentSquare == WhitePieces[y].currentSquare)
                    {
                        WhitePieces[y].Active = false;
                    }
                }
                BlackPieces[i].PieceWasMoved = false;
            }
        }
    }
}