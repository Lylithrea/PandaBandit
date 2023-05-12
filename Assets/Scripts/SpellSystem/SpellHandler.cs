using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHandler : MonoBehaviour
{


    public float QAbilityCooldown;
    public float QCurrentCooldown;
    public float FAbilityCooldown;
    public float FCurrentCooldown;

    [SerializeField] public S_Spell QSlot;
    [SerializeField] public S_Spell FSlot;

    private bool pressedQ = false;
    private bool pressedF = false;


    private void OnValidate()
    {
        QSlot.SetSpellHandler(this);
        FSlot.SetSpellHandler(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (QCurrentCooldown <= 0)
            {
                Debug.Log("Handling Q Spell");
                QSlot.CastSpell();
                pressedQ = true;
                if (QSlot.castType != SpellCastType.Channel)
                {
                    QCurrentCooldown = QAbilityCooldown;
                    pressedQ = false;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (pressedQ)
            {
                pressedQ = false;
                QCurrentCooldown = QAbilityCooldown;
            }
        }
        if (Input.GetKey(KeyCode.F))
        {
            if (FCurrentCooldown <= 0)
            {
                Debug.Log("Handling F Spell");
                FSlot.CastSpell();
                if (FSlot.castType != SpellCastType.Channel)
                {
                    FCurrentCooldown = FAbilityCooldown;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (pressedF)
            {
                pressedF = false;
                FCurrentCooldown = FAbilityCooldown;
            }
        }
        


        if (QCurrentCooldown > 0)
        {
            QCurrentCooldown -= Time.deltaTime;
        }
        if (FCurrentCooldown > 0)
        {
            FCurrentCooldown -= Time.deltaTime;
        }

    }
}
