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

            windCurrent.gameObject.GetComponentInChildren<AreaEffector2D>().forceMagnitude = EditorGUILayout.Slider("Magnitude", windCurrent.gameObject.GetComponentInChildren<AreaEffector2D>().forceMagnitude, 0, 30);

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Set Current Up"))
                {
                    windCurrent.GetComponentInChildren<AreaEffector2D>().forceAngle = 90;
                    //windCurrent.GetComponentInChildren<SpriteRenderer>().flipY = false;
                }
                if (GUILayout.Button("Set Current Down"))
                {
                    windCurrent.GetComponentInChildren<AreaEffector2D>().forceAngle = 270;
                    //windCurrent.GetComponentInChildren<SpriteRenderer>().flipY = true;
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