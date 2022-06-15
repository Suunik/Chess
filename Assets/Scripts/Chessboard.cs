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
    public List<ChessPiece> whitePieces;
    public List<ChessPiece> blackPieces;
    public List<Square> allWhiteMoves = null;
    public List<Square> allBlackMoves = null;
    public Square whiteKingSquare;
    public Square blackKingSquare;

    //For piece sprites
    public ChessPiece[] whitePiecePrefab = new ChessPiece[6];
    public ChessPiece[] blackPiecePrefab = new ChessPiece[6];

    public Square squarePrefab;

    public int turnCounter = 1;
    public int previousTurnCounter = 0;
    private string previousPosition;

    public Square enPassantSquare = null;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Create all tile objects and assign them a value e.g. d3
        SpawnSquares();

        spawnFENPosition("rhbqkbhr/pppppppp/8/8/8/8/PPPPPPPP/RHBQKBHR");
    }

    // Update is called once per frame
    void Update()
    {

        if (turnCounter != previousTurnCounter)
        {
            checkForEnPassant(previousPosition);
            //This could be modified to clear all blackPiece availableMoves arrays if its white's turn and vice versa
            foreach (ChessPiece item in whitePieces)
            {
                item.availableMoves.Clear();
                //Calculate availablemoves for every piece once per turn
                item.FindAvailableMoves();
                allWhiteMoves.AddRange(item.findPieceAttackingMoves());
                //After getting all unrestricted movement arrays it is now possible to restrict movements
                item.restrictMovements();
            }

            foreach (ChessPiece item in blackPieces)
            {
                item.availableMoves.Clear();
                //Calculate availablemoves for every piece once per turn
                item.FindAvailableMoves();
                allBlackMoves.AddRange(item.findPieceAttackingMoves());
                //After getting all unrestricted movement arrays it is now possible to restrict movements
                item.restrictMovements();
            }
            previousTurnCounter = turnCounter;

            Debug.Log(generateFEN());
            //remember the position
            previousPosition = generateFEN();
        }

        //Instead of calling piecemovement() in the ChessPiece class update(), having the board to all updates
        //is easier to manage
        foreach (ChessPiece item in whitePieces)
        {
            item.PieceMovement();
        }
        foreach (ChessPiece item in blackPieces)
        {
            item.PieceMovement();
        }

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
        whitePieces[PieceNumber].team = 1;
        whitePieces[PieceNumber].currentSquare.team = 1;

        squares[row, column].pieceOnSquare = whitePieces[PieceNumber].pieceLetter;
    }
    private void SpawnSingleBlackPiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        blackPieces[PieceNumber] = Instantiate(blackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        blackPieces[PieceNumber].currentSquare = squares[row, column];
        blackPieces[PieceNumber].team = -1;
        blackPieces[PieceNumber].currentSquare.team = -1;

        squares[row, column].pieceOnSquare = blackPieces[PieceNumber].pieceLetter;
    }

    //Finds all in bounds and no collision moves for every piece in a team
    //Returns a list of tiles a team could attack
    public List<Square> allTeamCoveredSquares(int team)
    {
        List<Square> result = new List<Square>();
        //For this list we need inbounds and no collision squares + pawn attack moves
        if (team == 1)
        {
            foreach (ChessPiece item in whitePieces)
            {
                result.AddRange(item.findPieceAttackingMoves());
            }
        }
        else if (team == -1)
        {
            foreach (ChessPiece item in blackPieces)
            {
                result.AddRange(item.findPieceAttackingMoves());
            }
        }

        return result;
    }
    //for the main position
    private string firstFENField()
    {
        string FEN = "";

        //starts counting squares from up left
        //finishes on bottom right
        for (int y = 7; y >= 0; --y)
        {
            //remember how many empty squares there are between pieces
            int emptySquareCount = 0;
            for (int x = 0; x < TILE_COUNT_X; ++x)
            {
                //if square is not empty
                if (squares[x, y].pieceOnSquare != '0')
                {
                    //if there has been empty space
                    if (emptySquareCount != 0)
                    {
                        //add emptySquareCount to the string
                        FEN = FEN + (char)(48 + emptySquareCount);
                        //reset the count
                        emptySquareCount = 0;
                    }
                    //add piece letter to the FEN code
                    FEN = FEN + squares[x, y].pieceOnSquare;
                }
                //if square is empty
                if (squares[x, y].pieceOnSquare == '0')
                {
                    //add to the count
                    ++emptySquareCount;
                }
            }
            //if whole row is empty add it to the string
            if (emptySquareCount != 0)
            {
                FEN = FEN + (char)(48 + emptySquareCount);
            }
            
            if (y > 0)
            {
                //new column
                FEN = FEN + '/';
            }
            //skips the last '/' symbol and adds whitespace instead
            else
            {
                FEN = FEN + ' ';
            }
        }
        return FEN;
    }
    //who can move this turn
    private string secondFENField()
    {
        string FEN = "";
        if (turnCounter % 2 != 0)
        {
            FEN = FEN + 'w' + ' ';
        }
        if (turnCounter % 2 == 0)
        {
            FEN = FEN + 'b' + ' ';
        }
        return FEN;
    }
    //for castling
    private string thirdFENField()
    {
        string FEN = "";

        FEN = FEN + '-' + ' ';
        return FEN;
    }
    //for en passant
    private string fourthFENField()
    {
        string FEN = "";

        FEN = FEN + '-' + ' ';
        return FEN;
    }
    //turn counter
    private string fifthFENField()
    {
        string FEN = "";

        FEN = "" + (char)(turnCounter + 48);

        return FEN;
    }

    private string generateFEN()
    {
        string FEN = "";

        FEN = FEN + firstFENField();
        FEN = FEN + secondFENField();
        FEN = FEN + thirdFENField();
        FEN = FEN + fourthFENField();
        FEN = FEN + fifthFENField();

        return FEN;
    }

    private void spawnFENPosition(string FEN)
    {
        //place where the checking begins
        int row = 0;
        int column = 7;
        //Piece numbers for white and black(needed for piece arrays)
        int white_piece_number = 0;
        int black_piece_number = 0;
        //loop the whole FEN code
        for (int i = 0; i < FEN.Length; ++i)
        {
            //if white piece
            //spawn corresponding piece on corresponding square
            if (FEN[i] > 'A' && FEN[i] < 'Z')
            {
                if (FEN[i] == 'P')
                    SpawnSingleWhitePiece(white_piece_number, 0, column, row);
                if (FEN[i] == 'R')
                    SpawnSingleWhitePiece(white_piece_number, 1, column, row);
                if (FEN[i] == 'H')
                    SpawnSingleWhitePiece(white_piece_number, 2, column, row);
                if (FEN[i] == 'B')
                    SpawnSingleWhitePiece(white_piece_number, 3, column, row);
                if (FEN[i] == 'Q')
                    SpawnSingleWhitePiece(white_piece_number, 4, column, row);
                if (FEN[i] == 'K')
                    SpawnSingleWhitePiece(white_piece_number, 5, column, row);

                ++white_piece_number;
                //next spot on the board
                row = row + 1;
            }
            //if black piece
            //spawn corresponding piece on corresponding square
            if (FEN[i] > 'a' && FEN[i] < 'z')
            {
                if (FEN[i] == 'p')
                    SpawnSingleBlackPiece(black_piece_number, 0, column, row);
                if (FEN[i] == 'r')
                    SpawnSingleBlackPiece(black_piece_number, 1, column, row);
                if (FEN[i] == 'h')
                    SpawnSingleBlackPiece(black_piece_number, 2, column, row);
                if (FEN[i] == 'b')
                    SpawnSingleBlackPiece(black_piece_number, 3, column, row);
                if (FEN[i] == 'q')
                    SpawnSingleBlackPiece(black_piece_number, 4, column, row);
                if (FEN[i] == 'k')
                    SpawnSingleBlackPiece(black_piece_number, 5, column, row);

                ++black_piece_number;
                //next spot on the board
                row = row + 1;
            }
            //if currently checked spot is a character number less than 8
            //means that there are empty space on board
            if (FEN[i] >= '0' && FEN[i] <= '8')
            {
                //how many empty rows there are
                int emptyRows = FEN[i] - 48;
                //skip that many rows
                row = row + emptyRows;
            }
            //new column
            if (FEN[i] == '/')
            {
                column = column - 1;
                row = 0;
            }
        }
    }

    private void checkForEnPassant(string previousPosition)
    {
       
    }
}
