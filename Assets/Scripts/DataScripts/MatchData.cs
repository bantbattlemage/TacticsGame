using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : GameData
{
	public int CurrentRound;
	public int CurrentActivePlayer;

	public override GameDataType DataType => GameDataType.Match;
}