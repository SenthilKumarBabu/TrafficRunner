using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Sector : MonoBehaviour
{
    public List<Theme> themes;
    public List<Block> blocks;
    public SectorData data;
    
    public void Initialize(List<Theme> themeList, List<Block> blockList,SectorData data)
    {
        themes = themeList;
        blocks = blockList;
        this.data = data;
        
        for (int i = 0; i < themes.Count; i++)
        {
            themes[i].gameObject.SetActive(i < this.data.totalBlocksToBeShown);
        }
        
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].gameObject.SetActive(i < this.data.totalBlocksToBeShown);
        }
    }
}

[Serializable]
public struct SectorData
{
    public SectorTheme Theme;
    public float StartPosition;
    public float EndPosition;
    public int totalBlocksToBeShown;
}

public enum SectorTheme
{
    CountrySide,
    Village,
    Beach
}
