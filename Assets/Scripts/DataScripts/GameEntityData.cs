using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEntityData", menuName = "ScriptableObjects/GameEntityData", order = 1)]
public class GameEntityData : ScriptableObject
{
    public int Owner = -1;
    public GameObject Prefab;
}