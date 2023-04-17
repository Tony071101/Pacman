using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PacmanDataClass
{
    public int lives;
    public int score;
    public float[] pos;

    public PacmanDataClass (GameManager pacman){
        lives = pacman.lives;
        score = pacman.score;
        pos = new float[2];
        pos[0] = pacman.transform.position.x;
        pos[1] = pacman.transform.position.y;
    }
}
