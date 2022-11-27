using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    
    private bool isInitialized = false;
    private Vector3 lastMousePosition;
    private Vector2 mapSize;
    private int minZoomDistance = 20;
    private int maxZoomDistance = 100;
    private float scrollSensitivity = 5;

    public void Initialize(GameMap map)
    {
        mapSize = map.MapSize;
        mapSize.Scale(new Vector2(GameMap.TileSize, GameMap.TileSize));
        isInitialized = true;
    }

    void Update()
    {
        if(!isInitialized || !isActiveAndEnabled)
        {
            return;
        }

        //  scroll

        if (Input.GetMouseButton(2) && lastMousePosition != null)
        {
            Vector2 delta = new Vector2(lastMousePosition.x - Input.mousePosition.x, lastMousePosition.y - Input.mousePosition.y);
            delta.Scale(new Vector2(0.1f, 0.1f));
            gameObject.transform.position += new Vector3(delta.x, 0, delta.y);
        }

        if (transform.position.x > mapSize.x)
        {
            transform.position = new Vector3(mapSize.x, transform.position.y, transform.position.z);
        }
        if (transform.position.z > mapSize.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, mapSize.y);
        }
        if(transform.position.x < 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
        if (transform.position.z < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        //  zoom

        if (Input.mouseScrollDelta.y != 0)
        {
            transform.position += new Vector3(0, -(Input.mouseScrollDelta.y * scrollSensitivity), 0);
        }

        if (transform.position.y < minZoomDistance)
        {
            transform.position = new Vector3(transform.position.x, minZoomDistance, transform.position.z);
        }
        if (transform.position.y > maxZoomDistance)
        {
            transform.position = new Vector3(transform.position.x, maxZoomDistance, transform.position.z);
        }

        lastMousePosition = Input.mousePosition;
    }
}