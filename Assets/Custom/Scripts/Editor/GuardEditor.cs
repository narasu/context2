using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Guard))]
public class GuardEditor : Editor
{
    
    private Guard guard;
    private SerializedProperty pathNodesList;
    private int selectedNodeIndex;
    
    private Dictionary<SerializedProperty, int> pathControls = new();

    private void OnEnable()
    {
        guard = (Guard)target;
        pathNodesList = serializedObject.FindProperty("PathNodes");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        for(int i=0; i< pathNodesList.arraySize; i++)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Node " + i);
                
                if (GUILayout.Button("Remove"))
                {
                    pathNodesList.DeleteArrayElementAtIndex(i);
                    Utility.RefreshSceneView();
                    break;
                }
            }
        }

        string buttonString = "Add Node";
        if (pathNodesList.arraySize == 0)
        {
            buttonString = "Create Path";
        }
        
        if (GUILayout.Button(buttonString))
        {
            Vector3 initPos;
            Quaternion initRot;

            if (guard.PathNodes.Count == 0)
            {
                initRot = guard.transform.rotation;
                initPos = initRot * Vector3.forward;
            }
            else
            {
                initPos = guard.PathNodes[^1].Position + guard.PathNodes[^1].Forward;
                initRot = guard.PathNodes[^1].Rotation;
            }
            
            guard.PathNodes.Add(new PathNode(initPos, initRot));
            Utility.RefreshSceneView();
        }

        serializedObject.ApplyModifiedProperties();
        // if (serializedObject.ApplyModifiedProperties())
        // {
        //     if (serializedObject.hasModifiedProperties)
        //     {
        //         // if a new node was added, generate a controlID and add it to the dictionary
        //         if (pathNodesList.arraySize > pathControls.Count)
        //         {
        //             for (int i = 0; i < pathNodesList.arraySize; i++)
        //             {
        //                 pathControls.TryAdd(pathNodesList.GetArrayElementAtIndex(i), GUIUtility.GetControlID(FocusType.Passive));
        //             }
        //         }
        //         
        //         // if a node was removed, remove it from the dictionary
        //         else if (pathNodesList.arraySize < pathControls.Count)
        //         {
        //             for (int i = 0; i < pathNodesList.arraySize; i++)
        //             {
        //                 SerializedProperty pathNode = pathNodesList.GetArrayElementAtIndex(i);
        //                 int controlID = pathControls.FirstOrDefault(_x => _x.Key.Equals(pathNode)).Value;
        //                 if (controlID == 0)
        //                 {
        //                     pathControls.Remove(pathNode);
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        if (guard.PathNodes.Count == 0)
        {
            return;
        }

        Vector3 guardPos = guard.transform.position;
        
        for(int i=0; i< pathNodesList.arraySize; i++)
        {
            SerializedProperty pathNode = pathNodesList.GetArrayElementAtIndex(i);
            
            
            Vector3 pos = guardPos + pathNode.FindPropertyRelative("Position").vector3Value;
            Quaternion rot = pathNode.FindPropertyRelative("Rotation").quaternionValue;

            
            Handles.color = Color.blue;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Handles.SphereHandleCap(controlID, pos, rot, .5f, EventType.Repaint);
            if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown && HandleUtility.nearestControl == controlID)
            {
                Debug.Log("Gizmo selected!");
                // pos = Handles.PositionHandle(pos, rot);
                // pathNode.FindPropertyRelative("Position").vector3Value = pos - guardPos;
                //
                // rot = Handles.RotationHandle(rot, pos);
                // pathNode.FindPropertyRelative("Rotation").quaternionValue = rot;
                Event.current.Use();
            }
            if (Event.current.GetTypeForControl(controlID) == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlID);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
