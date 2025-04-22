using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Instance;

    public List<Artifact> collectedArtifacts = new List<Artifact>();
    private Ability[] playerAbilities;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        playerAbilities = FindObjectsOfType<Ability>();
        ApplyAllArtifactEffects();
    }


    public void AddArtifact(Artifact artifact)
    {
        collectedArtifacts.Add(artifact);
        ApplyArtifactEffect(artifact);
        ArtifactUIManager.Instance.AddArtifactToUI(artifact);
    }

    private void ApplyArtifactEffect(Artifact artifact)
    {
        foreach (Ability ability in playerAbilities)
        {
            if (ability.abilityName.ToLower() == artifact.targetAbilityName.ToLower())
            {
                ability.ModifyAbility(
                    artifact.bonusDamage,
                    artifact.bonusRange,
                    artifact.bonusSpeed,
                    artifact.bonusDuration
                );

                Debug.Log($"[ArtifactManager] Applied {artifact.artifactName} to {ability.abilityName}");
            }
        }
    }

    private void ApplyAllArtifactEffects()
    {
        foreach (Artifact artifact in collectedArtifacts)
        {
            ApplyArtifactEffect(artifact);
        }
    }
}