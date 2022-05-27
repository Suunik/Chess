using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static private int TileCountX = 8;
    static private int TileCountY = 8;

    public static Board Instance;

    //Squares
    public Square SquarePrefab;
    public Square[,] squares = new Square[TileCountX, TileCountY];
    public List<Square> AvailableMoves = new List<Square>();
    private List<Square> EnemyAvailableMoves = new List<Square>();

    //Pieces
    private Piece[] WhitePieces = new Piece[16];
    private Piece[] BlackPieces = new Piece[16];
    private List<Piece> KingAttackers = new List<Piece>();

    //Nupu spritede jaoks
    public Piece[] WhitePiecePrefab = new Piece[6];
    public Piece[] BlackPiecePrefab = new Piece[6];

    //Gameplay
    public int turn_counter = 0;
    public int turn_flag = 0;
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
        CheckAvailableMoves(Pieceheld());

        if (turn_flag != turn_counter)
        {
            CheckForBlackPieceCapture();
            CheckForWhitePieceCapture();
            CheckForCastle();
            CheckForPawnTransform();
            CheckForChecks();

            turn_flag = turn_counter;
        }

    }

    //Spawning pieces and squares
    private void SpawnSquares()
    {
        for (int row = 0; row < TileCountX; ++row)
        {
            for (int column = 0; column < TileCountY; ++column)
            {
                squares[column, row] = Instantiate(SquarePrefab, new Vector3((-3.5f + column), (-3.5f + row), -1), Quaternion.identity);

                squares[column, row].SetRowAndColumn((char)(97 + column), (char)(49 + row));
                squares[column, row].SetSquareName();
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
        SpawnSingleWhitePiece(12, 5, 4, 4);
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
    private void SpawnSingleWhitePiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        WhitePieces[PieceNumber] = Instantiate(WhitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        WhitePieces[PieceNumber].currentSquare = squares[row, column];
    }
    private void SpawnSingleBlackPiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        BlackPieces[PieceNumber] = Instantiate(BlackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        BlackPieces[PieceNumber].currentSquare = squares[row, column];
    }
    private int[] ReturnRowColumn(Piece CurrentPiece)
    {
        string piece_coordinates = CurrentPiece.currentSquare.ReturnSquare();
        int[] value = new int[2];
        value[0] = piece_coordinates[0] - 97;
        value[1] = piece_coordinates[1] - 49;

        return value;
    }

    //Movement and capture
    private void CheckAvailableMoves(Piece Pieceheld)
    {
        if (Pieceheld != null)
        {
            if (Pieceheld.AvailableMovesCheck)
            {
                //PAWN Movement
                if (Pieceheld.PieceNumber == 0)
                     AvailableMoves = PawnMoves(Pieceheld);
                //ROOK Movement
                if (Pieceheld.PieceNumber == 1)
                    AvailableMoves = RookMoves(Pieceheld);
                //HORSE Movement
                if (Pieceheld.PieceNumber == 2)
                    AvailableMoves = HorseMoves(Pieceheld);
                //BISHOP Movement
                if (Pieceheld.PieceNumber == 3)
                    AvailableMoves = BishopMoves(Pieceheld);
                //QUEEN Movement
                if (Pieceheld.PieceNumber == 4)
                    AvailableMoves = QueenMoves(Pieceheld);
                //King Movement
                if (Pieceheld.PieceNumber == 5)
                    AvailableMoves = KingMoves(Pieceheld);

                for (int i = 0; i < AvailableMoves.Count; ++i)
                {
                    AvailableMoves[i].HighlightSquare();
                }
                Pieceheld.AvailableMovesCheck = false;
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
        for (int i = 0; i < WhitePieces.Length; ++i)
        {
            if (WhitePieces[i].PieceHeld)
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
                    if (WhitePieces[i].EnpassantSquare != null)
                    {
                        string white_square = WhitePieces[i].currentSquare.ReturnSquare();
                        int white_square_horizontal = white_square[0];
                        int white_square_vertical = white_square[1];

                        if (BlackPieces[y] != null)
                        {
                            string black_square = BlackPieces[y].currentSquare.ReturnSquare();
                            int black_square_horizontal = black_square[0];
                            int black_square_vertical = black_square[1];

                            if (white_square_vertical - 1 == black_square_vertical && white_square_horizontal == black_square_horizontal)
                            {
                                BlackPieces[y].PieceActive = false;
                                Debug.Log("Kaka");
                                break;
                            }
                        }
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
                    if (BlackPieces[i].EnpassantSquare != null)
                    {
                        string black_square = BlackPieces[i].currentSquare.ReturnSquare();
                        int black_square_horizontal = black_square[0];
                        int black_square_vertical = black_square[1];

                        if (BlackPieces[y] != null)
                        {
                            string white_square = WhitePieces[y].currentSquare.ReturnSquare();
                            int white_square_horizontal = white_square[0];
                            int white_square_vertial = white_square[1];

                            if (black_square_vertical + 1 == white_square_vertial && black_square_horizontal == white_square_horizontal)
                            {
                                WhitePieces[y].PieceActive = false;
                                Debug.Log("Kaka");
                                break;
                            }
                        }
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
    private bool IsLegalMove(Piece Piece, int x, int y)
    {
        if (WithinBounds(x, y))
        {
            if (Piece.PieceColor == 'w')
            {
                for (int i = 0; i < WhitePieces.Length; ++i)
                {
                    if (squares[x, y] == WhitePieces[i].currentSquare)
                    {
                        return false;
                    }
                    if (Piece.PieceNumber == 5)
                    {
                        foreach (Square enemy_squares in EnemyAvailableMoves)
                        {
                            if(enemy_squares == squares[x,y])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < BlackPieces.Length; ++i)
                {
                    if (squares[x, y] == BlackPieces[i].currentSquare)
                    {
                        return false;
                    }
                    if (Piece.PieceNumber == 5)
                    {
                        foreach (Square enemy_squares in EnemyAvailableMoves)
                        {
                            if (enemy_squares == squares[x, y])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
    private bool CheckForEnemy(Piece Piece, int x, int y)
    {
        if (WithinBounds(x, y))
        {
            if (Piece.PieceColor == 'w')
            {
                for (int i = 0; i < BlackPieces.Length; ++i)
                {
                    if (squares[x, y] == BlackPieces[i].currentSquare)
                    {
                        if(i == 12)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            else
                for (int i = 0; i < WhitePieces.Length; ++i)
                {
                    if (squares[x, y] == WhitePieces[i].currentSquare)
                    {
                        if (i == 12)
                        {
                            return false;
                        }
                        return true;
                    }
                }
        }
        return false;
    }
    private void EnemyMovesList(Piece CurrentPiece)
    {
        //Pawn
        if (CurrentPiece.PieceNumber == 0)
        {
            List<Square> pawn_squares = PawnAttackMoves(CurrentPiece);
            for (int index = 0; index < pawn_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(pawn_squares[index]);
                if(CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == pawn_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == pawn_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
        //Rook
        if (CurrentPiece.PieceNumber == 1)
        {
            List<Square> rook_squares = RookMoves(CurrentPiece);
            for (int index = 0; index < rook_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(rook_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == rook_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == rook_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
        //Horse
        if (CurrentPiece.PieceNumber == 2)
        {
            List<Square> horse_squares = HorseMoves(CurrentPiece);
            for (int index = 0; index < horse_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(horse_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == horse_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == horse_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
        //Bishop
        if (CurrentPiece.PieceNumber == 3)
        {
            List<Square> bishop_squares = BishopMoves(CurrentPiece);
            for (int index = 0; index < bishop_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(bishop_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == bishop_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == bishop_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
        //Queen
        if (CurrentPiece.PieceNumber == 4)
        {
            List<Square> queen_squares = QueenMoves(CurrentPiece);
            for (int index = 0; index < queen_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(queen_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == queen_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == queen_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
        //King
        if (CurrentPiece.PieceNumber == 5)
        {
            List<Square> king_squares = KingMoves(CurrentPiece);
            for (int index = 0; index < king_squares.Count; ++index)
            {
                EnemyAvailableMoves.Add(king_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && WhitePieces[12].currentSquare == king_squares[index])
                    KingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && BlackPieces[12].currentSquare == king_squares[index])
                    KingAttackers.Add(CurrentPiece);
            }
        }
    }
    private List<Square> PawnMoves(Piece CurrentPiece)
    {
        List<Square> PawnMovesList = new List<Square>();
        int direction = (CurrentPiece.PieceColor == 'w') ? 1 : -1;
      
        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];


        if (IsLegalMove(CurrentPiece, row, (column + direction)) && !CheckForEnemy(CurrentPiece, row, (column + direction)))
        {
            PawnMovesList.Add(squares[row, column + direction]);

            if (column == 1 && CurrentPiece.PieceColor == 'w')
            {
                PawnMovesList.Add(squares[row, column + direction * 2]);
            }
            if (column == 6 && CurrentPiece.PieceColor == 'b')
            {
                PawnMovesList.Add(squares[row, column + direction * 2]);
            }
            if (CheckForEnemy(CurrentPiece, (row + 1), (column + direction)))
            {
                PawnMovesList.Add(squares[(row + 1), (column + direction)]);
            }
            if (CheckForEnemy(CurrentPiece, (row - 1), (column + direction)))
            {
                PawnMovesList.Add(squares[(row - 1), (column + direction)]);
            }
        }

        return PawnMovesList;
    }
    private List<Square> PawnAttackMoves(Piece CurrentPiece)
    {
        List<Square> PawnAttackMoves = new List<Square>();
        int direction = (CurrentPiece.PieceColor == 'w') ? 1 : -1;

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        if (WithinBounds(row + 1, column + direction))
            PawnAttackMoves.Add(squares[(row + 1), (column + direction)]);

        if (WithinBounds(row - 1, column + direction))
            PawnAttackMoves.Add(squares[(row - 1), (column + direction)]);


        return PawnAttackMoves;
    }
    private List<Square> RookMoves(Piece CurrentPiece)
    {
        List<Square> RookMovesList = new List<Square>();

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column))
                RookMovesList.Add(squares[row + i, column]);
            else
            {
                if (WithinBounds(row + i, column))
                    EnemyAvailableMoves.Add(squares[row + i, column]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column))
                RookMovesList.Add(squares[row - i, column]);
            else
            {
                if(WithinBounds(row - i, column))
                    EnemyAvailableMoves.Add(squares[row - i, column]);

                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column + i))
                RookMovesList.Add(squares[row, column + i]);
            else
            {
                if (WithinBounds(row, column + i))
                    EnemyAvailableMoves.Add(squares[row, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column - i))
                RookMovesList.Add(squares[row, column - i]);
            else
            {
                if (WithinBounds(row, column - i))
                    EnemyAvailableMoves.Add(squares[row, column-i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row, column - i))
                break;
        }

        return RookMovesList;
    }
    private List<Square> HorseMoves(Piece CurrentPiece)
    {
        List<Square> HorseMovesList = new List<Square>();

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        if (IsLegalMove(CurrentPiece,row + 2, column + 1))
            HorseMovesList.Add(squares[row + 2, column + 1]);
        else
            if (WithinBounds(row + 2, column + 1))
                EnemyAvailableMoves.Add(squares[row + 2, column + 1]);

        if (IsLegalMove(CurrentPiece, row + 1, column + 2))
            HorseMovesList.Add(squares[row + 1, column + 2]);
        else
            if (WithinBounds(row + 1, column + 2))
            EnemyAvailableMoves.Add(squares[row + 1, column + 2]);

        if (IsLegalMove(CurrentPiece, row - 1, column + 2))
            HorseMovesList.Add(squares[row - 1, column + 2]);
        else
            if (WithinBounds(row- 1, column + 2))
            EnemyAvailableMoves.Add(squares[row - 1, column + 2]);

        if (IsLegalMove(CurrentPiece, row - 2, column + 1))
            HorseMovesList.Add(squares[row - 2, column + 1]);
        else
            if (WithinBounds(row - 2, column + 1))
            EnemyAvailableMoves.Add(squares[row - 2, column + 1]);

        if (IsLegalMove(CurrentPiece, row - 2, column - 1))
            HorseMovesList.Add(squares[row - 2, column - 1]);
        else
            if (WithinBounds(row - 2, column - 1))
            EnemyAvailableMoves.Add(squares[row - 2, column - 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column - 2))
            HorseMovesList.Add(squares[row - 1, column - 2]);
        else
            if (WithinBounds(row - 1, column - 2))
            EnemyAvailableMoves.Add(squares[row - 1, column - 2]);

        if (IsLegalMove(CurrentPiece, row + 1, column - 2))
            HorseMovesList.Add(squares[row + 1, column - 2]);
        else
            if (WithinBounds(row + 1, column - 2))
            EnemyAvailableMoves.Add(squares[row + 1, column - 2]);

        if (IsLegalMove(CurrentPiece, row + 2, column - 1))
            HorseMovesList.Add(squares[row + 2, column - 1]);
        else
            if (WithinBounds(row + 2, column - 1))
            EnemyAvailableMoves.Add(squares[row + 2, column - 1]);

        return HorseMovesList;
    }
    private List<Square> BishopMoves(Piece CurrentPiece)
    {
        List<Square> BishopMovesList = new List<Square>();

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column + i))
                BishopMovesList.Add(squares[row + i, column + i]);
            else
            {
                if (WithinBounds(row + i, column + i))
                    EnemyAvailableMoves.Add(squares[row + i, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column - i))
                BishopMovesList.Add(squares[row + i, column - i]);
            else
            {
                if (WithinBounds(row + i, column - i))
                    EnemyAvailableMoves.Add(squares[row + i, column - i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, + i, column - i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column - i))
                BishopMovesList.Add(squares[row - i, column - i]);
            else
            {
                if (WithinBounds(row - i, column - i))
                    EnemyAvailableMoves.Add(squares[row - i, column - i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column - i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column + i))
                BishopMovesList.Add(squares[row - i, column + i]);
            else
            {
                if (WithinBounds(row - i, column + i))
                    EnemyAvailableMoves.Add(squares[row - i, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column + i))
                break;
        }

        return BishopMovesList;
    }
    private List<Square> QueenMoves(Piece CurrentPiece)
    {
        List<Square> QueenMovesList = new List<Square>();

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column + i))
                QueenMovesList.Add(squares[row + i, column + i]);
            else
            {
                if (WithinBounds(row + i, column + i))
                    EnemyAvailableMoves.Add(squares[row + i, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column - i))
                QueenMovesList.Add(squares[row + i, column - i]);
            else
            {
                if (WithinBounds(row + i, column - i))
                    EnemyAvailableMoves.Add(squares[row + i, column - i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, +i, column - i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column - i))
                QueenMovesList.Add(squares[row - i, column - i]);
            else
            {
                if (WithinBounds(row - i, column - i))
                    EnemyAvailableMoves.Add(squares[row - i, column - i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column - i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column + i))
                QueenMovesList.Add(squares[row - i, column + i]);
            else
            {
                if (WithinBounds(row - i, column + i))
                    EnemyAvailableMoves.Add(squares[row - i, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row + i, column))
                QueenMovesList.Add(squares[row + i, column]);
            else
            {
                if (WithinBounds(row + i, column))
                    EnemyAvailableMoves.Add(squares[row + i, column]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row - i, column))
                QueenMovesList.Add(squares[row - i, column]);
            else
            {
                if (WithinBounds(row - i, column))
                    EnemyAvailableMoves.Add(squares[row - i, column]);

                break;
            }
            if (CheckForEnemy(CurrentPiece, row - i, column))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column + i))
                QueenMovesList.Add(squares[row, column + i]);
            else
            {
                if (WithinBounds(row, column + i))
                    EnemyAvailableMoves.Add(squares[row, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row + i, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column - i))
                QueenMovesList.Add(squares[row, column - i]);
            else
            {
                if (WithinBounds(row, column - i))
                    EnemyAvailableMoves.Add(squares[row, column - i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row, column - i))
                break;
        }

        return QueenMovesList;
    }
    private List<Square> KingMoves(Piece CurrentPiece)
    {
        List<Square> KingMovesList = new List<Square>();

        int row = ReturnRowColumn(CurrentPiece)[0];
        int column = ReturnRowColumn(CurrentPiece)[1];

        if (IsLegalMove(CurrentPiece, row + 1, column))
            KingMovesList.Add(squares[row + 1, column]);
        else
            if (WithinBounds(row + 1, column))
            EnemyAvailableMoves.Add(squares[row + 1, column]);

        if (IsLegalMove(CurrentPiece, row + 1, column - 1))
            KingMovesList.Add(squares[row + 1, column - 1]);
        else
            if (WithinBounds(row + 1, column - 1))
            EnemyAvailableMoves.Add(squares[row + 1, column - 1]);

        if (IsLegalMove(CurrentPiece, row + 1, column + 1))
            KingMovesList.Add(squares[row + 1, column + 1]);
        else
            if (WithinBounds(row + 1, column + 1))
            EnemyAvailableMoves.Add(squares[row + 1, column + 1]);

        if (IsLegalMove(CurrentPiece, row, column - 1))
            KingMovesList.Add(squares[row, column - 1]);
        else
            if (WithinBounds(row , column - 1))
            EnemyAvailableMoves.Add(squares[row, column - 1]);

        if (IsLegalMove(CurrentPiece, row, column + 1))
            KingMovesList.Add(squares[row, column + 1]);
        else
            if (WithinBounds(row , column + 1))
            EnemyAvailableMoves.Add(squares[row , column + 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column - 1))
            KingMovesList.Add(squares[row - 1, column - 1]);
        else
            if (WithinBounds(row - 1, column - 1))
            EnemyAvailableMoves.Add(squares[row - 1, column - 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column))
            KingMovesList.Add(squares[row - 1, column]);
        else
            if (WithinBounds(row - 1, column))
            EnemyAvailableMoves.Add(squares[row - 1, column]);

        if (IsLegalMove(CurrentPiece, row - 1, column + 1))
            KingMovesList.Add(squares[row - 1, column + 1]);
        else
            if (WithinBounds(row - 1, column + 1))
            EnemyAvailableMoves.Add(squares[row - 1, column + 1]);

        return KingMovesList;
    }
    private void CheckForChecks()
    {
        //single check : can king move? can king take attacker? can any piece block kingattacker or take it?
        //double check : can king move or take attacker(s)?
        foreach (Square kaka in EnemyAvailableMoves)
        {
            kaka.TransparentSquare();
        }

        KingAttackers.Clear();
        EnemyAvailableMoves.Clear();

        if (turn_counter % 2 == 0)
        {
            for (int i = 0; i < BlackPieces.Length; ++i)
            {
                if(BlackPieces[i].PieceActive)
                    EnemyMovesList(BlackPieces[i]);
            }
            foreach (Square EnemyMoves in EnemyAvailableMoves)
            {
                if (EnemyMoves == WhitePieces[12].currentSquare)
                {
                    if(KingAttackers.Count > 1)
                    {

                    }
                }
            }
        }

        if (turn_counter % 2 != 0)
        {
            for (int i = 0; i < WhitePieces.Length; ++i)
            {
                if (WhitePieces[i].PieceActive)
                    EnemyMovesList(WhitePieces[i]);
            }
        }
        foreach (Square kaka in EnemyAvailableMoves)
        {
            //kaka.HighlightSquare();
        }
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
                Square PawnTransformSquare;

                WhitePieces[i].PawnTransform = false;
                PawnTransformSquare = WhitePieces[i].currentSquare;
                WhitePieces[i].gameObject.SetActive(false);
                WhitePieces[i] = Instantiate(WhitePiecePrefab[4], WhitePieces[i].transform.position, Quaternion.identity);
                WhitePieces[i].currentSquare = PawnTransformSquare;
            }
            if (BlackPieces[i].PawnTransform)
            {
                Square PawnTransformSquare;

                BlackPieces[i].PawnTransform = false;
                PawnTransformSquare = BlackPieces[i].currentSquare;
                BlackPieces[i].gameObject.SetActive(false);
                BlackPieces[i] = Instantiate(BlackPiecePrefab[4], BlackPieces[i].transform.position, Quaternion.identity);
                BlackPieces[i].currentSquare = PawnTransformSquare;
            }
        }
    } 
}