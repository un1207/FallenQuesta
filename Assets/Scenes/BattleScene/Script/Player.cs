using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; private set; } //PlayerฬผO
    public float Defense { get; private set; } //ํ_[Wธญฆ
    public float Speed { get; private set; } //ฺฎฌx
    public float Recover { get; private set; } //๊bิษ๑ท้Kbcส
    public List<string> Projectiles { get; private set; } = new List<string>();

    public Player(string name, float defense, float speed, float recover, List<string> projectiles)
    { 
        Name = name;
        Defense = defense;
        Speed = speed;
        Recover = recover;
        Projectiles = projectiles;
    }
}
