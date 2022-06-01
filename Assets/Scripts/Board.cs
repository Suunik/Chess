using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    static private int TILE_COUNT_X = 8;
    static private int TILE_COUNT_Y = 8;

    public static Board instance;

    //Squares
    public Square squarePrefab;
    public Square[,] squares = new Square[TILE_COUNT_X, TILE_COUNT_Y];
    public List<Square> availableMoves = new List<Square>();
    private List<Square> enemyAvailableMoves = new List<Square>();
    private List<Square> kingAttackersquare = new List<Square>();
 
    //Pieces
    private Piece[] whitePieces = new Piece[16];
    private Piece[] blackPieces = new Piece[16];
    private List<Piece> kingAttackers = new List<Piece>();

    //Nupu spritede jaoks
    public Piece[] whitePiecePrefab = new Piece[6];
    public Piece[] blackPiecePrefab = new Piece[6];

    //Gameplay
    private bool check = false;

    public int turn_counter = 0;
    public int turn_flag = 0;
    public bool enPassantCheck = false;

    void Awake()
    {
        instance = this;

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
            checkForKingAttackers();

            turn_flag = turn_counter;
        }

    }

    //Spawning pieces and squares
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
        for (int i = 0; i < TILE_COUNT_Y; ++i)
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
        whitePieces[PieceNumber] = Instantiate(whitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        whitePieces[PieceNumber].currentSquare = squares[row, column];
    }
    private void SpawnSingleBlackPiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        blackPieces[PieceNumber] = Instantiate(blackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        blackPieces[PieceNumber].currentSquare = squares[row, column];
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
                {
                    if (!check)
                        availableMoves = PawnMoves(Pieceheld);
                    else
                        availableMoves = kingDefenderMoves(Pieceheld);
                }
                //ROOK Movement
                if (Pieceheld.PieceNumber == 1)
                {
                    if (!check)
                        availableMoves = RookMoves(Pieceheld);
                    else
                        availableMoves = kingDefenderMoves(Pieceheld);
                }
                //HORSE Movement
                if (Pieceheld.PieceNumber == 2)
                {
                    if (!check)
                        availableMoves = HorseMoves(Pieceheld);
                    else
                        availableMoves = kingDefenderMoves(Pieceheld);
                }
                //BISHOP Movement
                if (Pieceheld.PieceNumber == 3)
                {
                    if (!check)
                        availableMoves = BishopMoves(Pieceheld);
                    else
                        availableMoves = kingDefenderMoves(Pieceheld);
                }
                //QUEEN Movement
                if (Pieceheld.PieceNumber == 4)
                {
                    if (!check)
                        availableMoves = QueenMoves(Pieceheld);
                    else
                        availableMoves = kingDefenderMoves(Pieceheld);
                }
                //King Movement
                if (Pieceheld.PieceNumber == 5)
                    availableMoves = KingMoves(Pieceheld);

                for (int i = 0; i < availableMoves.Count; ++i)
                {
                    availableMoves[i].HighlightSquare();
                }
                Pieceheld.AvailableMovesCheck = false;
            }
        }
    }
    public void ClearAvailableMoves()
    {
        for (int i = 0; i < availableMoves.Count; ++i)
        {
            availableMoves[i].TransparentSquare();
        }

        availableMoves.Clear();
    }
    private Piece Pieceheld()
    {
        for (int i = 0; i < whitePieces.Length; ++i)
        {
            if (whitePieces[i].PieceHeld)
            {
                return whitePieces[i];
            }
        }

        for (int i = 0; i < blackPieces.Length; ++i)
        {
            if (blackPieces[i].PieceHeld)
            {
                return blackPieces[i];
            }
        }
        return null;
    }
    private void CheckForWhitePieceCapture()
    {
        for (int i = 0; i < whitePieces.Length; ++i)
        {
            if (whitePieces[i].CheckForEnemy)
            {
                for (int y = 0; y < blackPieces.Length; ++y)
                {
                    if (whitePieces[i].currentSquare == blackPieces[y].currentSquare)
                    {
                        blackPieces[y].PieceActive = false;
                    }
                    if (whitePieces[i].EnpassantSquare != null)
                    {
                        string white_square = whitePieces[i].currentSquare.ReturnSquare();
                        int white_square_horizontal = white_square[0];
                        int white_square_vertical = white_square[1];

                        if (blackPieces[y] != null)
                        {
                            string black_square = blackPieces[y].currentSquare.ReturnSquare();
                            int black_square_horizontal = black_square[0];
                            int black_square_vertical = black_square[1];

                            if (white_square_vertical - 1 == black_square_vertical && white_square_horizontal == black_square_horizontal)
                            {
                                blackPieces[y].PieceActive = false;
                                Debug.Log("Kaka");
                                break;
                            }
                        }
                    }
                }
                whitePieces[i].CheckForEnemy = false;
            }
        }
    }
    private void CheckForBlackPieceCapture()
    {
        for (int i = 0; i < blackPieces.Length; ++i)
        {
            if (blackPieces[i].CheckForEnemy)
            {
                for (int y = 0; y < whitePieces.Length; ++y)
                {
                    if (blackPieces[i].currentSquare == whitePieces[y].currentSquare)
                    {
                        whitePieces[y].PieceActive = false;
                    }
                    if (blackPieces[i].EnpassantSquare != null)
                    {
                        string black_square = blackPieces[i].currentSquare.ReturnSquare();
                        int black_square_horizontal = black_square[0];
                        int black_square_vertical = black_square[1];

                        if (blackPieces[y] != null)
                        {
                            string white_square = whitePieces[y].currentSquare.ReturnSquare();
                            int white_square_horizontal = white_square[0];
                            int white_square_vertial = white_square[1];

                            if (black_square_vertical + 1 == white_square_vertial && black_square_horizontal == white_square_horizontal)
                            {
                                whitePieces[y].PieceActive = false;
                                Debug.Log("Kaka");
                                break;
                            }
                        }
                    }
                }
                blackPieces[i].CheckForEnemy = false;
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
                for (int i = 0; i < whitePieces.Length; ++i)
                {
                    if (squares[x, y] == whitePieces[i].currentSquare)
                    {
                        return false;
                    }
                    if (Piece.PieceNumber == 5)
                    {
                        foreach (Square enemy_squares in enemyAvailableMoves)
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
                for (int i = 0; i < blackPieces.Length; ++i)
                {
                    if (squares[x, y] == blackPieces[i].currentSquare)
                    {
                        return false;
                    }
                    if (Piece.PieceNumber == 5)
                    {
                        foreach (Square enemy_squares in enemyAvailableMoves)
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
                for (int i = 0; i < blackPieces.Length; ++i)
                {
                    if (squares[x, y] == blackPieces[i].currentSquare)
                    {
                        return true;
                    }
                }
            }
            else
                for (int i = 0; i < whitePieces.Length; ++i)
                {
                    if (squares[x, y] == whitePieces[i].currentSquare)
                    {
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
                enemyAvailableMoves.Add(pawn_squares[index]);
                if(CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == pawn_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == pawn_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
        //Rook
        if (CurrentPiece.PieceNumber == 1)
        {
            List<Square> rook_squares = RookMoves(CurrentPiece);
            for (int index = 0; index < rook_squares.Count; ++index)
            {
                enemyAvailableMoves.Add(rook_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == rook_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == rook_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
        //Horse
        if (CurrentPiece.PieceNumber == 2)
        {
            List<Square> horse_squares = HorseMoves(CurrentPiece);
            for (int index = 0; index < horse_squares.Count; ++index)
            {
                enemyAvailableMoves.Add(horse_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == horse_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == horse_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
        //Bishop
        if (CurrentPiece.PieceNumber == 3)
        {
            List<Square> bishop_squares = BishopMoves(CurrentPiece);
            for (int index = 0; index < bishop_squares.Count; ++index)
            {
                enemyAvailableMoves.Add(bishop_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == bishop_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == bishop_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
        //Queen
        if (CurrentPiece.PieceNumber == 4)
        {
            List<Square> queen_squares = QueenMoves(CurrentPiece);
            for (int index = 0; index < queen_squares.Count; ++index)
            {
                enemyAvailableMoves.Add(queen_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == queen_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == queen_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
        //King
        if (CurrentPiece.PieceNumber == 5)
        {
            List<Square> king_squares = KingMoves(CurrentPiece);
            for (int index = 0; index < king_squares.Count; ++index)
            {
                enemyAvailableMoves.Add(king_squares[index]);
                if (CurrentPiece.PieceColor == 'b' && whitePieces[12].currentSquare == king_squares[index])
                    kingAttackers.Add(CurrentPiece);
                if (CurrentPiece.PieceColor == 'w' && blackPieces[12].currentSquare == king_squares[index])
                    kingAttackers.Add(CurrentPiece);
            }
        }
    }
    private void kingAttackerMoves(Piece kingAttacker,Piece enemyKing)
    {
        kingAttackersquare.Clear();
        //Kuninga asukoht
        int king_row = ReturnRowColumn(enemyKing)[0];
        int king_column = ReturnRowColumn(enemyKing)[1];
        //suund kuhu poole ruute arvutada ja kui mitu ruutu
        int attacker_direction_x =
             kingAttacker.currentSquare.ReturnSquare()[0] - enemyKing.currentSquare.ReturnSquare()[0];
        int attacker_direction_y =
             kingAttacker.currentSquare.ReturnSquare()[1] - enemyKing.currentSquare.ReturnSquare()[1];
        //all vasakul
        if(attacker_direction_x < 0 && attacker_direction_y < 0)
        {
            while(attacker_direction_x != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x + 1;
                attacker_direction_y = attacker_direction_y + 1;
            }
        }
        //vasakul
        if (attacker_direction_x < 0 && attacker_direction_y == 0)
        {
            while (attacker_direction_x != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x + 1;
            }
        }
        //yleval vasakul
        if (attacker_direction_x < 0 && attacker_direction_y > 0)
        {
            while (attacker_direction_x != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x + 1;
                attacker_direction_y = attacker_direction_y - 1;
            }
        }
        //yleval
        if (attacker_direction_x == 0 && attacker_direction_y > 0)
        {
            while (attacker_direction_y != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_y = attacker_direction_y - 1;
            }
        }
        //yleval paremal
        if (attacker_direction_x > 0 && attacker_direction_y > 0)
        {
            while (attacker_direction_x != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x - 1;
                attacker_direction_y = attacker_direction_y - 1;
            }
        }
        //paremal
        if (attacker_direction_x > 0 && attacker_direction_y == 0)
        {
            while (attacker_direction_x != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x - 1;
            }
        }
        //all paremal
        if (attacker_direction_x > 0 && attacker_direction_y < 0)
        {
            while (attacker_direction_y != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_x = attacker_direction_x - 1;
                attacker_direction_y = attacker_direction_y + 1;
            }
        }
        //all
        if (attacker_direction_x == 0 && attacker_direction_y < 0)
        {
            while (attacker_direction_y != 0)
            {
                kingAttackersquare.Add(squares[king_row + attacker_direction_x, king_column + attacker_direction_y]);
                attacker_direction_y = attacker_direction_y + 1;
            }
        }
        //hobune
        if(kingAttacker.PieceNumber == 2)
        {
            kingAttackersquare.Add(kingAttacker.currentSquare);
        }

        foreach (Square attacker_square in kingAttackersquare)
        {
            Debug.Log("Attacker square: " + attacker_square.ReturnSquare());
        }
    }
    private List<Square> kingDefenderMoves(Piece CurrentPiece)
    {
        List<Square> king_defender_square = new List<Square>();
        //Pawn
        if (CurrentPiece.PieceNumber == 0)
        {
            List<Square> pawn_squares = PawnMoves(CurrentPiece);
            List<Square> pawn_attack_squares = PawnAttackMoves(CurrentPiece);
            //Kui saab ette astuda
            for (int index = 0; index < pawn_squares.Count; ++index)
            {
                foreach (Square attacker_Square in kingAttackersquare)
                {
                    if (pawn_squares[index] == attacker_Square)
                    {
                        king_defender_square.Add(attacker_Square);
                    }
                }
            }
            //kui saab syya
            for (int index = 0; index < pawn_attack_squares.Count; ++index)
            {
                foreach (Piece king_attacker in kingAttackers)
                {
                    if (pawn_attack_squares[index] == king_attacker.currentSquare)
                    {
                        king_defender_square.Add(king_attacker.currentSquare);
                    }
                }
            }
        }
        //Rook
        if (CurrentPiece.PieceNumber == 1)
        {
            List<Square> rook_squares = RookMoves(CurrentPiece);
            for (int index = 0; index < rook_squares.Count; ++index)
            {
                foreach (Square attacker_Square in kingAttackersquare)
                {
                    if (rook_squares[index] == attacker_Square)
                    {
                        king_defender_square.Add(attacker_Square);
                    }
                }
            }
        }
        //Horse
        if (CurrentPiece.PieceNumber == 2)
        {
            List<Square> horse_squares = HorseMoves(CurrentPiece);
            for (int index = 0; index < horse_squares.Count; ++index)
            {
                foreach (Square attacker_Square in kingAttackersquare)
                {
                    if (horse_squares[index] == attacker_Square)
                    {
                        king_defender_square.Add(attacker_Square);
                    }
                }
            }
        }
        //Bishop
        if (CurrentPiece.PieceNumber == 3)
        {
            List<Square> bishop_squares = BishopMoves(CurrentPiece);
            for (int index = 0; index < bishop_squares.Count; ++index)
            {
                foreach (Square attacker_Square in kingAttackersquare)
                {
                    if (bishop_squares[index] == attacker_Square)
                    {
                        king_defender_square.Add(attacker_Square);
                    }
                }
            }
        }
        //Queen
        if (CurrentPiece.PieceNumber == 4)
        {
            List<Square> queen_squares = QueenMoves(CurrentPiece);
            for (int index = 0; index < queen_squares.Count; ++index)
            {
                foreach (Square attacker_Square in kingAttackersquare)
                {
                    if (queen_squares[index] == attacker_Square)
                    {
                        king_defender_square.Add(attacker_Square);
                    }
                }
            }
        }

        return king_defender_square;
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

            if (column == 1 && CurrentPiece.PieceColor == 'w' && !CheckForEnemy(CurrentPiece, row, column + direction * 2))
            {
                PawnMovesList.Add(squares[row, column + direction * 2]);
            }
            if (column == 6 && CurrentPiece.PieceColor == 'b' && !CheckForEnemy(CurrentPiece, row, column + direction * 2))
            {
                PawnMovesList.Add(squares[row, column + direction * 2]);
            }
        }
        if (CheckForEnemy(CurrentPiece, (row + 1), (column + direction)))
        {
            PawnMovesList.Add(squares[(row + 1), (column + direction)]);
        }
        if (CheckForEnemy(CurrentPiece, (row - 1), (column + direction)))
        {
            PawnMovesList.Add(squares[(row - 1), (column + direction)]);
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
                    enemyAvailableMoves.Add(squares[row + i, column]);
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
                    enemyAvailableMoves.Add(squares[row - i, column]);

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
                    enemyAvailableMoves.Add(squares[row, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row , column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column - i))
                RookMovesList.Add(squares[row, column - i]);
            else
            {
                if (WithinBounds(row, column - i))
                    enemyAvailableMoves.Add(squares[row, column-i]);
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
                enemyAvailableMoves.Add(squares[row + 2, column + 1]);

        if (IsLegalMove(CurrentPiece, row + 1, column + 2))
            HorseMovesList.Add(squares[row + 1, column + 2]);
        else
            if (WithinBounds(row + 1, column + 2))
            enemyAvailableMoves.Add(squares[row + 1, column + 2]);

        if (IsLegalMove(CurrentPiece, row - 1, column + 2))
            HorseMovesList.Add(squares[row - 1, column + 2]);
        else
            if (WithinBounds(row- 1, column + 2))
            enemyAvailableMoves.Add(squares[row - 1, column + 2]);

        if (IsLegalMove(CurrentPiece, row - 2, column + 1))
            HorseMovesList.Add(squares[row - 2, column + 1]);
        else
            if (WithinBounds(row - 2, column + 1))
            enemyAvailableMoves.Add(squares[row - 2, column + 1]);

        if (IsLegalMove(CurrentPiece, row - 2, column - 1))
            HorseMovesList.Add(squares[row - 2, column - 1]);
        else
            if (WithinBounds(row - 2, column - 1))
            enemyAvailableMoves.Add(squares[row - 2, column - 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column - 2))
            HorseMovesList.Add(squares[row - 1, column - 2]);
        else
            if (WithinBounds(row - 1, column - 2))
            enemyAvailableMoves.Add(squares[row - 1, column - 2]);

        if (IsLegalMove(CurrentPiece, row + 1, column - 2))
            HorseMovesList.Add(squares[row + 1, column - 2]);
        else
            if (WithinBounds(row + 1, column - 2))
            enemyAvailableMoves.Add(squares[row + 1, column - 2]);

        if (IsLegalMove(CurrentPiece, row + 2, column - 1))
            HorseMovesList.Add(squares[row + 2, column - 1]);
        else
            if (WithinBounds(row + 2, column - 1))
            enemyAvailableMoves.Add(squares[row + 2, column - 1]);

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
                    enemyAvailableMoves.Add(squares[row + i, column + i]);
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
                    enemyAvailableMoves.Add(squares[row + i, column - i]);
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
                    enemyAvailableMoves.Add(squares[row - i, column - i]);
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
                    enemyAvailableMoves.Add(squares[row - i, column + i]);
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
                    enemyAvailableMoves.Add(squares[row + i, column + i]);
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
                    enemyAvailableMoves.Add(squares[row + i, column - i]);
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
                    enemyAvailableMoves.Add(squares[row - i, column - i]);
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
                    enemyAvailableMoves.Add(squares[row - i, column + i]);
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
                    enemyAvailableMoves.Add(squares[row + i, column]);
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
                    enemyAvailableMoves.Add(squares[row - i, column]);

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
                    enemyAvailableMoves.Add(squares[row, column + i]);
                break;
            }
            if (CheckForEnemy(CurrentPiece, row, column + i))
                break;
        }
        for (int i = 1; i < 8; ++i)
        {
            if (IsLegalMove(CurrentPiece, row, column - i))
                QueenMovesList.Add(squares[row, column - i]);
            else
            {
                if (WithinBounds(row, column - i))
                    enemyAvailableMoves.Add(squares[row, column - i]);
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
            enemyAvailableMoves.Add(squares[row + 1, column]);

        if (IsLegalMove(CurrentPiece, row + 1, column - 1))
            KingMovesList.Add(squares[row + 1, column - 1]);
        else
            if (WithinBounds(row + 1, column - 1))
            enemyAvailableMoves.Add(squares[row + 1, column - 1]);

        if (IsLegalMove(CurrentPiece, row + 1, column + 1))
            KingMovesList.Add(squares[row + 1, column + 1]);
        else
            if (WithinBounds(row + 1, column + 1))
            enemyAvailableMoves.Add(squares[row + 1, column + 1]);

        if (IsLegalMove(CurrentPiece, row, column - 1))
            KingMovesList.Add(squares[row, column - 1]);
        else
            if (WithinBounds(row , column - 1))
            enemyAvailableMoves.Add(squares[row, column - 1]);

        if (IsLegalMove(CurrentPiece, row, column + 1))
            KingMovesList.Add(squares[row, column + 1]);
        else
            if (WithinBounds(row , column + 1))
            enemyAvailableMoves.Add(squares[row , column + 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column - 1))
            KingMovesList.Add(squares[row - 1, column - 1]);
        else
            if (WithinBounds(row - 1, column - 1))
            enemyAvailableMoves.Add(squares[row - 1, column - 1]);

        if (IsLegalMove(CurrentPiece, row - 1, column))
            KingMovesList.Add(squares[row - 1, column]);
        else
            if (WithinBounds(row - 1, column))
            enemyAvailableMoves.Add(squares[row - 1, column]);

        if (IsLegalMove(CurrentPiece, row - 1, column + 1))
            KingMovesList.Add(squares[row - 1, column + 1]);
        else
            if (WithinBounds(row - 1, column + 1))
            enemyAvailableMoves.Add(squares[row - 1, column + 1]);

        return KingMovesList;
    }
    private void checkForKingAttackers()
    {
        //single check : can king move? can king take attacker? can any piece block kingattacker or take it?
        //double check : can king move or take attacker(s)?
        foreach (Square kaka in enemyAvailableMoves)
        {
            kaka.TransparentSquare();
        }

        kingAttackers.Clear();
        enemyAvailableMoves.Clear();

        if (turn_counter % 2 == 0)
        {
            for (int i = 0; i < blackPieces.Length; ++i)
            {
                if(blackPieces[i].PieceActive)
                    EnemyMovesList(blackPieces[i]);
            }
            foreach (Square EnemyMoves in enemyAvailableMoves)
            {
                if (EnemyMoves == whitePieces[12].currentSquare)
                {
                    check = true;
                    //teeb attackerite movedest listi
                    kingAttackerMoves(kingAttackers[0], whitePieces[12]);
                }
            }
        }

        if (turn_counter % 2 != 0)
        {
            for (int i = 0; i < whitePieces.Length; ++i)
            {
                if (whitePieces[i].PieceActive)
                    EnemyMovesList(whitePieces[i]);
            }
            foreach (Square EnemyMoves in enemyAvailableMoves)
            {
                if (EnemyMoves == blackPieces[12].currentSquare)
                {
                    check = true;
                    //teeb attackerite movedest listi
                    kingAttackerMoves(kingAttackers[0], whitePieces[12]);
                }
            }
        }
        if (kingAttackers.Count == 0)
        {
            check = false;
        }
    }
    //Special Moves
    private void CheckForCastle()
    {
        if (whitePieces[12].CastleTime)
        {
            if (whitePieces[12].currentSquare == squares[0, 6])
            {
                whitePieces[12].CastleTime = false;
                whitePieces[15].transform.position = squares[0, 5].transform.position;
                whitePieces[15].currentSquare = squares[0, 5];
                whitePieces[15].PieceHasMoved = true;
            }
            if (whitePieces[12].currentSquare == squares[0, 2])
            {
                whitePieces[12].CastleTime = false;
                whitePieces[8].transform.position = squares[0, 3].transform.position;
                whitePieces[8].currentSquare = squares[0, 3];
                whitePieces[8].PieceHasMoved = true;
            }
        }
        if (blackPieces[12].CastleTime)
        {
            if (blackPieces[12].currentSquare == squares[7, 6])
            {
                blackPieces[12].CastleTime = false;
                blackPieces[15].transform.position = squares[7, 5].transform.position;
                blackPieces[15].currentSquare = squares[7, 5];
                blackPieces[15].PieceHasMoved = true;
            }
            if (blackPieces[12].currentSquare == squares[7, 2])
            {
                blackPieces[12].CastleTime = false;
                blackPieces[8].transform.position = squares[7, 3].transform.position;
                blackPieces[8].currentSquare = squares[7, 3];
                blackPieces[8].PieceHasMoved = true;
            }
        }
    }
    private void CheckForPawnTransform()
    {
        for(int i = 0; i < 8; ++i)
        {
            if(whitePieces[i].PawnTransform)
            {
                Square PawnTransformSquare;

                whitePieces[i].PawnTransform = false;
                PawnTransformSquare = whitePieces[i].currentSquare;
                whitePieces[i].gameObject.SetActive(false);
                whitePieces[i] = Instantiate(whitePiecePrefab[4], whitePieces[i].transform.position, Quaternion.identity);
                whitePieces[i].currentSquare = PawnTransformSquare;
            }
            if (blackPieces[i].PawnTransform)
            {
                Square PawnTransformSquare;

                blackPieces[i].PawnTransform = false;
                PawnTransformSquare = blackPieces[i].currentSquare;
                blackPieces[i].gameObject.SetActive(false);
                blackPieces[i] = Instantiate(blackPiecePrefab[4], blackPieces[i].transform.position, Quaternion.identity);
                blackPieces[i].currentSquare = PawnTransformSquare;
            }
        }
    } 
}