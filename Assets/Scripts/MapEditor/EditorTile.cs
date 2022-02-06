using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EditorTile :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler
{
    public TileType tileType = null;
    public TileType defaultTileType;
    public int x, y;
    Image _image;
    MapEditor _mapEditor;
    MapManager _mapManager;
    
    void Start()
    {
        _image = GetComponent<Image>();
        _mapEditor = MapEditor.Get();
        _mapManager = MapManager.Get();
        _image.color = defaultTileType.editorColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Preview tile or spawn point on hover
        MapEditorTool selectedTool = _mapEditor.selectedTool;
        if (selectedTool == null) return;
        
        // Move spawn point preview
        if (selectedTool.isSpawnPointTool)
        {
            _mapEditor.spawnPointPreview.SetActive(true);
            _mapEditor.spawnPointPreview.transform.position = transform.position;
        }
        else
        {
            _image.color = selectedTool.tileType.editorColor;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Disable spawn point preview on hover exit
        if (_mapEditor.selectedTool != null && 
            _mapEditor.selectedTool.isSpawnPointTool)
        {
            // _mapEditor.spawnPointPreview.SetActive(false);
        }
        
        // Revert back from editor tool preview to original view
        if (tileType == null)
        {
            _image.color = defaultTileType.editorColor;
        }
        else
        {
            _image.color = tileType.editorColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // TODO clean up logic flow
        var buttonClicked = eventData.button;
        bool settingSpawnPoint =
            buttonClicked == PointerEventData.InputButton.Left &&
            _mapEditor.selectedTool.isSpawnPointTool;
        print(buttonClicked);
        if (settingSpawnPoint)
        {
            SetAsSpawnPoint();
            return;
        }

        // Right click to clear
        if (buttonClicked == PointerEventData.InputButton.Right)
        {
            ClearTileType();
        }
        // Left click to set
        else if (buttonClicked == PointerEventData.InputButton.Left)
        {
            MapEditorTool selectedTool = _mapEditor.selectedTool;
            if (selectedTool == null) return;

            SetTileType(selectedTool.tileType);
        }
    }

    public void SetAsSpawnPoint()
    {
        _mapEditor.spawnPointIndicator.transform.position = transform.position;

        Vector2Int newSpawn = new Vector2Int(x, y);
        _mapManager.spawnPoint = newSpawn;
        _mapManager.SpawnPlayer(newSpawn);
    }

    public void SetTileType(TileType newTileType)
    {
        TileType value = newTileType == null ? defaultTileType : newTileType;
        
        // Set in editor then set in-game
        tileType = value;
        _image.color = tileType.editorColor;
        
        _mapManager.SetTile(value, x, y);
    }

    void ClearTileType()
    {
        // Remove in editor then remove in-game
        tileType = defaultTileType;
        _image.color = defaultTileType.editorColor;
        
        _mapManager.SetTile(tileType, x, y);
    }
}