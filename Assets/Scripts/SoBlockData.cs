using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using PlatformCharacterController;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BlockData", menuName = "ScriptableObjects/BlockData", order = 1)]
public class SoBlockData : ScriptableObject
{
    public string id;
    public BlockType blockType;
    public List<VehicleData> vehicles;
    public List<ObstacleData> obstacles;
    public List<PlatformData> platforms;
    public List<RampData> ramps;
    public List<PowerUpData> powerUps;
    public List<CheckpointData> checkpoints;

    private readonly Vector3 defaultRotation = new Vector3(0,-90,0);
    
    private GameManager _gameManager;

    public Block Create(GameManager gameData, int totalNumberOfBlocksCreated)
    {
        var blockParent = new GameObject($"Block{id}");
        var block = blockParent.AddComponent<Block>();
        var blockTransform = block.transform;
        blockTransform.localPosition = new Vector3(0,0,totalNumberOfBlocksCreated * gameData.blockSize);
        blockTransform.localEulerAngles = defaultRotation;

        var vehicleIndexList = new List<VehicleType>();
        var obstacleIndexList = new List<ObstacleType>();
        var platformIndexList = new List<PlatformType>();
        var rampIndexList = new List<RampType>();
        var powerUpIndexList = new List<PowerUpType>();
        var checkpointIndexList = new List<CheckpointType>();
        var vehicleList = new List<GameObject>();
        var obstacleList = new List<GameObject>();
        var platformList = new List<GameObject>();
        var rampList = new List<GameObject>();
        var powerUpList = new List<GameObject>();
        var checkpointList = new List<GameObject>();
        if (vehicles.Count > 0)
        {
            var vehicleParent = new GameObject($"Vehicle");
            vehicleParent.transform.SetParent(blockTransform);
            vehicleParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < vehicles.Count; i++)
            {
                var insVehicle = Instantiate(gameData.prefabsDataValue.vehicles[(int)vehicles[i].type], vehicleParent.transform);
                insVehicle.transform.localPosition = vehicles[i].startPosition;
                insVehicle.transform.localEulerAngles = Vector3.zero;
                var vehicle = insVehicle.GetComponent<Vehicle>();
                vehicle.data = new VehicleData()
                {
                    type = vehicles[i].type,
                    vehicle = insVehicle,
                    startPosition = vehicles[i].startPosition,
                    endPosition = vehicles[i].endPosition,
                    moveSpeed = vehicles[i].moveSpeed
                };
                vehicleList.Add(insVehicle);
                vehicleIndexList.Add(vehicles[i].type);
            }
        }
        
