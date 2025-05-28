using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltimateProgressUI : MonoBehaviour
{
    public Slider progressSlider;

    void Update()
    {
        progressSlider.value = Ability.GetCurrentUltimateProgress();
    }
}
