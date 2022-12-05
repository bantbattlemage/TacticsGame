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

	public PlayerDataObject Instantiate()
	{
		PlayerDataObject newPlayer = CreateInstance<PlayerDataObject>();
		newPlayer.ID = ID;
		newPlayer.name= Name;
		newPlayer.Money= Money;
		newPlayer.State = State;

		return newPlayer;
	}

	public static PlayerDataObject Instantiate(PlayerData playerData)
	{
		PlayerDataObject newPlayer = CreateInstance<PlayerDataObject>();

		newPlayer.ID = playerData.ID;
		newPlayer.name = playerData.Name;
		newPlayer.Money = playerData.Money;
		newPlayer.State = playerData.State;

		return newPlayer;
	}

	public new PlayerData ToData()
	{
		PlayerData playerData= new PlayerData();

		playerData.ID = ID;
		playerData.Name = Name;
		playerData.Money = Money;
		playerData.State = State;

		return playerData;
	}
}