using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommandPlan : MonoBehaviour
{
    public int commandIndex = 0;
    GameObject _playerGameObject;
    MapManager _mapManager;
    
    #region singleton
    static CommandPlan _instance;
    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public static CommandPlan Get()
    {
        return _instance;
    }
    #endregion

    void Start()
    {
        _mapManager = MapManager.Get();
        _playerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            RunNextCommand();
        }
        
        if (Input.GetKeyDown("r"))
        {
            ResetRun();
        }
        
        if (Input.GetKeyDown("s"))
        {
            MapManager.Get().SaveToFile();
        }
    }

    public void ResetRun()
    {
        StopAllCoroutines();
        _mapManager.Reset();
        _playerGameObject.GetComponent<PlayerController>().Reset();
        
        // Deselect any lit up steps
        EventSystem.current.SetSelectedGameObject(null);
        commandIndex = 0;
    }

    public void RunNextCommand()
    {
        if (commandIndex >= transform.childCount)
        {
            print("End of command plan");
            commandIndex = 0;
            return;
        }
        print($"Running command #{commandIndex}");
        StartCoroutine(WaitAndRunNextCommand());
    }

    IEnumerator WaitAndRunNextCommand()
    {
        Transform child = transform.GetChild(commandIndex);
        child.GetComponent<Button>().Select();
        yield return new WaitForSeconds(0.5f);
        Command command = child.GetComponent<Command>();
        command.Run();

        commandIndex += 1;
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
