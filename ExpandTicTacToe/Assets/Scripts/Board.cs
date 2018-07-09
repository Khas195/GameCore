using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public class PlayerWinEvent : UnityEvent<PlayerControl.Player> { }
[Serializable]
public class ValidMoveEvent : UnityEvent<bool> { }
[Serializable]
public class BoardExpandEvent : UnityEvent<int> { }
public class Board : MonoBehaviour
{

    [SerializeField]
    int expandAmount;
    [SerializeField]
    int size;
    [SerializeField]
    int maxSize;
    [SerializeField]
    float squareSize;
    [SerializeField]
    int winCon = 3;
    [SerializeField]
    int maxWinCon = 5;
    [SerializeField]
    int winConIncreaseAmount = 2;
    [SerializeField]
    GameObject squarePrefab;
    public PlayerWinEvent playerWinEvent;
    public ValidMoveEvent validMove;
    public UnityEvent onBoardFull;
    public BoardExpandEvent onBoardExpand;
    Vector3 boardSize;

    Vector3 boardStartPos;
    Square[,] grid;
    List<Square> squareList = new List<Square>();

    // Use this for initialization
    void Start()
    {
        if (playerWinEvent == null)
        {
            playerWinEvent = new PlayerWinEvent();
        }
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        boardSize = new Vector3(1, 0, 1) * (squareSize * maxSize);

        grid = new Square[maxSize, maxSize];
        var offset = (boardSize / 2 - new Vector3(squareSize, 0, squareSize) / 2);
        offset.z *= -1;
        boardStartPos = transform.position - offset;

        for (int x = 0; x < maxSize; x++)
        {
            for (int y = 0; y < maxSize; y++)
            {
                var pos = boardStartPos + new Vector3(x * squareSize, 0, -y * squareSize);
                var newSquare = Instantiate(squarePrefab, pos, Quaternion.identity);
                newSquare.transform.parent = transform;
                var sqaure = newSquare.GetComponent<Square>();
                sqaure.SetCoordinateInGrid(x, y);
                grid[x, y] = sqaure;
                squareList.Add(sqaure);
                if (IsWithinRange(x, y))
                {
                    if (!sqaure.gameObject.activeInHierarchy)
                    {
                        sqaure.gameObject.SetActive(true);
                    }
                }
                else
                {
                    sqaure.gameObject.SetActive(false);
                }
            }
        }
    }

    private bool IsWithinRange(int x, int y)
    {
        int halfSize = Mathf.FloorToInt(size / 2);
        int middlePoint = Mathf.FloorToInt(maxSize / 2);
        int lowerBound = middlePoint - halfSize;
        int upperBound = middlePoint + halfSize;
        return x >= lowerBound && x <= upperBound && y >= lowerBound && y <= upperBound;
    }

    public void OnRestart() {
        size = 3;
        winCon = 3;
        TurnOnTileWithinRange();
        int halfSize = Mathf.FloorToInt(size / 2);
        int middlePoint = Mathf.FloorToInt(maxSize / 2);
        int lowerBound = middlePoint - halfSize;
        int upperBound = middlePoint + halfSize;
        for (int x = lowerBound; x <= upperBound; x++)
        {
           for (int y = lowerBound; y <= upperBound; y++)
           {
              grid[x,y].SetState(Square.State.Empty);
           } 
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < maxSize; x++)
        {
            for (int y = 0; y < maxSize; y++)
            {
                var pos = boardStartPos + new Vector3(x * squareSize, 0, -y * squareSize);
                Gizmos.DrawWireCube(pos, new Vector3(squareSize, 0, squareSize));
            }
        }
    }

    public bool IsWinningMove(Square square)

    {
        return CanRowWin(square) || CanColumnWin(square) || CanDiagonalsWin(square);
    }

    private bool CanDiagonalsWin(Square square)
    {
        int numCon = 1;
        var dir = new Vector2(1, 1);
        numCon += CountInDirection(dir, square);
        numCon += CountInDirection(dir * -1, square);
        if (numCon < winCon)
        {
            numCon = 1;
            dir.Set(1, -1);
            numCon += CountInDirection(dir, square);
            numCon += CountInDirection(dir * -1, square);
            return numCon >= winCon;
        }
        else
        {
            return true;
        }

    }
    public void OnMoveMade(PlayerControl.Player player, Square square)
    {
        if (square.GetState() != Square.State.Empty)
        {
            Debug.Log("Invalid Move " + player);
            validMove.Invoke(false);
            return;
        }
        if (player == PlayerControl.Player.Cross)
        {
            square.SetState(Square.State.Cross);
        }
        else
        {
            square.SetState(Square.State.Circle);
        }

        if (IsWinningMove(square))
        {
            playerWinEvent.Invoke(player);
            return;
        }
        validMove.Invoke(true);
        if (IsBoardFull())
        {
            onBoardFull.Invoke();
            ExpandBoard();
            Debug.Log("Board is Full, Expand board by " + expandAmount);
        }
    }

    private bool IsBoardFull()
    {
        int halfSize = Mathf.FloorToInt(size / 2);
        int middlePoint = Mathf.FloorToInt(maxSize / 2);
        int lowerBound = middlePoint - halfSize;
        int upperBound = middlePoint + halfSize;
        for (int x = lowerBound; x <= upperBound; x++)
        {
            for (int y = lowerBound; y <= upperBound; y++)
            {
                if (grid[x, y].GetState() == Square.State.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool CanColumnWin(Square square)
    {
        int numCon = 1;
        var dir = new Vector2(0, 1);
        numCon += CountInDirection(dir, square);
        numCon += CountInDirection(dir * -1, square);
        return numCon >= winCon;
    }

    private bool CanRowWin(Square square)
    {
        int numCon = 1;
        var dir = new Vector2(1, 0);
        numCon += CountInDirection(dir, square);
        numCon += CountInDirection(dir * -1, square);
        return numCon >= winCon;
    }
    int CountInDirection(Vector2 direction, Square square)
    {
        int numCon = 0;
        var position = square.GetCoordinateInGrid() + direction;
        while (IsWithinRange((int)position.x, (int)position.y))
        {
            var toCheck = grid[(int)position.x, (int)position.y];
            if (toCheck.GetState() == square.GetState())
            {
                numCon++;
            }
            else
            {
                return numCon;
            }
            position += direction;
        }
        return numCon;
    }


    void OnDestroy()
    {
        for (int i = 0; i < squareList.Count; i++)
        {
            var temp = squareList[i];
            squareList.RemoveAt(i);
            Destroy(temp);

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExpandBoard();
        }
    }

    private void ExpandBoard()
    {
        var sizeAfterExpand = size + expandAmount;
        size = sizeAfterExpand >= maxSize ? maxSize : sizeAfterExpand;
        TurnOnTileWithinRange();
        var winConAfter = winCon + winConIncreaseAmount;
        winCon = winConAfter >= maxWinCon ? maxWinCon : winConAfter;
        onBoardExpand.Invoke(size);
    }

    private void TurnOnTileWithinRange()
    {
        for (int x = 0; x < maxSize; x++)
        {
            for (int y = 0; y < maxSize; y++)
            {
                var square = grid[x, y];
                if (IsWithinRange(x, y))
                {
                    if (!square.gameObject.activeInHierarchy)
                    {
                        square.SetState(Square.State.Empty);
                        square.gameObject.SetActive(true);
                    }
                }
                else
                {
                    square.gameObject.SetActive(false);
                    square.SetState(Square.State.Empty);
                }
            }
        }
    }
}
