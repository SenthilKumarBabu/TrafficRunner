using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "ScriptableObjects/DifficultyData", order = 1)]
public class SoDifficultyData : ScriptableObject
{
   public GameDifficulty type;
   public Vector2Int numberOfBlocksSpawnMinMax;
}

public enum GameDifficulty
{
   Easy,
   Medium,
   Hard
}