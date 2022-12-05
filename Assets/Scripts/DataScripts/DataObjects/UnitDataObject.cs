using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitDataObject : GameEntityDataObject, IUnitData
{
	public new UnitDefinition Definition
	{
		get
		{
			return _definition;
		}
		set
		{
			base.Definition = value;
			_definition = value;
		}
	}

	private UnitDefinition _definition;

	[field: SerializeField]
	public int RemainingMovement { get; set; }

	[field: SerializeField]
	public int RemainingAttacks { get; set; }

	public override int RemainingActions { get { return RemainingMovement + RemainingAttacks; } }

	public UnitDataObject() 
	{
		if (Definition == null && DefinitionObject != null)
		{
			Definition = (DefinitionObject as UnitDefinitionObject).ToData();
		}
	}

	public UnitDataObject Instantiate()
	{
		UnitDataObject data = CreateInstance<UnitDataObject>();

		data.Definition = Definition;
		data.RemainingMovement = RemainingMovement;
		data.RemainingAttacks = RemainingAttacks;
		data.Location = Location;
		data.Owner = Owner;
		data.State = State;

		if (Definition == null && DefinitionObject != null)
		{
			data.Definition = (DefinitionObject as UnitDefinitionObject).ToData();
		}

		return data;
	}

	public static UnitDataObject Instantiate(UnitData unitData)
	{
		UnitDataObject data = CreateInstance<UnitDataObject>();

		data.Definition = unitData.Definition;
		data.RemainingMovement = unitData.RemainingMovement;
		data.RemainingAttacks = unitData.RemainingAttacks;
		data.Location = unitData.Location;
		data.Owner = unitData.Owner;
		data.State = unitData.State;

		return data;
	}

	public virtual new UnitData ToData()
	{
		UnitData data = new UnitData();

		data.Definition = Definition;
		data.RemainingMovement = RemainingMovement;
		data.RemainingAttacks = RemainingAttacks;
		data.Location = Location;
		data.Owner = Owner;
		data.State = State;

		return data;
	}
}