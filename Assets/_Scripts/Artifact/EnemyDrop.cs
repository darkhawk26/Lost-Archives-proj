using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    private bool isQuitting = false;

    public Artifact possibleDrop;
    [Range(0, 1)] public float dropChance = 0.1f;
    public GameObject pickupPrefab;

    public void DropArtifact()
    {
        if (Random.value <= dropChance && possibleDrop != null)
        {
            GameObject pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            pickup.GetComponent<ArtifactPickup>().artifact = possibleDrop;
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
