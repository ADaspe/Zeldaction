using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Objects",menuName = "ScriptableObjects/CharacterVariablesSO", order =1)]
public class AXD_CharacterVariablesSO : ScriptableObject
{
    public float pacificationRadius;
    public int maxEnemyPacification;
    public float ShieldDuration;
    public float SpiritDecreaseSpeed;
    public float DashDistance;
    public float DashTime;
    public float DashUpgradeDistance;
    public float DashUpgradeTime;
    public float DashCoolDown;
    public float SeparationDistance;
    public float SeparationTime;
    public float SFXRynScaredFrequency;
    public int initialHP;
    public float TorchActivationDuration;
    public float RynSpeed;
    public float SpiritSpeed;
    public float ShieldCooldown;
    public float AttackCooldown;
    public float AttackTogetherRange;
    public float StunTime;
    public float SpiritRespawn;
    public float SpeedGrabbing;
    public float TogetherAttackDetectionAngle;
    public float IdenProjectionDistance;
    public float IdenProjectionTime;
    public float IdenSlowDownTime;
    public float TimeToPatPat;
    
    public LayerMask EnemiesLayerMask;
}
