using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    public BlockObject objs;
    public BlockData data;
    public bool isTriggered;

    public void Initialize(BlockObject obj,BlockData data)
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
public struct BlockObject
{
    public List<GameObject> vehicleList;
    public List<GameObject> obstacleList;
    public List<GameObject> platformList;
    public List<GameObject> rampList;
    public List<GameObject> powerUpList;
    public List<GameObject> checkpointList;

    public BlockObject(List<GameObject> vehicleList, List<GameObject> obstacleList, List<GameObject> platformList, List<GameObject> rampList, List<GameObject> powerUpList, List<GameObject> checkpointList)
    {
        this.vehicleList = vehicleList;
        this.obstacleList = obstacleList;
        this.platformList = platformList;
        this.rampList = rampList;
        this.powerUpList = powerUpList;
        this.checkpointList = checkpointList;
    }
}

[Serializable]
public struct BlockData
{
    public string id;
    public float startPosition;
    public float endPosition;
    public List<VehicleType> vehicleIndexList;
    public List<ObstacleType> obstacleTypeList;
    public List<PlatformType> platformTypeList;
    public List<RampType> rampTypeList;
    public List<PowerUpType> powerUpTypeList;
    public List<CheckpointType> checkpointTypeList;

    public BlockData(string id,float startPosition, float endPosition, List<VehicleType> vehicleIndexList, List<ObstacleType> obstacleTypeList, List<PlatformType> platformTypeList, List<RampType> rampTypeList, List<PowerUpType> powerUpTypeList, List<CheckpointType> checkpointTypeList)
    {
        this.id = id;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.vehicleIndexList = vehicleIndexList;
        this.obstacleTypeList = obstacleTypeList;
        this.platformTypeList = platformTypeList;
        this.rampTypeList = rampTypeList;
        this.powerUpTypeList = powerUpTypeList;
        this.checkpointTypeList = checkpointTypeList;
    }
}


