using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static private int TileCountX = 8;
    static private int TileCountY = 8;

    public static Board Instance;

    //Squares
    public Square[,] squares = new Square[TileCountX, TileCountY];
    public List<Square> AvailableMoves = new List<Square>();
    public Square SquarePrefab;

    private Square PawnTransformSquare;

    //Pieces
    private Piece[] WhitePieces = new Piece[16];
    private Piece[] BlackPieces = new Piece[16];

    //Nupu spritede jaoks
    public Piece[] WhitePiecePrefab = new Piece[6];
    public Piece[] BlackPiecePrefab = new Piece[6];

    //Gameplay
    public int TurnCounter = 0;
    public bool EnPassantCheck = false;

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

        CheckAvailableMoves(Pieceheld());

        CheckForCastle();
        CheckForPawnTransform();

        if (EnPassantCheck)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (WhitePieces[i].EnPassantDone)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        if (BlackPieces[x].EnPassant)
                        {
                            BlackPieces[x].PieceActive = false;
                        }
                    }
                    WhitePieces[i].EnPassantDone = false;
                }
            }
            EnPassantCheck = false;
        }
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
         //Spawns the pawns
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

    //Movement and capture
    private void CheckAvailableMoves(Piece Pieceheld)
    {
        //PAWN Movement
        if (Pieceheld != null && Pieceheld.PieceNumber == 0)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            if (Pieceheld.PieceColor == 'w')
                            {
                                if (IsLegalMoveWhite(x + 1, y) && !CheckForBlackEnemy(x + 1, y))
                                    AvailableMoves.Add(squares[x + 1, y]);

                                if (!Pieceheld.PieceHasMoved)
                                {
                                    AvailableMoves.Add(squares[x + 2, y]);
                                }
                                if (CheckForBlackEnemy(x + 1, y + 1))
                                    AvailableMoves.Add(squares[x + 1, y + 1]);
                                if (CheckForBlackEnemy(x + 1, y - 1))
                                    AvailableMoves.Add(squares[x + 1, y - 1]);
                                //En Passant
                                if(Pieceheld.currentSquare == squares[4 , y])
                                {
                                    for (int i = 0; i < BlackPieces.Length; ++i)
                                    {
                                        //Kontrollib kas mustadel on en passant aktiveeritud
                                        if (BlackPieces[i].EnPassant)
                                        {
                                            //otsib yles mis ruudul must nupp asub
                                            for (int z = 0; z < TileCountX; ++z)
                                            {
                                                for (int c = 0; c < TileCountY; ++c)
                                                {
                                                    if (BlackPieces[i].currentSquare == squares[z, c])
                                                    {
                                                        if (WithinBounds(z, c - 1) && squares[z, c - 1] == Pieceheld.currentSquare)
                                                        {
                                                            if (Pieceheld.EnpassantSquare == null)
                                                            {
                                                                //Salvestab ruudu kuhu valge k2ima peab et EnPassant toimuks
                                                                for (int PieceNumber = 0; PieceNumber < 8; ++PieceNumber)
                                                                {
                                                                    if (Pieceheld == WhitePieces[PieceNumber])
                                                                    {
                                                                        EnPassantCheck = true;
                                                                        WhitePieces[PieceNumber].EnpassantSquare = squares[z + 1, c];
                                                                    }
                                                                }
                                                            }
                                                            AvailableMoves.Add(squares[z + 1, c]);
                                                        }
                                                        if (WithinBounds(z, c + 1) && squares[z, c + 1] == Pieceheld.currentSquare)
                                                        {
                                                            if (Pieceheld.EnpassantSquare == null)
                                                            {
                                                                //Salvestab ruudu kuhu valge k2ima peab et EnPassant toimuks
                                                                for (int PieceNumber = 0; PieceNumber < 8; ++PieceNumber)
                                                                {
                                                                    if (Pieceheld == WhitePieces[PieceNumber])
                                                                    {
                                                                        EnPassantCheck = true;
                                                                        WhitePieces[PieceNumber].EnpassantSquare = squares[z + 1, c];
                                                                    }
                                                                }
                                                            }
                                                            AvailableMoves.Add(squares[z + 1, c]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (Pieceheld.PieceColor == 'b')
                            {
                                if (IsLegalMoveBlack(x - 1, y) && !CheckForWhiteEnemy(x - 1, y))
                                    AvailableMoves.Add(squares[x - 1, y]);

                                if (!Pieceheld.PieceHasMoved)
                                {
                                    AvailableMoves.Add(squares[x - 2, y]);
                                }

                                if (CheckForWhiteEnemy(x - 1, y + 1))
                                    AvailableMoves.Add(squares[x - 1, y + 1]);
                                if (CheckForWhiteEnemy(x - 1, y - 1))
                                    AvailableMoves.Add(squares[x - 1, y - 1]);
                                //En passant
                                if (Pieceheld.currentSquare == squares[3, y])
                                {
                                    for (int i = 0; i < WhitePieces.Length; ++i)
                                    {
                                        //Kontrollib kas valgetel on en passant aktiveeritud
                                        if (WhitePieces[i].EnPassant)
                                        {
                                            //otsib yles mis ruudul must nupp asub
                                            for (int z = 0; z < TileCountX; ++z)
                                            {
                                                for (int c = 0; c < TileCountY; ++c)
                                                {
                                                    if (WhitePieces[i].currentSquare == squares[z, c])
                                                    {
                                                        if (WithinBounds(z, c - 1) && squares[z, c - 1] == Pieceheld.currentSquare)
                                                        {
                                                            AvailableMoves.Add(squares[z - 1, c]);
                                                        }
                                                        if (WithinBounds(z, c + 1) && squares[z, c + 1] == Pieceheld.currentSquare)
                                                        {
                                                            AvailableMoves.Add(squares[z - 1, c]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }
                            Pieceheld.AvailableMovesCheck = false;
                        }
                    }
                }
            }
        }
        //KING Movement
        if (Pieceheld != null && Pieceheld.PieceNumber == 5)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            if(IsLegalMoveWhite(x + 1, y) || IsLegalMoveBlack(x + 1, y))
                                AvailableMoves.Add(squares[x + 1, y]);

                            if (IsLegalMoveWhite(x + 1, y - 1) || IsLegalMoveBlack(x + 1, y - 1))
                                AvailableMoves.Add(squares[x + 1, y - 1]);

                            if (IsLegalMoveWhite(x + 1, y + 1) || IsLegalMoveBlack(x + 1, y + 1))
                                AvailableMoves.Add(squares[x + 1, y + 1]);

                            if (IsLegalMoveWhite(x, y - 1) || IsLegalMoveBlack(x, y - 1))
                                AvailableMoves.Add(squares[x, y - 1]);

                            if (IsLegalMoveWhite(x, y + 1) || IsLegalMoveBlack(x , y + 1))
                                AvailableMoves.Add(squares[x, y + 1]);

                            if (IsLegalMoveWhite(x - 1, y - 1) || IsLegalMoveBlack(x - 1, y - 1))
                                AvailableMoves.Add(squares[x - 1, y - 1]);

                            if (IsLegalMoveWhite(x - 1, y) || IsLegalMoveBlack(x - 1, y))
                                AvailableMoves.Add(squares[x - 1, y]);

                            if (IsLegalMoveWhite(x - 1, y + 1) || IsLegalMoveBlack(x - 1, y + 1))
                                AvailableMoves.Add(squares[x - 1, y + 1]);

                            //Vangerdus
                            if (!Pieceheld.PieceHasMoved)
                            {
                                //Valge
                                if (IsLegalMoveWhite(x, y + 1) && !CheckForBlackEnemy(x, y + 1))
                                {
                                    if (IsLegalMoveWhite(x, y + 2) && !CheckForBlackEnemy(x, y + 2))
                                    {
                                        AvailableMoves.Add(squares[x, y + 2]);
                                    }
                                }
                                if (IsLegalMoveWhite(x, y - 1) && !CheckForBlackEnemy(x, y - 1))
                                {
                                    if (IsLegalMoveWhite(x, y - 2) && !CheckForBlackEnemy(x, y - 2))
                                    {
                                        if (IsLegalMoveWhite(x, y - 3) && !CheckForBlackEnemy(x, y - 3))
                                        {
                                            AvailableMoves.Add(squares[x, y - 2]);
                                        }
                                    }
                                }
                                //Must
                                if (IsLegalMoveBlack(x, y + 1) && !CheckForWhiteEnemy(x, y + 1))
                                {
                                    if (IsLegalMoveBlack(x, y + 2) && !CheckForWhiteEnemy(x, y + 2))
                                    {
                                        AvailableMoves.Add(squares[x, y + 2]);
                                    }
                                }
                                if (IsLegalMoveBlack(x, y - 1) && !CheckForWhiteEnemy(x, y - 1))
                                {
                                    if (IsLegalMoveBlack(x, y - 2) && !CheckForWhiteEnemy(x, y - 2))
                                    {
                                        if (IsLegalMoveBlack(x, y - 3) && !CheckForWhiteEnemy(x, y - 3))
                                        {
                                            AvailableMoves.Add(squares[x, y - 2]);
                                        }
                                    }
                                }
                            }
                            
                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }

                            Pieceheld.AvailableMovesCheck = false;
                            break;
                        }
                    }
                }
            }
        }
        //ROOK Movement
        if(Pieceheld != null && Pieceheld.PieceNumber == 1)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x + i, y) || IsLegalMoveBlack(x + i, y))
                                    AvailableMoves.Add(squares[x + i, y]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x + i, y) || CheckForWhiteEnemy(x + i, y))
                                    break;
                            }

                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y) || IsLegalMoveBlack(x - i, y))
                                    AvailableMoves.Add(squares[x - i, y]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x - i, y) || CheckForWhiteEnemy(x - i, y))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x, y + i) || IsLegalMoveBlack(x, y + i))
                                    AvailableMoves.Add(squares[x, y + i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x, y + i) || CheckForWhiteEnemy(x, y + i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x, y - i) || IsLegalMoveBlack(x, y - i))
                                    AvailableMoves.Add(squares[x, y - i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x, y - i) || CheckForWhiteEnemy(x, y - i))
                                    break;
                                
                            }

                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }
                            Pieceheld.AvailableMovesCheck = false;
                        }
                    }
                }
            }
        }
        //HORSE Movement
        if (Pieceheld != null && Pieceheld.PieceNumber == 2)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            if (IsLegalMoveWhite(x + 2, y + 1) || IsLegalMoveBlack(x + 2, y + 1))
                                AvailableMoves.Add(squares[x + 2, y + 1]);
                            if (IsLegalMoveWhite(x + 1, y + 2) || IsLegalMoveBlack(x + 1, y + 2))
                                AvailableMoves.Add(squares[x + 1, y + 2]);
                            if (IsLegalMoveWhite(x - 1, y + 2) || IsLegalMoveBlack(x - 1, y + 2))
                                AvailableMoves.Add(squares[x - 1, y + 2]);
                            if (IsLegalMoveWhite(x - 2, y + 1) || IsLegalMoveBlack(x - 2, y + 1))
                                AvailableMoves.Add(squares[x - 2, y + 1]);
                            if (IsLegalMoveWhite(x - 2, y - 1) || IsLegalMoveBlack(x - 2, y - 1))
                                AvailableMoves.Add(squares[x - 2, y - 1]);
                            if (IsLegalMoveWhite(x - 1, y - 2) || IsLegalMoveBlack(x - 1, y - 2))
                                AvailableMoves.Add(squares[x - 1, y - 2]);
                            if (IsLegalMoveWhite(x + 1, y - 2) || IsLegalMoveBlack(x + 1, y - 2))
                                AvailableMoves.Add(squares[x + 1, y - 2]);
                            if (IsLegalMoveWhite(x + 2, y - 1) || IsLegalMoveBlack(x + 2, y - 1))
                                AvailableMoves.Add(squares[x + 2, y - 1]);

                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }
                            Pieceheld.AvailableMovesCheck = false;
                        }
                    }
                }
            }
        }
        //BISHOP Movement
        if (Pieceheld != null && Pieceheld.PieceNumber == 3)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x + i, y + i) || IsLegalMoveBlack(x + i, y + i))  
                                    AvailableMoves.Add(squares[x + i, y + i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x + i, y + i) || CheckForWhiteEnemy(x + i, y + i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                               if (IsLegalMoveWhite(x + i, y - i) || IsLegalMoveBlack(x + i, y - i))
                                   AvailableMoves.Add(squares[x + i, y - i]);
                               else
                                   break;
                                if (CheckForBlackEnemy(x + i, y - i) || CheckForWhiteEnemy(x + i, y - i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y - i) || IsLegalMoveBlack(x - i, y - i))
                                    AvailableMoves.Add(squares[x - i, y - i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x - i, y - i) || CheckForWhiteEnemy(x - i, y - i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y + i) || IsLegalMoveBlack(x - i, y + i))
                                    AvailableMoves.Add(squares[x - i, y + i]);
                                else
                                    break;
                                 if (CheckForBlackEnemy(x - i, y + i) || CheckForWhiteEnemy(x - i, y + i))
                                     break;
                            }
                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }
                            Pieceheld.AvailableMovesCheck = false;
                        }
                    }
                }
            }
        }
        //QUEEN Movement
        if (Pieceheld != null && Pieceheld.PieceNumber == 4)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                for (int x = 0; x < TileCountX; ++x)
                {
                    for (int y = 0; y < TileCountY; ++y)
                    {
                        if (Pieceheld.currentSquare == squares[x, y])
                        {
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x + i, y) || IsLegalMoveBlack(x + i, y))
                                    AvailableMoves.Add(squares[x + i, y]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x + i, y) || CheckForWhiteEnemy(x + i, y))
                                    break;
                            }

                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y) || IsLegalMoveBlack(x - i, y))
                                    AvailableMoves.Add(squares[x - i, y]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x - i, y) || CheckForWhiteEnemy(x - i, y))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x, y + i) || IsLegalMoveBlack(x, y + i))
                                    AvailableMoves.Add(squares[x, y + i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x, y + i) || CheckForWhiteEnemy(x, y + i))
                                    break;

                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x, y - i) || IsLegalMoveBlack(x, y - i))
                                    AvailableMoves.Add(squares[x, y - i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x, y - i) || CheckForWhiteEnemy(x, y - i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x + i, y + i) || IsLegalMoveBlack(x + i, y + i))
                                    AvailableMoves.Add(squares[x + i, y + i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x + i, y + i) || CheckForWhiteEnemy(x + i, y + i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x + i, y - i) || IsLegalMoveBlack(x + i, y - i))
                                    AvailableMoves.Add(squares[x + i, y - i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x + i, y - i) || CheckForWhiteEnemy(x + i, y - i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y - i) || IsLegalMoveBlack(x - i, y - i))
                                    AvailableMoves.Add(squares[x - i, y - i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x - i, y - i) || CheckForWhiteEnemy(x - i, y - i))
                                    break;
                            }
                            for (int i = 1; i < 8; ++i)
                            {
                                if (IsLegalMoveWhite(x - i, y + i) || IsLegalMoveBlack(x - i, y + i))
                                    AvailableMoves.Add(squares[x - i, y + i]);
                                else
                                    break;
                                if (CheckForBlackEnemy(x - i, y + i) || CheckForWhiteEnemy(x - i, y + i))
                                    break;
                            }
                            for (int i = 0; i < AvailableMoves.Count; ++i)
                            {
                                AvailableMoves[i].HighlightSquare();
                            }
                            Pieceheld.AvailableMovesCheck = false;
                        }
                    }
                }
            }
        }
    }
    public void ClearAvailableMoves()
    {
        for (int i = 0; i < AvailableMoves.Count; ++i)
        {
            AvailableMoves[i].TransparentSquare();
        }

        AvailableMoves.Clear();
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
            if (WhitePieces[i].CheckForEnemy)
            {
                for (int y = 0; y < BlackPieces.Length; ++y)
                {
                    if (WhitePieces[i].currentSquare == BlackPieces[y].currentSquare)
                    {
                        BlackPieces[y].PieceActive = false;
                    }
                }
                WhitePieces[i].CheckForEnemy = false;
            }
        }
    }
    private void CheckForBlackPieceCapture()
    {
        for (int i = 0; i < BlackPieces.Length; ++i)
        {
            if (BlackPieces[i].CheckForEnemy)
            {
                for (int y = 0; y < WhitePieces.Length; ++y)
                {
                    if (BlackPieces[i].currentSquare == WhitePieces[y].currentSquare)
                    {
                        WhitePieces[y].PieceActive = false;
                    }
                }
                BlackPieces[i].CheckForEnemy = false;
            }
        }
    }
    private bool WithinBounds(int x, int y)
    {
        if (x < 8 && x > -1)
        {
            if (y < 8 && y > -1)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsLegalMoveWhite(int x, int y)
    {
        if(WithinBounds(x,y))
        {
            for (int z = 0; z < WhitePieces.Length; ++z)
            {
                if (WhitePieces[z].AvailableMovesCheck)
                {
                    for (int i = 0; i < WhitePieces.Length; ++i)
                    {
                        if (squares[x, y] == WhitePieces[i].currentSquare)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private bool IsLegalMoveBlack(int x, int y)
    {

        if (WithinBounds(x, y))
        {
            for (int z = 0; z < BlackPieces.Length; ++z)
            {
                if (BlackPieces[z].AvailableMovesCheck)
                {
                    for (int i = 0; i < BlackPieces.Length; ++i)
                    {
                        if (squares[x, y] == BlackPieces[i].currentSquare)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private bool CheckForBlackEnemy(int x, int y)
    {
        if (WithinBounds(x, y))
        {
            for (int z = 0; z < WhitePieces.Length; ++z)
            {
                if (WhitePieces[z].AvailableMovesCheck)
                {
                    for (int i = 0; i < BlackPieces.Length; ++i)
                    {
                        if (squares[x, y] == BlackPieces[i].currentSquare)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool CheckForWhiteEnemy(int x, int y)
    {
        if (WithinBounds(x, y))
        {
            if (WithinBounds(x, y))
            {
                for (int z = 0; z < BlackPieces.Length; ++z)
                {
                    for (int i = 0; i < WhitePieces.Length; ++i)
                    {
                        if (squares[x, y] == WhitePieces[i].currentSquare)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    //Special Moves
    private void CheckForCastle()
    {
        if (WhitePieces[12].CastleTime)
        {
            if (WhitePieces[12].currentSquare == squares[0, 6])
            {
                WhitePieces[12].CastleTime = false;
                WhitePieces[15].transform.position = squares[0, 5].transform.position;
                WhitePieces[15].currentSquare = squares[0, 5];
                WhitePieces[15].PieceHasMoved = true;
            }
            if (WhitePieces[12].currentSquare == squares[0, 2])
            {
                WhitePieces[12].CastleTime = false;
                WhitePieces[8].transform.position = squares[0, 3].transform.position;
                WhitePieces[8].currentSquare = squares[0, 3];
                WhitePieces[8].PieceHasMoved = true;
            }
        }
        if (BlackPieces[12].CastleTime)
        {
            if (BlackPieces[12].currentSquare == squares[7, 6])
            {
                BlackPieces[12].CastleTime = false;
                BlackPieces[15].transform.position = squares[7, 5].transform.position;
                BlackPieces[15].currentSquare = squares[7, 5];
                BlackPieces[15].PieceHasMoved = true;
            }
            if (BlackPieces[12].currentSquare == squares[7, 2])
            {
                BlackPieces[12].CastleTime = false;
                BlackPieces[8].transform.position = squares[7, 3].transform.position;
                BlackPieces[8].currentSquare = squares[7, 3];
                BlackPieces[8].PieceHasMoved = true;
            }
        }
    }
    private void CheckForPawnTransform()
    {
        for(int i = 0; i < 8; ++i)
        {
            if(WhitePieces[i].PawnTransform)
            {
                WhitePieces[i].PawnTransform = false;
                PawnTransformSquare = WhitePieces[i].currentSquare;
                WhitePieces[i].gameObject.SetActive(false);
                WhitePieces[i] = Instantiate(WhitePiecePrefab[4], WhitePieces[i].transform.position, Quaternion.identity);
                WhitePieces[i].currentSquare = PawnTransformSquare;
            }
            if (BlackPieces[i].PawnTransform)
            {
                BlackPieces[i].PawnTransform = false;
                PawnTransformSquare = BlackPieces[i].currentSquare;
                BlackPieces[i].gameObject.SetActive(false);
                BlackPieces[i] = Instantiate(BlackPiecePrefab[4], BlackPieces[i].transform.position, Quaternion.identity);
                BlackPieces[i].currentSquare = PawnTransformSquare;
            }
        }
    } 
    private bool CheckForEnPassant()
    {
        for(int i = 0; i < BlackPieces.Length; ++i)
        {
            if(BlackPieces[i].EnPassant)
            {
                return true;
            }
        }
        return false;
    }
}