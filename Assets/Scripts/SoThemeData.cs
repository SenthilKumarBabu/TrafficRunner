using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ThemeData", menuName = "ScriptableObjects/ThemeData", order = 1)]
public class SoThemeData : ScriptableObject
{
    public string id;
    public RoadType roadType;
    public PropType propType;
    public EnvironmentType environmentType;
    
    private readonly Vector3 defaultRotation = new Vector3(0,-90,0);

    public Theme Create(GameManager gameManager,int totalNumberOfBlocksCreated)
    {
        var themeParent = new GameObject($"Theme{id}");
        var theme = themeParent.AddComponent<Theme>();
        var themeTransform = theme.transform;
        
        themeTransform.localPosition = new Vector3(0,0,totalNumberOfBlocksCreated * gameManager.blockSize);
        themeTransform.localEulerAngles = defaultRotation;
        
        var insRoad = Instantiate(gameManager.prefabsDataValue.roads[(int)roadType], themeTransform);
        var insProp = Instantiate(gameManager.prefabsDataValue.props[(int)propType], themeTransform);
        var insEnvironment = Instantiate(gameManager.prefabsDataValue.environments[(int)environmentType], themeTransform);
        
        var themeObject = new ThemeObject()
        {
            road = insRoad,
            props = insProp,
            environment = insEnvironment
        };

        var themeData = new ThemeData()
        {
            id = id,
            startPosition = totalNumberOfBlocksCreated * gameManager.blockSize,
            endPosition = (totalNumberOfBlocksCreated + 1) * gameManager.blockSize,
            roadType =  roadType,
            propType = propType,
            environmentType = environmentType
        };
        
        theme.Initialize(themeObject,themeData);
        return theme;
    }
}

public enum RoadType
{
    Road
}

public enum PropType
{
    Prop1,
    Prop2
}

public enum EnvironmentType
{
    Environment1,
    Environment2,
    Environment3
}