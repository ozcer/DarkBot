using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Vector2Int coord;
    Vector2Int _facing;
    
    // Turn Controls
    Vector3 _startRotation;
    Vector3 _endRotation;
    public float turnDuration = 1f;
    float _elapsedTurnTime;
    
    // Movement Controls
    GameObject _destTileObject;
    Vector3 _startPos;
    public Vector2Int destCoord;
    public float walkTime = 2f;
    float _elapsedWalkTime;
    
    static readonly Vector2Int Nowhere = -Vector2Int.one;
    public static readonly float Epsilon = 0.1f;
    
    public Animator animator;
    public Rigidbody rb;

    MapManager _mapManager;

    public UnityEvent onCommandComplete;
    static readonly int MovingFlag = Animator.StringToHash("Moving");

    void Start()
    {
        _mapManager = MapManager.Get();
        onCommandComplete.AddListener(() => { print("Command Complete"); });
        
        Reset();
    }

    public void Reset()
    {
        destCoord = Nowhere;
        _elapsedWalkTime = 0f;
        _elapsedTurnTime = 0f;
        _facing = Vector2Int.up;
        _startRotation = Vector3.zero;
        _endRotation = Vector3.zero;
        animator.SetBool(MovingFlag, false);

    }

    void FixedUpdate()
    {
        if (destCoord != Nowhere)
        {
            // Move to tile position, ignoring y coordinate
            Vector3 destTilePosition = _destTileObject.transform.position;
            Vector3 destPos = new Vector3(
                destTilePosition.x,
                transform.position.y,
                destTilePosition.z
            );
            Vector3 newPos = Vector3.Lerp(_startPos, destPos, _elapsedWalkTime / walkTime);
            transform.position = newPos;

            _elapsedWalkTime += Time.deltaTime;
            if (_elapsedWalkTime > walkTime)
            {
                destCoord = Nowhere;
                onCommandComplete.Invoke();
            }
        }
        animator.SetBool(MovingFlag, destCoord != Nowhere);

        // TODO: fix condition
        if (_endRotation != _startRotation && _elapsedTurnTime < turnDuration)
        {
            Quaternion startRot = Quaternion.Euler(_startRotation);
            Quaternion endRot = Quaternion.Euler(_endRotation);
            Quaternion newRot = Quaternion.Lerp(startRot, endRot, _elapsedTurnTime / turnDuration);
            transform.rotation = newRot;

            _elapsedTurnTime += Time.deltaTime;
            if (_elapsedTurnTime > turnDuration)
            {
                destCoord = Nowhere;
                onCommandComplete.Invoke();
            }
        }
    }

    public void GoForward()
    {
        Vector2Int destination = coord + _facing;
        if (_mapManager.IsTileWalkable(destination))
        {
            SetDestination(destination);
        }
        else
        {
            print($"{destination} is not reachable");
        }
    }

    public void TurnLeft()
    {
        _startRotation = transform.eulerAngles;
        float y = Mathf.Round(_startRotation.y / 90f) * 90f;
        Vector3 newAngles = new Vector3(
            _startRotation.x,
            y - 90f,
            _startRotation.z);
        _endRotation = newAngles;
        _elapsedTurnTime = 0f;
        _facing = Vector2Int.RoundToInt(Vector2.Perpendicular(_facing));
    }

    public void TurnRight()
    {
        _startRotation = transform.eulerAngles;
        float y = Mathf.Round(_startRotation.y / 90f) * 90f;
        Vector3 newAngles = new Vector3(
            _startRotation.x,
            y + 90f,
            _startRotation.z);
        _endRotation = newAngles;
        _elapsedTurnTime = 0f;

        _facing = -Vector2Int.RoundToInt(Vector2.Perpendicular(_facing));
    }

    void SetDestination(Vector2Int targetCoord)
    {
        _startPos = transform.position;
        coord = targetCoord;
        destCoord = targetCoord;
        _destTileObject = _mapManager.tileObjects[targetCoord.x][targetCoord.y];

        _elapsedWalkTime = 0f;
    }
}