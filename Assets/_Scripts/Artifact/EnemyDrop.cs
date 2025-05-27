using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    private bool isQuitting = false;

    public List<Artifact> possibleDrops = new List<Artifact>();
    [Range(0, 1)] public float dropChance = 0.1f;
 

    public void DropArtifact()
    {
        List<Artifact> validDrops = possibleDrops.FindAll(artifact =>
       artifact != null &&
       !ArtifactManager.Instance.collectedArtifacts.Any(a => a.artifactName == artifact.artifactName)
        );

        
        if (validDrops.Count > 0 && Random.value <= dropChance)
        {
            Artifact selectedDrop = validDrops[Random.Range(0, validDrops.Count)];

            GameObject pickup = Instantiate(
                selectedDrop.artifactPickupPrefab,
                transform.position,
                Quaternion.identity
            );

            pickup.GetComponent<ArtifactPickup>().artifact = selectedDrop;
        }
    }
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!isQuitting)
        {
            DropArtifact();
        }
    }
}
