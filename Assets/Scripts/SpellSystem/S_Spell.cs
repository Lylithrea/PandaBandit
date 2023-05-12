using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Spell")]
public class S_Spell : ScriptableObject, I_Spell
{
    public SpellType spellType;
    public SpellCastType castType;
    public Sprite vfxSprite;

    private SpellHandler spellHandler;

    public float speed;
    public float lifeTime = 2;
    public float channelTriggerSpeed = 0.1f;


    public void SetSpellHandler(SpellHandler handler)
    {
        spellHandler = handler;
    }

    public void CastSpell()
    {
        Debug.Log("Casting spell: " + spellType);

        switch (castType)
        {
            case SpellCastType.Instant:
                castInstantSpell();
                break;
            case SpellCastType.Channel:
                break;
            case SpellCastType.Toggle:
                break;
            default:
                Debug.LogWarning("Spell type " + spellType + " is not implemented to be casted.");
                break;
        }

    }
    private void castInstantSpell()
    {
        handleSpellCastType();
    }


    private GameObject handleSpellCastType()
    {
        switch (spellType)
        {
            case SpellType.Missle:
                return createMissle();

            case SpellType.AoE:
                return createAoE();

            case SpellType.Aura:
                return createAura(); ;

            default:
                Debug.LogWarning("Spell cast type " + spellType + " is not implemented to be handled.");
                return null;
        }
    }

    private GameObject createAoE()
    {

        GameObject newMissle = new GameObject();

        //implementation of range?? -------------------------------------

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        newMissle.transform.position = worldPosition;

        newMissle.AddComponent<SpellHelper>();
        SpellHelper helper = newMissle.GetComponent<SpellHelper>();


        helper.vfx = vfxSprite;
        helper.lifeTime = lifeTime;

        return newMissle;
    }


    private GameObject createMissle()
    {
        GameObject newMissle = new GameObject();

        newMissle.transform.position = spellHandler.gameObject.transform.position;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition -= spellHandler.gameObject.transform.position;
        worldPosition.z = 0;


        newMissle.AddComponent<SpellHelper>();
        SpellHelper helper = newMissle.GetComponent<SpellHelper>();
        helper.direction = worldPosition.normalized;
        helper.vfx = vfxSprite;
        helper.speed = speed;
        helper.lifeTime = lifeTime;
        return newMissle;
    }


    private GameObject createAura()
    {
        GameObject newMissle = new GameObject();
        newMissle.transform.SetParent(spellHandler.gameObject.transform);


        newMissle.AddComponent<SpellHelper>();
        SpellHelper helper = newMissle.GetComponent<SpellHelper>();

        helper.vfx = vfxSprite;
        //auras have lifetime? ------- mana limited?
        helper.lifeTime = lifeTime;
        return newMissle;
    }


}



public enum SpellType
{
    Missle,
    AoE,
    Aura
}

public enum SpellCastType
{
    Instant,
    Channel,
    Toggle
}