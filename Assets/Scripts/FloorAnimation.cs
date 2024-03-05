using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAnimation : MonoBehaviour
{
    public float scrollX = 0.5f;
    public float scrollY = 0.5f;
    void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;
        gameObject.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
