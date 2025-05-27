using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Artifacts/Artifact")]
public class Artifact : ScriptableObject
{

    public GameObject artifactPickupPrefab;


    public string artifactName;
    public Sprite icon;
    public string description;

    
    public string targetAbilityName;

 
    public float bonusDamage;
    public float bonusRange;
    public float bonusSpeed;
    public float bonusCooldown;
    public float bonusDuration;
}