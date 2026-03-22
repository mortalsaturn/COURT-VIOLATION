using System.Numerics;
using UnityEngine;

public enum ShotType
{
    BasicShot,
    Lob,
    DropShot,
}

public class ShotData
{
    public ShotType shotType;
    public UnityEngine.Vector3 direction;
    public float velocity;
    public string user;
}
