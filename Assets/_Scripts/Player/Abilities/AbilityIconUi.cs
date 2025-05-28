using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUi : MonoBehaviour
{
    public KeyCode abilityKey;
    public Ability linkedAbility;
    public Image cooldownOverlay;

    private void Update()
    {
        if (linkedAbility == null) return;

        float cooldown = linkedAbility.GetRemainingCooldown();
        float maxCooldown = linkedAbility.abilityCooldown;

        if (cooldown > 0)
        {
            cooldownOverlay.fillAmount = cooldown / maxCooldown;
        }
        else
        {
            cooldownOverlay.fillAmount = 0;
        }
    }
}
