using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorToolButton : MonoBehaviour
{
   public Button button;
   public MapEditorTool tool;
   public MapEditor mapEditor;
   public bool isSpawnPointTool = false;
   void Start()
   {
      mapEditor = MapEditor.Get();
   }

   public void Select()
   {
      mapEditor.selectedTool = tool;
   }
}
