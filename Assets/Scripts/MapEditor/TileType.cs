using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "TileType", order = 1)]
public class TileType : ScriptableObject
{
    public string tileName;
    public Color editorColor;
    public string serialized = "x";
    public bool isWalkable = true;
    public GameObject prefab;
    public bool isVoid = false;

    public static TileType Void;
}