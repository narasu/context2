using System;
using UnityEngine;

public class ThrowHandler : MonoBehaviour
{
    
    public BombData BombDataAsset;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bomb = Instantiate(BombDataAsset.Prefab, transform.position, Quaternion.identity);
            Bomb b = bomb.GetComponent<Bomb>();
            b.Initialize(BombDataAsset);
            b.Throw(transform.forward);
        }
    }
}
