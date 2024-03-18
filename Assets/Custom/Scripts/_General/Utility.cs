using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class Utility
{
    
    
#if UNITY_EDITOR
    public static void RefreshSceneView()
    {
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
    }
#endif
}
