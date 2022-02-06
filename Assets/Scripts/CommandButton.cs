using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour
{
    private GameObject commandPlan;
    public Command command;
    public GameObject plannedCommandPrefab;

    private void Start()
    {
        commandPlan = GameObject.FindGameObjectWithTag("CommandPlan");
    }

    public void AddToPlan()
    {
        GameObject go = Instantiate(plannedCommandPrefab, commandPlan.transform);
        go.GetComponent<Command>().method = command.method;
        
        // TODO: Clean up icon sprite assignment
        Image fromImage = transform.GetChild(0).GetComponent<Image>();
        Image toImage = go.transform.GetChild(0).GetComponent<Image>();
        toImage.sprite = fromImage.sprite;
        toImage.color = fromImage.color;
    }
}
