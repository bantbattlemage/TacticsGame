using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitDataObject : GameEntityDataObject, IUnitData
{
	[field: SerializeField]
	public new UnitDefinition Definition { get; set; }

	[field: SerializeField]
	public int RemainingMovement { get; set; }

	[field: SerializeField]
	public int RemainingAttacks { get; set; }

	public override int RemainingActions { get { return RemainingMovement + RemainingAttacks; } }

	public UnitDataObject Instantiate()
	{
		UnitDataObject newUnit = CreateInstance<UnitDataObject>();
		newUnit.Definition = Definition;
		newUnit.RemainingMovement = RemainingMovement;
		newUnit.RemainingAttacks = RemainingAttacks;

		return newUnit;
	}

	public virtual new UnitData GetData()
	{
		UnitData data = new UnitData();

		return data;
	}
}