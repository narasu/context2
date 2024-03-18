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
    private int selectedNode;
    private int newNodeIndex = -1;
    
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
            newNodeIndex = pathNodesList.arraySize;
            Vector3 initPos;
            Quaternion initRot;

            if (guard.PathNodes.Count == 0)
            {
                initRot = guard.transform.rotation;
                initPos = initRot * Vector3.forward * 2.0f;
            }
            else
            {
                var pn = pathNodesList.GetArrayElementAtIndex(guard.PathNodes.Count - 1);
                
                Vector3 p = pn.FindPropertyRelative("Position").vector3Value;
                Quaternion r = pn.FindPropertyRelative("Rotation").quaternionValue;
                
                initPos = p + r * Vector3.forward;
                initRot = r;
                
            }

            pathNodesList.InsertArrayElementAtIndex(pathNodesList.arraySize);
            SerializedProperty n = pathNodesList.GetArrayElementAtIndex(pathNodesList.arraySize - 1);
            n.FindPropertyRelative("Position").vector3Value = initPos;
            n.FindPropertyRelative("Rotation").quaternionValue = initRot;
            newNodeIndex = -1;
        }

        serializedObject.ApplyModifiedProperties();
        Utility.RefreshSceneView();
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        if (pathNodesList.arraySize == 0)
        {
            return;
        }
        
        Vector3 guardPos = guard.transform.position;

        if (newNodeIndex >= 0)
        {
            
        }
        
        for(int i=0; i< pathNodesList.arraySize; i++)
        {
            SerializedProperty pathNode = pathNodesList.GetArrayElementAtIndex(i);
            
            // get position and rotation to draw node at
            Vector3 pos = guardPos + pathNode.FindPropertyRelative("Position").vector3Value;
            Quaternion rot = pathNode.FindPropertyRelative("Rotation").quaternionValue;

            // draw node
            Handles.color = Color.blue;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Handles.SphereHandleCap(controlID, pos, rot, .25f, EventType.Repaint);
            
            // if node is selected, draw transform gizmo
            if (selectedNode == controlID)
            {
                EnableTransformHandles(pathNode, pos, rot);
            }
            // otherwise, just draw a line indicating its forward direction
            else
            {
                Handles.DrawAAPolyLine(4.0f, pos + Vector3.up * 0.1f, pos + rot * Vector3.forward * 0.5f + Vector3.up * 0.1f);
            }

            var evt = Event.current;
            switch (evt.GetTypeForControl(controlID))
            {
                case EventType.Layout:
                    if (selectedNode != controlID)
                    {
                        float distanceToHandle = HandleUtility.DistanceToCircle(pos, .25f);
                        HandleUtility.AddControl(controlID, distanceToHandle);
                    }
                    break;
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == controlID && evt.button == 0)
                    {
                        if (selectedNode != controlID)
                        {
                            Debug.Log("Gizmo selected!");
                            GUIUtility.hotControl = controlID;
                            selectedNode = controlID;
                            evt.Use();
                        }
                    }
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawNode(SerializedProperty _pathNode)
    {
        Vector3 guardPos = guard.transform.position;
        // get position and rotation to draw node at
        Vector3 pos = guardPos + _pathNode.FindPropertyRelative("Position").vector3Value;
        Quaternion rot = _pathNode.FindPropertyRelative("Rotation").quaternionValue;

        // draw node
        Handles.color = Color.blue;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Handles.SphereHandleCap(controlID, pos, rot, .25f, EventType.Repaint);
    }

    private void EnableTransformHandles(SerializedProperty _pathNode, Vector3 _pos, Quaternion _rot)
    {
        Vector3 guardPos = guard.transform.position;
        
        _pos = Handles.PositionHandle(_pos, _rot);
        _pathNode.FindPropertyRelative("Position").vector3Value = _pos - guardPos;
                
        _rot = Handles.RotationHandle(_rot, _pos);
        _pathNode.FindPropertyRelative("Rotation").quaternionValue = _rot;
    }
}
