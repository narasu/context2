using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Written by Sebastian Lague for his Field of View visualisation series
/// https://www.youtube.com/watch?v=rQG9aUWarwE
/// 
/// This code was published under the MIT License
/// https://github.com/SebLague/Field-of-View/
/// </summary>

[CustomEditor (typeof (ViewCone))]
public class ViewConeEditor : Editor {

    void OnSceneGUI() {
        ViewCone cone = (ViewCone)target;
        Handles.color = Color.white;
        Handles.DrawWireArc (cone.transform.position, Vector3.up, Vector3.forward, 360, cone.viewRadius);
        Vector3 viewAngleA = cone.DirFromAngle (-cone.viewAngle / 2, false);
        Vector3 viewAngleB = cone.DirFromAngle (cone.viewAngle / 2, false);

        Handles.DrawLine (cone.transform.position, cone.transform.position + viewAngleA * cone.viewRadius);
        Handles.DrawLine (cone.transform.position, cone.transform.position + viewAngleB * cone.viewRadius);

        Handles.color = Color.red;
        // foreach (Transform visibleTarget in fow.visibleTargets) {
        //     Handles.DrawLine (fow.transform.position, visibleTarget.position);
        // }
    }

}
