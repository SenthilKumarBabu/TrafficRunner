using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoPrefabsData", menuName = "ScriptableObjects/PrefabsData", order = 1)]
public class SoPrefabsData : ScriptableObject
{
    public List<GameObject> roads;
    public List<GameObject> props;
    public List<GameObject> environments;
    public List<GameObject> vehicles;
    public List<GameObject> obstacles;
    public List<GameObject> platforms;
    public List<GameObject> ramps;
    public List<GameObject> powerUps;
    public List<GameObject> checkpoints;
}
