using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "ScriptableObjects/UnitDefinition", order = 1)]
public class UnitDefinition : GameDefinition
{
    public GameUnitType UnitType;
    public int BaseHealth = 10;
    public int BaseMovement = 3;
    public int BaseAttackRange = 1;
    public int BaseNumberOfAttacks = 1;

    public override GameDataType DataType => GameDataType.Entity;
    public override GameEntityType EntityType => GameEntityType.Unit;
}