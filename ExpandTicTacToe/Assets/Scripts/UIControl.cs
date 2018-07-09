using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{

    [SerializeField]
    Text turn;
    [SerializeField]
    Text curPlayer;
	[SerializeField]
	Text winningText;
    [SerializeField]
    PlayerControl pc;
    // Use this for initialization
    void Start()
    {
		winningText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void OnRestart() {
        winningText.gameObject.SetActive(false);
    }
	public void OnValidMove(bool validation)
    {
        if (!validation) return;
        UpdateUI();
    }

    public void UpdateUI()
    {
        turn.text = "Turn : " + pc.GetCurrentTurn();
        var player = pc.GetCurrentPlayer();
        if (player == PlayerControl.Player.Cross)
        {
            curPlayer.text = "Player: Cross";
        }
        else
        {
            curPlayer.text = "Player: Circle";
        }
    }

    public void OnPlayerWin(PlayerControl.Player player)
    {
		winningText.gameObject.SetActive(true);
        if (player == PlayerControl.Player.Cross)
        {
            winningText.text = "Player: Cross";
			winningText.color = Color.red;
        }
        else
        {
            winningText.text = "Player: Circle";
			winningText.color = Color.blue;
        }
    }
}
