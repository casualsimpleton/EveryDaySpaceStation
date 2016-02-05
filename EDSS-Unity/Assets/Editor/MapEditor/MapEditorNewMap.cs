using UnityEngine;
using UnityEditor;

public class MapEditorNewMap : EditorWindow
{
    static void Init()
    {
        MapEditorNewMap window = ScriptableObject.CreateInstance<MapEditorNewMap>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("This is an example of EditorWindow.ShowPopup", EditorStyles.miniTextField);
        GUILayout.Space(70);
        if (GUILayout.Button("Agree!")) this.Close();
    }
}