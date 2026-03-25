using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theme : MonoBehaviour
{
    public ThemeObject objs;
    public ThemeData data;
    public bool isTriggered;
    
    public void Initialize(ThemeObject obj,ThemeData data)
    {
        objs = obj;
        this.data = data;
    }
    
    public void Trigger()
    {
        //this.gameObject.SetActive(true);
        //var obstacle = Objs.ObstacleList[0].GetComponent<ZoneManager>();
        //obstacle.Trigger();
        isTriggered = true;
    }
}

[Serializable]
public struct ThemeObject
{
    public GameObject road;
    public GameObject props;
    public GameObject environment;

    public ThemeObject(GameObject road, GameObject props, GameObject environment)
    {
        this.road = road;
        this.props = props;
        this.environment = environment;
    }
}

[Serializable]
public struct ThemeData
{
    public string id;
    public float startPosition;
    public float endPosition;
    public RoadType roadType;
    public PropType propType;
    public EnvironmentType environmentType;

    public ThemeData(string id,float startPosition, float endPosition,RoadType roadType,PropType propType, EnvironmentType environmentType)
    {
        this.id = id;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.roadType = roadType;
        this.propType = propType;
        this.environmentType = environmentType;
    }
}