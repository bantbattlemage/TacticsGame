using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

public class MatchDataObject : GameDataObject, IMatchData
{
	[field: SerializeField]
	public int CurrentRound { get; set; }

	[field: SerializeField]
	public int CurrentActivePlayer { get; set; }

	public override GameDataType DataType => GameDataType.Match;
}