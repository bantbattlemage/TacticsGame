using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	private int minZoomDistance = 20;
	private int maxZoomDistance = 100;
	private float mousePanSensitivity = 0.1f;
	private float scrollSensitivity = 5;
	private float autoPanSensitivity = 0.05f;
	private float maxPanDuration = 1f;

	private bool isInitialized = false;
	private Vector3 lastMousePosition;
	private Vector2 mapSize;
	private bool ignoreInput = false;
	private Transform panTarget = null;
	private float panDuration = 0f;

	public void Initialize(GameMap map)
	{
		mapSize = map.MapSize;
		mapSize.Scale(new Vector2(GameMap.TileSize, GameMap.TileSize));
		isInitialized = true;
	}

	void Update()
	{
		//  auto-pan
		if (panTarget != null)
		{
			Vector2 delta = new Vector2(panTarget.transform.position.x - transform.position.x, panTarget.transform.position.z - transform.position.z);
			delta.Scale(new Vector2(autoPanSensitivity, autoPanSensitivity));
			panDuration += Time.deltaTime;

			if (PanTo(delta) || panDuration >= maxPanDuration)
			{
				panTarget = null;
				ignoreInput = false;
				panDuration = 0f;
			}
		}

		if (!isInitialized || !isActiveAndEnabled || ignoreInput)
		{
			if(panTarget != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
			{
				panDuration += maxPanDuration;
			}

			return;
		}

		//  pan
		if ((Input.GetMouseButton(1) || Input.GetMouseButton(2)) && lastMousePosition != null)
		{
			Vector2 delta = new Vector2(lastMousePosition.x - Input.mousePosition.x, lastMousePosition.y - Input.mousePosition.y);
			delta.Scale(new Vector2(mousePanSensitivity, mousePanSensitivity));
			PanTo(delta);
		}

		//  zoom
		if (Input.mouseScrollDelta.y != 0 && !PlayerUI.IsMouseOverInterface())
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

	private bool PanTo(Vector2 target)
	{
		bool[] bounded = new bool[2] { false, false };

		gameObject.transform.position += new Vector3(target.x, 0, target.y);

		if (transform.position.x >= mapSize.x)
		{
			transform.position = new Vector3(mapSize.x, transform.position.y, transform.position.z);
			bounded[0] = true;
		}
		if (transform.position.z >= mapSize.y)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, mapSize.y);
			bounded[1] = true;
		}
		if (transform.position.x <= 0)
		{
			transform.position = new Vector3(0, transform.position.y, transform.position.z);
			bounded[0] = true;
		}
		if (transform.position.z <= 0)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			bounded[1] = true;
		}
		if (transform.position.x >= mapSize.x)
		{
			transform.position = new Vector3(mapSize.x, transform.position.y, transform.position.z);
			bounded[0] = true;
		}
		if (transform.position.z >= mapSize.y)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, mapSize.y);
			bounded[1] = true;
		}
		if (transform.position.x <= 0)
		{
			transform.position = new Vector3(0, transform.position.y, transform.position.z);
			bounded[0] = true;
		}
		if (transform.position.z <= 0)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			bounded[1] = true;
		}

		return bounded[0] && bounded[1];
	}

	public void PanTo(Transform target)
	{
		ignoreInput = true;
		panTarget = target;
	}
}