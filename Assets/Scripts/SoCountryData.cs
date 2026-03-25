using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoCountryData", menuName = "ScriptableObjects/CountryData", order = 1)]
public class SoCountryData : ScriptableObject
{
    public List<SoDifficultyData> difficultyList;
    public List<SoThemeData> themeList;
    public List<SoBlockData> initialBlockList;
    public List<SoBlockData> middleBlockList;
    public List<SoBlockData> finalBlockList;

    public List<SoBlockData> GetBlockDataList(BlockType blockType)
    {
        if (blockType == BlockType.Beginning)
        {
            return initialBlockList;
        }
        else if(blockType == BlockType.Middle)
        {
            return middleBlockList;
        }
        else
        {
            return finalBlockList;
        }
    }
}
