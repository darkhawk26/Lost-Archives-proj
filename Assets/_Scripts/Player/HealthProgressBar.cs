using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthProgressBar : MonoBehaviour
{
    public Slider slider;

    void Update()
    {
        slider.value = PlayerStats.GetCurrentHpProggres();
    }
}
