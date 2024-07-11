using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
public class Level : ScriptableObject {
    public int levelNum;
    public bool isLocked = true;
    public bool hasCleared;
    public List<BrickCols> BrickRows;
    public Sprite levelBG;

    public Level(
        bool isLocked,
        bool hasCleared, 
        List<BrickCols> BrickRows
    ) {
        this.isLocked = isLocked;
        this.hasCleared = hasCleared;
        this.BrickRows = BrickRows;
    }
}

[Serializable]
public struct BrickCols {
    public List<int> brickColSize;
}