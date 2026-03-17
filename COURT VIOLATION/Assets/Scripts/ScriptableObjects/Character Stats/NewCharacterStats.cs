using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Scriptable Objects/NewCharacterStats")]
public class NewCharacterStats : ScriptableObject
{
    [Header("General")]
    public string characterName;

    [Header("Movement")]
    public float baseSpeed;
    public float sprintModifier;

    [Space(10)]
    public float jumpForce;

    [Space(10)]
    public float quickStepDistance;
    public float quickStepDuration;

    [Header("Swinging")]
    public float chargeRate;

    [Space(10)]
    public float basicShotBaseVel;
    public float basicShotMaxVel;

    [Space(10)]
    public float lobBaseVel;
    public float lobMaxVel;
}
