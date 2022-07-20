using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(CriarPredio))]
public class BuildingCreator : Editor
{

    public Object terrio;
    public Object andar;
    public Object cobertura;

    public float alturaAndar = 4f;

    public int andarQuantMax = 10;
    public int andarQuantMin = 3;
    

    public override void OnInspectorGUI()
    {
        CriarPredio myTarget = (CriarPredio)target;

        //ground floor
        EditorGUILayout.LabelField("ground floor");
        terrio = EditorGUILayout.ObjectField(terrio, typeof(GameObject), true);
        EditorGUILayout.LabelField("floor");
        andar = EditorGUILayout.ObjectField(andar, typeof(GameObject), true);
        EditorGUILayout.LabelField("roof");
        cobertura = EditorGUILayout.ObjectField(cobertura, typeof(GameObject), true);

        myTarget.terrio = (GameObject)this.terrio;
        myTarget.andar = (GameObject)this.andar;
        myTarget.cobertura = (GameObject)this.cobertura;

        alturaAndar = EditorGUILayout.FloatField("Floor height", alturaAndar);
        myTarget.alturaAndar = alturaAndar;

        andarQuantMax = EditorGUILayout.IntField("Maximum number of floors", andarQuantMax);
        andarQuantMin = EditorGUILayout.IntField("Minimum number of floors", andarQuantMin);
        myTarget.andarQuantMax = andarQuantMax;
        myTarget.andarQuantMin = andarQuantMin;

        if (GUILayout.Button("Create"))
        {
            myTarget.criarPredio();
        }

        //myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        //EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
    }

}
#endif