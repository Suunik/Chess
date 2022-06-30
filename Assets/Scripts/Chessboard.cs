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

    //Game controlling
    public int turnCounter = 1;
    public int previousTurnCounter = 0;
    bool whiteTurn = true;

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

        spawnFENPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1");
        //spawnFENPosition("r1bqkbnr/ppppp2p/n5p1/4Pp2/8/8/PPPPKPPP/RNBQ1BNR w kq f6 7");
    }

    // Update is called once per frame
    void Update()
    {
        if (turnCounter != previousTurnCounter)
        {
            Debug.Log("whiteturn is: " + whiteTurn);
            Debug.Log("EnPassantSquare is: " + enPassantSquare);
            Debug.Log("turncounter: " + turnCounter);
            foreach (ChessPiece item in whitePieces)
            {
                if(item.pieceLetter =='K')
                Debug.Log("white king firstmove: " + item.firstMove);
                if(item.pieceLetter == 'R')
                Debug.Log("white rook firstmove: " + item.firstMove);
            }
            foreach(ChessPiece item in blackPieces)
            {
                if (item.pieceLetter == 'k')
                    Debug.Log("black king firstmove: " + item.firstMove);
                if (item.pieceLetter == 'r')
                    Debug.Log("black rook firstmove: " + item.firstMove);
            }
            List<ChessPiece> pieces = (whiteTurn) ? whitePieces : blackPieces;
            //clear previous moves
            foreach(ChessPiece item in pieces)
            {
                item.availableMoves.Clear();
            }
            //change the turn
            whiteTurn = (turnCounter % 2 != 0) ? true : false;
            //special move management
            processSuccessfulEnPassant();
            checkForEnPassant();
            processSuccessfulCastle();
            castleSquare.Clear();

            pieces = (whiteTurn) ? whitePieces : blackPieces;

            foreach (ChessPiece item in pieces)
            {
                checkForPawnTransformation(item);
                //Calculate availablemoves for every piece once per turn
                item.FindAvailableMoves();
                //After getting all unrestricted movement arrays it is now possible to restrict movements
                item.restrictMovements();
            }
            previousTurnCounter = turnCounter;
            Debug.Log(generateFEN());
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
    private void SpawnSingleWhitePiece(int PiecePrefab, int column, int row)
    {
        ChessPiece whitePiece  = Instantiate(whitePiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        whitePieces.Add(whitePiece);

        whitePieces[whitePieces.Count - 1].currentSquare = squares[row, column];
        whitePieces[whitePieces.Count - 1].team = 1;
        whitePieces[whitePieces.Count - 1].currentSquare.team = 1;

        squares[row, column].pieceOnSquare = whitePieces[whitePieces.Count - 1].pieceLetter;
    }
    private void SpawnSingleBlackPiece(int PiecePrefab, int column, int row)
    {
        ChessPiece blackPiece = Instantiate(blackPiecePrefab[PiecePrefab], squares[row, column].transform.position, Quaternion.identity);

        blackPieces.Add(blackPiece);
        blackPieces[blackPieces.Count - 1].currentSquare = squares[row, column];
        blackPieces[blackPieces.Count - 1].team = -1;
        blackPieces[blackPieces.Count - 1].currentSquare.team = -1;

        squares[row, column].pieceOnSquare = blackPieces[blackPieces.Count - 1].pieceLetter;
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

    //FEN CODE
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
        bool white_king_first_move = true;
        bool black_king_first_move = true;

        int castling_available = 0;
        //check for white castling
        foreach (ChessPiece whitePiece in whitePieces)
        {
            //if found the king
            if (whitePiece.pieceLetter == 'K')
            {
                //if king has moved
                if (!whitePiece.firstMove)
                {
                    //remember it has moved
                    white_king_first_move = false;
                    //break from the loop, because cant castle if king has moved
                    break;
                }

                for (int i = (whitePieces.Count - 1); i >= 0; --i)
                {
                    //find a rook and check if the rook has moved
                    //if rook has moved, skip
                    if (whitePieces[i].pieceLetter == 'R' && whitePieces[i].firstMove == true)
                    {
                        //if its the right rook
                        if (whitePieces[i].currentSquare.ReturnSquare()[0] == 'h')
                        {
                            ++castling_available;

                            FEN = FEN + 'K';
                        }
                        //if its the left rook
                        if (whitePieces[i].currentSquare.ReturnSquare()[0] == 'a')
                        {
                            ++castling_available;

                            FEN = FEN + 'Q';
                        }
                    }
                }        
            }
        }
        //check for black castling
        foreach (ChessPiece blackPiece in blackPieces)
        {
            if (blackPiece.pieceLetter == 'k')
            {
                //if king has moved
                if (!blackPiece.firstMove)
                {
                    //remember it has moved
                    black_king_first_move = false;
                    //break from the loop because you cant castle if king has moved
                    break;
                }
                for (int i = (blackPieces.Count - 1); i >= 0; --i)
                {
                    
                    //find the rook and check if the rook has moved
                    //skip if it has moved
                    if (blackPieces[i].pieceLetter == 'r' && blackPieces[i].firstMove == true)
                    {
                        //if its the right rook
                        if (blackPieces[i].currentSquare.ReturnSquare()[0] == 'h')
                        {
                            ++castling_available;

                            FEN = FEN + 'k';
                        }
                        //if its the left rook
                        if (blackPieces[i].currentSquare.ReturnSquare()[0] == 'a')
                        {
                            ++castling_available;

                            FEN = FEN + 'q';
                        }
                    }
                }
            }
        }

        if(!white_king_first_move && !black_king_first_move)
        {
            FEN = FEN + '-' + ' ';
        }
        else if(castling_available == 0)
        {
            FEN = FEN + '-' + ' ';
        }
        if(castling_available > 0)
        {
            FEN = FEN + ' ';
        }

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
        else
        {
            FEN = FEN + '-' + ' ';
        }

        return FEN;
    }
    //turn counter
    private string fifthFENField()
    {
        string FEN = "";

        FEN = "" + turnCounter;

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
        //for navigating through different FEN sections
        int whitespace_counter = 0;
        //place where the checking begins
        int row = 0;
        int column = 7;
        //needed for third FEN section(castling)

        //needed for fourth FEN section(EnPassant)
        int enPassant_letter_count = 0;
        //loop the whole FEN code
        for (int i = 0; i < FEN.Length; ++i)
        {
            //skip the check if FEN is whitespace
            if (FEN[i] != ' ')
            {
                //check the first part of FEN which is the main position of pieces
                //also place the pieces
                if (whitespace_counter == 0)
                {
                    //if white piece
                    //spawn corresponding piece on corresponding square
                    if (FEN[i] > 'A' && FEN[i] < 'Z')
                    {
                        if (FEN[i] == 'P')
                            SpawnSingleWhitePiece(0, column, row);
                        if (FEN[i] == 'R')
                            SpawnSingleWhitePiece(1, column, row);
                        if (FEN[i] == 'N')
                            SpawnSingleWhitePiece(2, column, row);
                        if (FEN[i] == 'B')
                            SpawnSingleWhitePiece(3, column, row);
                        if (FEN[i] == 'Q')
                            SpawnSingleWhitePiece(4, column, row);
                        if (FEN[i] == 'K')
                            SpawnSingleWhitePiece(5, column, row);

                        //next spot on the board
                        row = row + 1;
                    }
                    //if black piece
                    //spawn corresponding piece on corresponding square
                    if (FEN[i] > 'a' && FEN[i] < 'z')
                    {
                        if (FEN[i] == 'p')
                            SpawnSingleBlackPiece(0, column, row);
                        if (FEN[i] == 'r')
                            SpawnSingleBlackPiece(1, column, row);
                        if (FEN[i] == 'n')
                            SpawnSingleBlackPiece(2, column, row);
                        if (FEN[i] == 'b')
                            SpawnSingleBlackPiece(3, column, row);
                        if (FEN[i] == 'q')
                            SpawnSingleBlackPiece(4, column, row);
                        if (FEN[i] == 'k')
                            SpawnSingleBlackPiece(5, column, row);

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
                //check the second part of FEN which shows who can move this turn
                if (whitespace_counter == 1)
                {
                    whiteTurn = (FEN[i] == 'w') ? true : false;
                }
                //check if there is castling available
                //needs work
                if (whitespace_counter == 2)
                {
                    for (int x = 0; x < whitePieces.Count; ++x)
                    {
                        if (FEN[i] == 'K')
                        {
                            if(whitePieces[x].pieceLetter == 'R' && whitePieces[x].currentSquare.ReturnSquare()[0] == 'h')
                            {
                                whitePieces[x].firstMove = true;
                            }
                        }
                    }
                }
                //check if there is a EnPassant square
                if (whitespace_counter == 3)
                {
                    //means that we can EnPassant
                    if (FEN[i] != '-')
                    {
                        if (enPassant_letter_count == 0)
                        {
                            //set the EnPassant square
                            enPassantSquare = "" + FEN[i] + FEN[i + 1];
                            ++enPassant_letter_count;
                        }
                    }
                }
                //set the turn counter
                if (whitespace_counter == 4)
                {
                    turnCounter = FEN[i];
                }
            }
            //if we see a whitespace, means we are at the end of first section
            //time to move onto the next one
            if(FEN[i] == ' ')
            {
                ++whitespace_counter;
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

            if (pawn_column == 7)
            {
                whitePieces.Remove(piece);
                piece.killYourself();

                SpawnSingleWhitePiece(4, pawn_column, pawn_row);
            }
        }
        if (piece.pieceLetter == 'p')
        {
            int pawn_row = piece.currentSquare.ReturnSquare()[0] - 97;
            int pawn_column = piece.currentSquare.ReturnSquare()[1] - 49;

            if (pawn_column == 0)
            {
                blackPieces.Remove(piece);
                piece.killYourself();

                SpawnSingleBlackPiece(4, pawn_column, pawn_row);
            }
        }
    }
}