        if (obstacles.Count > 0)
        {
            var obstacleParent = new GameObject($"Obstacle");
            obstacleParent.transform.SetParent(blockTransform);
            obstacleParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < obstacles.Count; i++)
            {
                var insObstacle = Instantiate(gameData.prefabsDataValue.obstacles[(int)obstacles[i].type], obstacleParent.transform);
                insObstacle.transform.localPosition = obstacles[i].position;
                if (obstacles[i].type == ObstacleType.ZebraCrossing)
                {
                    insObstacle.transform.localEulerAngles = defaultRotation;
                }
                else
                {
                    insObstacle.transform.localEulerAngles = Vector3.zero;
                }
                var obstacle = insObstacle.GetComponent<Obstacle>();
                obstacle.data = new ObstacleData()
                {
                    type = obstacles[i].type,
                    obstacle = insObstacle,
                    position = obstacles[i].position
                };
                obstacleList.Add(insObstacle);
                obstacleIndexList.Add(obstacles[i].type);
            }
        }
        
        if (platforms.Count > 0)
        {
            var platformParent = new GameObject($"Platform");
            platformParent.transform.SetParent(blockTransform);
            platformParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < platforms.Count; i++)
            {
                var insPlatform = Instantiate(gameData.prefabsDataValue.platforms[(int)platforms[i].type], platformParent.transform);
                insPlatform.transform.localPosition = platforms[i].position;
                insPlatform.transform.localEulerAngles = Vector3.zero;
                insPlatform.transform.localScale = platforms[i].scale;

                var platform = insPlatform.GetComponent<Platform>();
                if (platforms[i].type == PlatformType.Jumping)
                {
                    var jumpingPlatform = insPlatform.GetComponent<JumpingPlatform>();
                    jumpingPlatform.jumpForce = platforms[i].value;
                }
                else
                {
                    var changeSpeedZone = insPlatform.GetComponent<ChangeSpeedZone>();
                    changeSpeedZone.ZoneSpeed = platforms[i].value;
                }
                
                platform.data = new PlatformData()
                {
                    type = platforms[i].type,
                    platform = insPlatform,
                    position = platforms[i].position,
                    scale = platforms[i].scale,
                    value = platforms[i].value
                };
                platformList.Add(insPlatform);
                platformIndexList.Add(platforms[i].type);
            }
        }
        
        if (ramps.Count > 0)
        {
            var rampParent = new GameObject($"Ramp");
            rampParent.transform.SetParent(blockTransform);
            rampParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < ramps.Count; i++)
            {
                var insRamp = Instantiate(gameData.prefabsDataValue.ramps[(int)ramps[i].type], rampParent.transform);
                insRamp.transform.localPosition = ramps[i].position;
                insRamp.transform.localScale = ramps[i].scale;
                
                var ramp = insRamp.GetComponent<Ramp>();
                ramp.data = new RampData()
                {
                    type = ramps[i].type,
                    ramp = insRamp,
                    position = ramps[i].position,
                    scale = ramps[i].scale
                };
                rampList.Add(insRamp);
                rampIndexList.Add(ramps[i].type);
            }
        }
        
        if (powerUps.Count > 0)
        {
            var powerUpParent = new GameObject($"PowerUp");
            powerUpParent.transform.SetParent(blockTransform);
            powerUpParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < powerUps.Count; i++)
            {
                var insPowerUp = Instantiate(gameData.prefabsDataValue.powerUps[(int)powerUps[i].type], powerUpParent.transform);
                insPowerUp.transform.localPosition = powerUps[i].position;
                
                var powerUp = insPowerUp.GetComponent<PowerUp>();
                powerUp.data = new PowerUpData()
                {
                    type = powerUps[i].type,
                    powerUp = insPowerUp,
                    position = powerUps[i].position,
                };
                powerUpList.Add(insPowerUp);
                powerUpIndexList.Add(powerUps[i].type);
            }
        }
        
        if (checkpoints.Count > 0)
        {
            var checkPointsParent = new GameObject($"Checkpoints");
            checkPointsParent.transform.SetParent(blockTransform);
            checkPointsParent.transform.localPosition = Vector3.zero;

            for (int i = 0; i < checkpoints.Count; i++)
            {
                var insCheckPoint = Instantiate(gameData.prefabsDataValue.checkpoints[(int)checkpoints[i].type], checkPointsParent.transform);
                insCheckPoint.transform.localPosition = checkpoints[i].position;
                
                var checkpoint = insCheckPoint.GetComponent<Checkpoint>();
                checkpoint.data = new CheckpointData()
                {
                    type = checkpoints[i].type,
                    checkpoint = insCheckPoint,
                    position = checkpoints[i].position,
                };
                checkpointList.Add(insCheckPoint);
                checkpointIndexList.Add(checkpoints[i].type);
            }
        }
        
        var blockObject = new BlockObject()
        {
            vehicleList = vehicleList,
            obstacleList = obstacleList,
            platformList = platformList,
            rampList = rampList,
            powerUpList = powerUpList,
            checkpointList = checkpointList
        };

        var blockData = new BlockData()
        {
            id = id,
            startPosition = totalNumberOfBlocksCreated * gameData.blockSize,
            endPosition = (totalNumberOfBlocksCreated + 1) * gameData.blockSize,
            vehicleIndexList = vehicleIndexList,
            obstacleTypeList = obstacleIndexList,
            platformTypeList = platformIndexList,
            rampTypeList = rampIndexList,
            powerUpTypeList = powerUpIndexList,
            checkpointTypeList = checkpointIndexList
        };
        
        block.Initialize(blockObject,blockData);
        return block;
    }

    public BlockType GetBlockType()
    {
        return blockType;
    }
}

public enum BlockType
{
    Beginning,
    Middle,
    Finish
}