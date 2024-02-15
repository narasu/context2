using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonInteractHandler : MonoBehaviour
{
    public LayerMask IgnoreLayers;
    private Camera cam;
    private IClickable currentTarget;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Ray r = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
        Debug.DrawRay(r.origin, r.direction, Color.white);
        if (Physics.Raycast(r, out RaycastHit hit, 8.0f, ~IgnoreLayers))
        {
            var t = hit.transform.GetComponent<IClickable>();

            if (currentTarget != t)
            {
                if (t != null)
                {
                    t.OnHoverEnter();
                }
                else
                {
                    currentTarget?.OnHoverExit();
                }

                currentTarget = t;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                currentTarget?.OnClick();
            }
        }
        else
        {
            currentTarget?.OnHoverExit();
            currentTarget = null;
        }
    }
}
