using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEntityData", menuName = "ScriptableObjects/GameEntityData", order = 1)]
public class GameEntityDataObject : GameDataObject, IGameEntityData<GameDefinition>, IGameDataProvider<GameEntityData<GameDefinition>>
{
	public GameDefinitionObject DefinitionObject;

	public GameDefinition Definition { get; set; }

	[field: SerializeField]
	public int Owner { get; set; }

	[field: SerializeField]
	public Point Location { get; set; }

	[field: SerializeField]
	public int RemainingHealth { get; set; }

	[field: SerializeField]
	public GameEntityState State { get; set; }

	public virtual int RemainingActions { get { return 0; } }

	public override GameDataType DataType
	{
		get
		{
			return GameDataType.Entity;
		}
	}

	public GameEntityDataObject()
	{
		if (DefinitionObject != null && Definition == null)
		{
			Definition = DefinitionObject.ToData();
		}
	}

	public override GameDataType OutputDataToString(out string info)
	{
		info = "";

		return DataType;
	}

	public virtual new GameEntityData<GameDefinition> ToData()
	{
		GameEntityData<GameDefinition> data = new GameEntityData<GameDefinition>();

		return data;
	}
}