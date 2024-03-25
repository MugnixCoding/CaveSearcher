using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonManager), true)]
public class DungeonManagerEditor : Editor
{
    DungeonManager manager;

    private void Awake()
    {
        manager = (DungeonManager)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Dungeon"))
        {
            manager.GenerateDungeon();
        }
        if (GUILayout.Button("Clear Dungeon"))
        {
            manager.ClearDungeon();
        }
    }
}
