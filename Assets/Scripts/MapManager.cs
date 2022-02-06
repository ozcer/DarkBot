using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
using UnityEngine.WSA;
using Application = UnityEngine.Application;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    [Header("In-Game References")] 
    GameObject _playerGameObject;
    public GameObject squarePrefab;
    public Transform tileObjectsParent;
    public GameObject[][] tileObjects;
    
    // Data Model
    TileType[][] _tileData;
    int Width => _tileData.Length;
    int Height => Width > 1 ? _tileData[0].Length : 0;
    public Vector2Int spawnPoint = new Vector2Int(0, 0);
    
    class SerializedMapData
    {
        public Vector2Int spawnPoint;
        public string tileData;
    }

    class MapData
    {
        public Vector2Int spawnPoint;
        public TileType[][] tileData;
    }

    public TileType voidType;
    public List<TileType> tileTypes;
    Dictionary<string, TileType> asciiToTypes = new Dictionary<string, TileType>();

    MapEditor _mapEditor;

    #region singleton
    static MapManager _instance;
    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public static MapManager Get()
    {
        return _instance;
    }
    #endregion
   
    void Start()
    {
        _mapEditor = MapEditor.Get();
        
        RegisterTileTypes();
        ReloadLevel();
    }

    void RegisterTileTypes()
    {
        foreach (TileType tileType in tileTypes)
        {
            asciiToTypes[tileType.serialized] = tileType;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("x"))
        {
            ClearLevel();
            MapData mapData = GenerateRandomMap();
            _tileData = mapData.tileData;
            spawnPoint = mapData.spawnPoint;
            
            SpawnTiles(_tileData);
            SpawnPlayer();
            SaveToFile();
            StartCoroutine(_mapEditor.LoadLevelCoroutine(_tileData, spawnPoint));

        }
    }

    public bool IsTileWalkable(Vector2Int coord)
    {
        // Coord within edges of room
        bool withinBounds =  coord.x >= 0 && 
                             coord.x < MapEditor.width && 
                             coord.y >= 0 && 
                             coord.y < MapEditor.height;
        if (!withinBounds) return false;
        
        // Check tile is walkable
        TileType tileType = _tileData[coord.x][coord.y];
        return tileType != null && tileType.isWalkable;
    }

    public void ReloadLevel()
    {
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        ClearLevel();
        LoadFromFile();
        SpawnTiles(_tileData);
        SpawnPlayer();
        SaveToFile();
        StartCoroutine(_mapEditor.LoadLevelCoroutine(_tileData, spawnPoint));
    }

    public void Reset()
    {
        SpawnPlayer();
    }

    MapData GenerateRandomMap()
    {
        TileType[][] tileData = EmptyTileData();
        Vector2Int spawnLocation = Vector2Int.zero;
        
        // Generate tiles
        for (var x = 0; x < MapEditor.width; x++)
        {
            for (var y = 0; y < MapEditor.height; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                TileType randomType = tileTypes.GetRandom();
                tileData[x][y] = randomType;
                if (randomType.isWalkable && !IsTileWalkable(spawnLocation))
                {
                    spawnLocation = coord;
                }
            }
        }
        return new MapData
        {
            spawnPoint = spawnLocation,
            tileData = tileData
        };
    }

    void ClearLevel()
    {
        // Clear tile GameObjects and data model
        foreach (Transform square in tileObjectsParent)
        {
            Destroy(square.gameObject);
        }
        tileObjects = EmptyTileObjects();
        _tileData = EmptyTileData();
    }

    static GameObject[][] EmptyTileObjects()
    {
        GameObject[][] newObjects = new GameObject[MapEditor.width][];
        for (int i = 0; i < MapEditor.width; i++)
        {
            newObjects[i] = new GameObject[MapEditor.height];
        }

        return newObjects;
    }

    static TileType[][] EmptyTileData()
    {
        TileType[][] newData = new TileType[MapEditor.width][];
        for (int i = 0; i < MapEditor.width; i++)
        {
            newData[i] = new TileType[MapEditor.height];
        }

        return newData;
    }

    void SpawnTiles(TileType[][] tileData)
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                SpawnTile(tileData[x][y], x, y);
            }
        }
    }

    public void SetTile(TileType tileType, int x, int y)
    {
        RemoveTile(x, y);
        _tileData[x][y] = tileType;
        SpawnTile(tileType, x, y);
    }

    void SpawnTile(TileType tileType, int x, int y)
    {
        if (tileType == null || tileType.isVoid) return;
        
        // Get bounds
        Renderer squareRenderer = squarePrefab.GetComponent<Renderer>();
        Vector3 tileDimensions = squareRenderer.bounds.size;
        
        // Spawn and add to object references
        Vector3 pos = new Vector3(
            x * tileDimensions.x,
            0,
            y * tileDimensions.z);
        GameObject go = Instantiate(tileType.prefab, 
            pos, 
            Quaternion.identity,
            tileObjectsParent);

        tileObjects[x][y] = go;
    }
    
    void RemoveTile(int x, int y)
    {
        // Destroy and remove from references
        GameObject go = tileObjects[x][y];
        if (go != null) Destroy(go);

        tileObjects[x][y] = null;
        _tileData[x][y] = null;
    }

    void SpawnPlayer()
    {
        SpawnPlayer(spawnPoint);
    }

    public void SpawnPlayer(Vector2Int coord)
    {
        // TODO clean up magic numbers
        float spawnY = 0.5f;
        Vector3 pos = new Vector3(coord.x,
            spawnY,
            coord.y);
        Transform playerTransform = _playerGameObject.transform;
        playerTransform.position = pos;
        playerTransform.rotation = Quaternion.identity;
        
        _playerGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        PlayerController controller = _playerGameObject.GetComponent<PlayerController>();
        controller.coord = spawnPoint;
        controller.Reset();
    }

    public void SaveToFile()
    {
        string serialized = SerializeToString();
        string path = Path.Combine(Application.dataPath, "map.json");
        File.WriteAllText(
            path,
            serialized);
        
        print($"Saved to {path}");
    }

    public void LoadFromFile()
    {
        string path = Path.Combine(Application.dataPath, "map.json");
        string rawData = File.ReadAllText(path);
        SerializedMapData mapData = JsonUtility.FromJson<SerializedMapData>(rawData);
        spawnPoint = mapData.spawnPoint;
        _tileData = DeserializeMap(mapData.tileData);
    }

    string SerializeToString()
    {
        SerializedMapData mapData = new SerializedMapData
        {
            spawnPoint = this.spawnPoint,
            tileData = SerializeMap()
        };
        string result = JsonUtility.ToJson(mapData);
       

        return result;
    }

    string SerializeMap()
    {
        StringBuilder sb = new StringBuilder();
        foreach (TileType[] col in _tileData)
        {
            foreach (TileType tileType in col)
            {
                // print(tileType);
                string encoded = tileType == null ? voidType.serialized : tileType.serialized;
                // print(encoded);
                sb.Append(encoded);
            }
        
            sb.Append("\n");
        }

        return sb.ToString();
    }

    TileType[][] DeserializeMap(string serialized)
    {
        string[] lineData = serialized.Split('\n');

        TileType[][] result = EmptyTileData();
        for (int x = 0; x < MapEditor.width; x++)
        {
            string line = lineData[x];
            for (int y = 0; y < MapEditor.height; y++)
            {
                bool withinBounds =
                    x.IsWithin(0, MapEditor.width - 1) &&
                    y.IsWithin(0, MapEditor.height - 1);
                TileType tileType;
                if (withinBounds)
                {
                    string code = line[y].ToString();
                    tileType = asciiToTypes[code];
                }
                else
                {
                    tileType = voidType;
                }

                result[x][y] = tileType;
            }
        }

        return result;
    }
}
