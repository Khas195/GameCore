using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MoveMadeEvent : UnityEvent<PlayerControl.Player, Square>
{

}
[ExecuteInEditMode]
public class PlayerControl : MonoBehaviour
{

    public enum Player
    {
        Cross,
        Circle
    }
    [SerializeField]
    LayerMask boardMask;
    [SerializeField]
    Board board;
    public MoveMadeEvent moveMade;
    Player curPlayer;
    int turn = 0;
    // Use this for initialization
    void Start()
    {
        curPlayer = Player.Circle;
        if (moveMade == null)
        {
            moveMade = new MoveMadeEvent();
        }
    }

    public int GetCurrentTurn()
    {
        return turn;
    }

    public Player GetCurrentPlayer()
    {
        return curPlayer;
    }

    public void OnRestart(){
        curPlayer = Player.Circle;
        turn = 0;
        this.enabled = true;

    }
    public void OnGameOver(){
        this.OnRestart();
        this.enabled= false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, boardMask))
            {
                var square = hit.collider.gameObject.GetComponent<Square>();
                moveMade.Invoke(curPlayer, square);
                Debug.Log(square.GetCoordinateInGrid());
            }
        }
    }
    public void OnMoveValidation(bool isValid)
    {
        if (isValid)
        {
            if (curPlayer == Player.Cross)
            {
                curPlayer = Player.Circle;
            }
            else
            {
                curPlayer = Player.Cross;
            }
            turn++;
        }
    }
    public void OnPlayerWin(PlayerControl.Player player)
    {
        if (player == Player.Cross)
        {
            Debug.Log("Cross Win");
        }
        else
        {
            Debug.Log("Circle Win");
        }
    }
}
