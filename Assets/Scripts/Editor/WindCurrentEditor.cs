using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(WindCurrent))]
public class WindCurrentEditor : Editor
{
    //Called on Property Change.
    private void OnValidate()
    {
        
    }

    //SerializedObject so;
    //SerializedProperty propWindSpeed;

    private void OnEnable()
    {
        //so = serializedObject;
        //propWindSpeed = so.FindProperty("windSpeed");
    }

    public override void OnInspectorGUI()
    {
        WindCurrent windCurrent = target as WindCurrent;

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            windCurrent.transform.localScale = new Vector3(EditorGUILayout.Slider("Current Width", windCurrent.transform.localScale.x, 1, 15), windCurrent.transform.localScale.y, windCurrent.transform.localScale.z);
            windCurrent.transform.localScale = new Vector3(windCurrent.transform.localScale.x, EditorGUILayout.Slider("Current Height", windCurrent.transform.localScale.y, 1, 15), windCurrent.transform.localScale.z);
            
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Set Current Up"))
                {
                    windCurrent.GetComponentInChildren<AreaEffector2D>().forceAngle = 90;
                }
                if (GUILayout.Button("Set Current Down"))
                {
                    windCurrent.GetComponentInChildren<AreaEffector2D>().forceAngle = 270;
                }
            } 
        }


        //GUI.
        //GUILayout.
        //EditorGUI.
        //EditorGUILayout.



        //base.OnInspectorGUI();


    }
}