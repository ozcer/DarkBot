using System;
using System.Collections;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public MapEditorTool selectedTool;
    public static int width = 8;
    public static int height = 8;
    public Transform tilesParent;
    Rigidbody _playerRb;
    
    public GameObject spawnPointIndicator;
    public GameObject spawnPointPreview;

    public DrawerTab drawerTab;
    public bool isEditing = false;
    public Color editingBackgroundColor, playingBackgroundColor;
    public GameObject commandUi;
    Camera _camera;

    bool _initialized = false;
    MapManager _mapManager;

    #region singleton

    static MapEditor _instance;

    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public static MapEditor Get()
    {
        return _instance;
    }

    #endregion

    void Start()
    {
        _mapManager = MapManager.Get();
        _camera = Camera.main;
        _playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        InitTiles();

        drawerTab.onOpen.AddListener(StartEditing);
        drawerTab.onClose.AddListener(StopEditing);
    }

    void Update()
    {
        // TODO refactor to toggle instead of constant checks
        bool usingSpawnTool = selectedTool != null && selectedTool.isSpawnPointTool;
        spawnPointPreview.SetActive(usingSpawnTool);
    }

    void InitTiles()
    {
        int x = 0;
        int y = height - 1;
        foreach (Transform tileTransform in tilesParent)
        {
            EditorTile tile = tileTransform.GetComponent<EditorTile>();
            tile.x = x;
            tile.y = y;

            x++;
            if (x >= width)
            {
                y--;
                x = 0;
            }
        }

        _initialized = true;
    }

    public IEnumerator LoadLevelCoroutine(TileType[][] tileData, Vector2Int spawnPoint)
    {
        // Wait for tiles to be initialized
        yield return new WaitUntil(() => _initialized);

        // Set spawn point
        EditorTile spawnTile = CoordToTile(spawnPoint.x, spawnPoint.y);
        spawnTile.SetAsSpawnPoint();

        // Set tiles types
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileType tileType = tileData[x][y];
                EditorTile editorTile = CoordToTile(x, y);
                editorTile.SetTileType(tileType);
            }
        }
    }

    public void SaveLevel()
    {
        _mapManager.SaveToFile();
    }

    static int CoordToIndex(int x, int y)
    {
        int row = height - y - 1;
        return row * width + x;
    }

    public static Vector2Int IndexToCoord(int i)
    {
        int col = i % width;
        int row = (i - col) / width;
        return new Vector2Int(col, row);
    }

    EditorTile CoordToTile(int x, int y)
    {
        int childIndex = CoordToIndex(x, y);
        Transform tileObject = tilesParent.GetChild(childIndex);
        return tileObject.GetComponent<EditorTile>();
    }

    void StartEditing()
    {
        isEditing = true;
        _camera.backgroundColor = editingBackgroundColor;
        commandUi.SetActive(false);
        _playerRb.useGravity = false;
    }

    void StopEditing()
    {
        isEditing = false;
        _camera.backgroundColor = playingBackgroundColor;
        commandUi.SetActive(true);
        _playerRb.useGravity = true;

    }
}