using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dummy : MonoBehaviour
{
    public TextMeshProUGUI textbox;

    public EquipmentDamage[] resitances;


    public void DoDamage(EquipmentDamage[] damage)
    {
        Debug.Log("Dealing damage to dummy!");
        textbox.text = "Damage dealt: \n";
        foreach (EquipmentDamage damageType in damage)
        {
            textbox.text += "Damage type: " + damageType.damageType.ToString() + " - Original amount: " + damageType.amount + "\n";
            textbox.text += "Resitances : " + getResistances(damageType.damageType) + " - New value: " + (damageType.amount - damageType.amount * (getResistances(damageType.damageType)/ 100)) + "\n";
        }

    }

    private float getResistances(DAMAGETYPES damageType)
    {
        for (int i = 0; i < resitances.Length; i++)
        {
            if (resitances[i].damageType == damageType)
            {
                return resitances[i].amount;
            }
        }
        return 0;
    }


}
