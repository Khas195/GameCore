using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public enum State
    {
        Cross,
        Circle,
        Empty
    }
    [SerializeField]
    GameObject Cross;
    [SerializeField]
    GameObject Circle;
    [SerializeField]
    GameObject tile;
    Vector2 position;
    private State curState = State.Empty;

    // Use this for initialization
    void Start()
    {
        UpdateVisual();
    }

    public void SetCoordinateInGrid(int x, int y)
    {
        position.Set(x, y);
    }
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SetState(State newState)
    {
        curState = newState;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        switch (curState)
        {
            case State.Empty:
                Circle.SetActive(false);
                Cross.SetActive(false);
                break;
            case State.Cross:
                Circle.SetActive(false);
                Cross.SetActive(true);
                break;
            case State.Circle:
                Circle.SetActive(true);
                Cross.SetActive(false);
                break;
            default:
                break;
        }
    }

    public int GetConsecutives(State state)
    {

        return 0;
    }
    public State GetState()
    {
        return curState;
    }
    public Vector2 GetCoordinateInGrid()
    {
		return position;
    }
}
