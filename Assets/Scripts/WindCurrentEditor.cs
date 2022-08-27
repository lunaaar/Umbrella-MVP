using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(WindCurrent))]
public class WindCurrentEditor : Editor
{
    [SerializeField] private float currentSize  = 5;

    

    //Called on Property Change.
    private void OnValidate()
    {
        
    }

    SerializedObject so;
    //SerializedProperty propWindSpeed;

    private void OnEnable()
    {
        so = serializedObject;
        //propWindSpeed = so.FindProperty("windSpeed");
    }

    public override void OnInspectorGUI()
    {
        WindCurrent windCurrent = target as WindCurrent;

        //windCurrent.transform.localScale.Set(EditorGUILayout.FloatField("Width", windCurrent.transform.localScale.x), windCurrent.transform.localScale.y, windCurrent.transform.localScale.z);

        EditorGUILayout.FloatField(currentSize);

        //windCurrent.transform.localScale = new Vector3(currentSize, windCurrent.transform.localScale.y, windCurrent.transform.localScale.z);

        /**using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            so.Update();
            //EditorGUILayout.PropertyField(propWindSpeed);
            //EditorGUILayout.Slider()

            

            if (so.ApplyModifiedProperties())
            {
                //if something changed

            }

            //windCurrent.transform.localScale += new Vector3(EditorGUILayout.FloatField("Width", currentSize), 0);
            //windCurrent.transform.localScale = new Vector3(windCurrent.transform.localScale.x + EditorGUILayout.FloatField("Width", currentSize), windCurrent.transform.localScale.y, windCurrent.transform.localScale.z);
        }*/
        
        //GUI.
        //GUILayout.
        //EditorGUI.
        //EditorGUILayout.

        base.OnInspectorGUI();


    }
}