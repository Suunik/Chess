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
    public List<ChessPiece> whitePieces = new List<ChessPiece>();
    public List<ChessPiece> blackPieces = new List<ChessPiece>();
    public List<Square> allWhiteMoves = null;
    public List<Square> allBlackMoves = null;
    public Square whiteKingSquare;
    public Square blackKingSquare;

    //For piece sprites
    public ChessPiece[] whitePiecePrefab = new ChessPiece[6];
    public ChessPiece[] blackPiecePrefab = new ChessPiece[6];

    public Square squarePrefab;

    //Game controlling
    public int turnCounter = 1;
    public int previousTurnCounter = 0;
    private string previousPosition;

    public List<Square[]> moveList = new List<Square[]>();

    //special moves
    //En Passant
    public string enPassantSquare = "-";
    public bool enPassantForFEN = false;
    //Castling
    public List<string> castleSquare = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Create all tile objects and assign them a value e.g. d3
        SpawnSquares();

        spawnFENPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        //spawnFENPosition("r1b1kb1r / pp1p2pp / 2pqpn2 / 4Pp2 / 1n3Q1P / 3P2PN / PPP1KP2 / RNB2B1R ");
    }

    // Update is called once per frame
    void Update()
    {
        if (turnCounter != previousTurnCounter)
        {
            processSuccessfulEnPassant();
            checkForEnPassant();
            processSuccessfulCastle();
            castleSquare.Clear();
            //This could be modified to clear all blackPiece availableMoves arrays if its white's turn and vice versa
            foreach (ChessPiece item in whitePieces)
            {
                checkForPawnTransformation(item);
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
    private void SpawnSingleWhitePiece(int PieceNumber, int PiecePrefab, int column, int row)
    {
        ChessPiece whiteObject = Instantiate(whitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);
        whitePieces.Add(whiteObject);
        whitePieces[whitePieces.Count - 1].currentSquare = squares[row, column];
        whitePieces[whitePieces.Count - 1].team = 1;
        whitePieces[whitePieces.Count - 1].currentSquare.team = 1;

        squares[row, column].pieceOnSquare = whitePieces[whitePieces.Count - 1].pieceLetter;
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

        if (enPassantForFEN)
        {
            FEN = FEN + enPassantSquare + ' ';
            enPassantForFEN = false;
        }

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
                if (FEN[i] == 'N')
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
                if (FEN[i] == 'n')
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
    //special moves
    private void checkForEnPassant()
    {
        if(moveList.Count == 0)
        {
            return;
        }
        if(moveList[moveList.Count - 1][1].pieceOnSquare == 'P' || moveList[moveList.Count - 1][1].pieceOnSquare == 'p')
        {
            int squareJump = (moveList[moveList.Count - 1][1].pieceOnSquare == 'P') ? 2 : -2;
            string start_square = moveList[moveList.Count - 1][0].ReturnSquare();
            string end_square = moveList[moveList.Count - 1][1].ReturnSquare();

            if (start_square[1] + squareJump == end_square[1])
            {
                enPassantSquare = "" + start_square[0] + (char)(start_square[1] + (squareJump/2));        
            }
            else
            {
                enPassantSquare = "-";
            }
        }
        else
        {
            enPassantSquare = "-";         
        }
    }
    private void processSuccessfulEnPassant()
    {
        if(moveList.Count == 0)
        {
            return;
        }
        //if there is a enPassant square
        if(enPassantSquare != "-")
        {
            //if last move was made by pawn
            if (moveList[moveList.Count - 1][1].pieceOnSquare == 'P' || moveList[moveList.Count - 1][1].pieceOnSquare == 'p')
            {
                //if last move was made to enpassant square
                if (moveList[moveList.Count - 1][1].ReturnSquare() == enPassantSquare)
                {
                    //check for the enemy piece to delete
                    for(int i = 0; i < whitePieces.Count; ++i)
                    {
                        string arg1 = whitePieces[i].currentSquare.ReturnSquare();
                        string arg2 = "";
                        arg2 += enPassantSquare[0];
                        arg2 += (char)(enPassantSquare[1] + 1);
                        if (arg1 == arg2)
                        {
                            whitePieces[i].currentSquare.team = 0;
                            whitePieces[i].killYourself();
                            whitePieces.Remove(whitePieces[i]);
                            break;
                        }
                    }
                    //check for the enemy piece to delete
                    for (int i = 0; i < blackPieces.Count; ++i)
                    {
                        string arg1 = blackPieces[i].currentSquare.ReturnSquare();
                        string arg2 = "";
                        arg2 += enPassantSquare[0];
                        arg2 += (char)(enPassantSquare[1] - 1);

                        if ( arg1==arg2 )
                        {
                            blackPieces[i].currentSquare.team = 0;
                            blackPieces[i].killYourself();
                            blackPieces.Remove(blackPieces[i]);
                            break;
                        }
                    }
                }
            }
        }
    }
    public List<ChessPiece> findAvailableRookForCastleing()
    {
        List<ChessPiece> rooks_found = new List<ChessPiece>();

        foreach(ChessPiece piece in whitePieces)
        {
            if (piece.firstMove)
            {
                if (piece.pieceLetter == 'R')
                {
                    rooks_found.Add(piece);
                }
            }
        }
        foreach (ChessPiece piece in blackPieces)
        {
            if (piece.firstMove)
            {
                if (piece.pieceLetter == 'r')
                {
                    rooks_found.Add(piece);
                }
            }
        }
        return rooks_found;
    }
    public void processSuccessfulCastle()
    {
        //returns if first move
        if(moveList.Count == 0)
        {
            return;
        }

        //check if there is a castleing opportunity
        if(castleSquare.Count != 0)
        {
            //check if last move was made by king
            if(moveList[moveList.Count - 1][1].pieceOnSquare == 'K' || moveList[moveList.Count - 1][1].pieceOnSquare == 'k')
            {
                //loops through all possible castleingsquares (Should be max 2)
                foreach (string castlingSquare in castleSquare)
                {
                    //if last move was made by king onto castleing square
                    if (moveList[moveList.Count - 1][1].ReturnSquare() == castlingSquare)
                    {
                        //check which side king moved to
                        //Comparing kings original position against kings current position
                        //calling returnsquare function and comparing only the rows
                        if (moveList[moveList.Count - 1][0].ReturnSquare()[0] < moveList[moveList.Count - 1][1].ReturnSquare()[0])
                        {
                            if (moveList[moveList.Count - 1][1].pieceOnSquare == 'K')
                            {
                                //find the rook thats on the right
                                //loop through all the pieces
                                foreach (ChessPiece piece in whitePieces)
                                {
                                    //if we found a rook
                                    if (piece.pieceLetter == 'R')
                                    {
                                        //check if its on the right side
                                        if (moveList[moveList.Count - 1][1].ReturnSquare()[0] < piece.currentSquare.ReturnSquare()[0])
                                        {
                                            //forget rooks current squares team and letter
                                            piece.currentSquare.team = 0;
                                            piece.currentSquare.pieceOnSquare = '0';
                                            //move the rook to the other side of the king
                                            piece.transform.position = squares[(castlingSquare[0] - 97 - 1), 0].transform.position;
                                            //assign new square to the rook
                                            piece.currentSquare = squares[(castlingSquare[0] - 97 - 1), 0];
                                            piece.currentSquare.team = piece.team;
                                            piece.currentSquare.pieceOnSquare = 'R';
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChessPiece piece in blackPieces)
                                {
                                    //if we found a rook
                                    if (piece.pieceLetter == 'r')
                                    {
                                        //check if its on the right side
                                        if (moveList[moveList.Count - 1][1].ReturnSquare()[0] < piece.currentSquare.ReturnSquare()[0])
                                        {
                                            //forget rooks current squares team and letter
                                            piece.currentSquare.team = 0;
                                            piece.currentSquare.pieceOnSquare = '0';
                                            //move the rook to the other side of the king
                                            piece.transform.position = squares[(castlingSquare[0] - 97 - 1), 7].transform.position;
                                            //assign new square to the rook
                                            piece.currentSquare = squares[(castlingSquare[0] - 97 - 1), 7];
                                            piece.currentSquare.team = piece.team;
                                            piece.currentSquare.pieceOnSquare = 'r';
                                        }
                                    }
                                }
                            }

                        }
                        //if king moved to the left
                        if (moveList[moveList.Count - 1][0].ReturnSquare()[0] > moveList[moveList.Count - 1][1].ReturnSquare()[0])
                        {
                            if (moveList[moveList.Count - 1][1].pieceOnSquare == 'K')
                            {
                                //find the rook thats on the left
                                //loop through all the pieces
                                foreach (ChessPiece piece in whitePieces)
                                {
                                    //if we found a rook
                                    if (piece.pieceLetter == 'R')
                                    {
                                        //check if its on the right side
                                        if (moveList[moveList.Count - 1][1].ReturnSquare()[0] > piece.currentSquare.ReturnSquare()[0])
                                        {
                                            //forget rooks current squares team and letter
                                            piece.currentSquare.team = 0;
                                            piece.currentSquare.pieceOnSquare = '0';
                                            //move the rook to the other side of the king
                                            piece.transform.position = squares[(castlingSquare[0] - 97 + 1), 0].transform.position;
                                            //assign new square to the rook
                                            piece.currentSquare = squares[(castlingSquare[0] - 97 + 1), 0];
                                            piece.currentSquare.team = piece.team;
                                            piece.currentSquare.pieceOnSquare = 'R';
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ChessPiece piece in blackPieces)
                                {
                                    //if we found a rook
                                    if (piece.pieceLetter == 'r')
                                    {
                                        //check if its on the right side
                                        if (moveList[moveList.Count - 1][1].ReturnSquare()[0] > piece.currentSquare.ReturnSquare()[0])
                                        {
                                            //forget rooks current squares team and letter
                                            piece.currentSquare.team = 0;
                                            piece.currentSquare.pieceOnSquare = '0';
                                            //move the rook to the other side of the king
                                            piece.transform.position = squares[(castlingSquare[0] - 97 + 1), 7].transform.position;
                                            //assign new square to the rook
                                            piece.currentSquare = squares[(castlingSquare[0] - 97 + 1), 7];
                                            piece.currentSquare.team = piece.team;
                                            piece.currentSquare.pieceOnSquare = 'r';
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void checkForPawnTransformation(ChessPiece piece)
    {
        if (piece.pieceLetter == 'P')
        {
            int pawn_row = piece.currentSquare.ReturnSquare()[0] - 97;
            int pawn_column = piece.currentSquare.ReturnSquare()[1] - 49;
            
            int pieceNumber = whitePieces.Count;

            if (pawn_column == 7)
            {
                Debug.Log("White pieces count before deleting: " + whitePieces.Count);
                whitePieces.Remove(piece);
                piece.killYourself();
                Debug.Log("White pieces count after deleting: " + whitePieces.Count);
                SpawnSingleWhitePiece(whitePieces.Count, 4, pawn_column, pawn_row);
            }
        }
    }

}
