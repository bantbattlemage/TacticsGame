using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerDataObject : GameDataObject, IPlayerData
{
	[field: SerializeField]
	public int ID { get; set; }

	[field: SerializeField]
	public string Name { get; set; }

	[field: SerializeField]
	public int Money { get; set; }

	[field: SerializeField]
	public GamePlayerState State { get; set; }

	public override GameDataType DataType => GameDataType.Player;

	public new PlayerData GetData()
	{
		return new PlayerData();
	}
}