    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    public Artifact artifact;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collected");
            ArtifactManager.Instance.AddArtifact(artifact);
            Destroy(gameObject);
        }
    }
}
