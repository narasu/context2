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
    private int selectedNodeIndex = -1;

    private GUIStyle removeButton;

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
            using (new GUILayout.VerticalScope())
            {
                if (GUILayout.Button("Node " + i, EditorStyles.boldLabel))
                {
                    selectedNodeIndex = i;
                }

                var pn = pathNodesList.GetArrayElementAtIndex(i);

                using (new GUILayout.HorizontalScope())
                {
                    float waitTime = pn.FindPropertyRelative("WaitTime").floatValue;
                    GUILayout.Label("Wait Time: " + waitTime, GUILayout.Width(80));
                    waitTime = GUILayout.HorizontalSlider(waitTime, 0.0f, 10.0f);
                    pn.FindPropertyRelative("WaitTime").floatValue = waitTime;
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space(40.0f, true);
                if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60)))
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
            selectedNodeIndex = pathNodesList.arraySize;
            Vector3 initPos;
            Quaternion initRot;

            if (pathNodesList.arraySize == 0)
            {
                initRot = guard.transform.rotation;
                initPos = initRot * Vector3.forward * 2.0f;
            }
            else
            {
                var pn = pathNodesList.GetArrayElementAtIndex(guard.PathNodes.Count - 1);
                
                Vector3 p = pn.FindPropertyRelative("Position").vector3Value;
                Quaternion r = pn.FindPropertyRelative("Rotation").quaternionValue;
                
                initPos = p + r * (Vector3.forward * 2.0f);
                initRot = r;
            }

            pathNodesList.InsertArrayElementAtIndex(pathNodesList.arraySize);
            SerializedProperty n = pathNodesList.GetArrayElementAtIndex(pathNodesList.arraySize - 1);
            n.FindPropertyRelative("Position").vector3Value = initPos;
            n.FindPropertyRelative("Rotation").quaternionValue = initRot;
        }

        Utility.RefreshSceneView();
        serializedObject.ApplyModifiedProperties();
        
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        if (pathNodesList.arraySize == 0)
        {
            return;
        }
        
        Vector3 guardPos = guard.transform.position;

        
        for(int i=0; i< pathNodesList.arraySize; i++)
        {
            SerializedProperty pathNode = pathNodesList.GetArrayElementAtIndex(i);
            
            // get position and rotation to draw node at
            Vector3 pos = guardPos + pathNode.FindPropertyRelative("Position").vector3Value;
            Quaternion rot = pathNode.FindPropertyRelative("Rotation").quaternionValue;

            // draw node
            Handles.color = selectedNodeIndex == i ? Color.green : Color.blue;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Handles.SphereHandleCap(controlID, pos, rot, .25f, EventType.Repaint);
            
            // if node is selected, draw transform gizmo
            if (selectedNodeIndex == i)
            {
                selectedNode = controlID;
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
                            GUIUtility.hotControl = controlID;
                            selectedNode = controlID;
                            selectedNodeIndex = i;
                            evt.Use();
                        }
                    }
                    break;
                case EventType.ExecuteCommand:
                    if (evt.commandName == "FrameSelected" && selectedNode == controlID)
                    {
                        SceneView.lastActiveSceneView.Frame(new Bounds(pos, Vector3.one * 2.0f), false);
                        evt.Use();
                    }

                    break;
            }
        }
        serializedObject.ApplyModifiedProperties();
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
