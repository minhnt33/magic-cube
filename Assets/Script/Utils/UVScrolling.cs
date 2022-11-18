using UnityEngine;
using System.Collections;

public class UVScrolling : MonoBehaviour
{
    public float scrollSpeed_X = 0f;
    public float scrollSpeed_Y = 0f;

    private Material _sharedMat;

    void OnEnable()
    {
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
    }

    void OnDisable()
    {
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        Renderer renderer = GetComponent<Renderer>();
        _sharedMat = renderer.sharedMaterial;
    }

    void LateUpdate()
    {
        float offsetX = Time.time * scrollSpeed_X;
        float offsetY = Time.time * scrollSpeed_Y;
        Vector2 offsetVec = _sharedMat.mainTextureOffset;
        offsetVec.x = offsetX;
        offsetVec.y = offsetY;

        _sharedMat.mainTextureOffset = offsetVec;
    }
}