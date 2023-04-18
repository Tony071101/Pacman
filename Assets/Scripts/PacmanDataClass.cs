using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PacmanDataClass
{
    public int lives, score;
    public Vector3 pos;
    public Vector2 rotationAngle;
    // public List<PelletData> pelletsData;
    public List<GhostData> ghostData;
    public List<Vector3> eatenPelletPositions;

    public PacmanDataClass()
    {
        // Initialize ghostDataList
        ghostData = new List<GhostData>();
    }
}
[System.Serializable]
public class PelletData
{
    public Vector3 pos; 
    public bool isEaten;// Position of the pellet
}
[System.Serializable]
public class GhostData
{
    public Vector3 position;
    public Vector2 direction;
}
