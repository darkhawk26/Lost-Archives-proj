using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ArtifactUIManager : MonoBehaviour
{
    public static ArtifactUIManager Instance;

    public GameObject artifactPanel;
    public GameObject artifactEntryPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddArtifactToUI(Artifact artifact)
    {
        GameObject entry = Instantiate(artifactEntryPrefab, artifactPanel.transform);

        Image iconImage = entry.transform.GetChild(0).GetComponent<Image>();
    

        iconImage.sprite = artifact.icon;
       
    }
}