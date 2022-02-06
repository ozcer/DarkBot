using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorTool", menuName = "MapEditorTool", order = 1)]
public class MapEditorTool : ScriptableObject
{
    public TileType tileType;
    public bool isSpawnPointTool = false;
}
