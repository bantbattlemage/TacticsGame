using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public GameCamera PlayerCamera;

    public void Initialize(GameMap map)
    {
        PlayerCamera.Initialize(map);
    }
}
