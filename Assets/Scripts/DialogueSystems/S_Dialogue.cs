using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueText
{
    public PlayerEmotes playerEmote;
    public NpcEmotes npcEmote;
    [TextArea]
    public string text;
}

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class S_Dialogue : ScriptableObject, I_Requirement, I_Dialogue
{
    public string characterName;
    public Priority priority;
    public bool oneTime = false;
    public List<ScriptableObject> requirements;

    public List<DialogueText> dialogues = new List<DialogueText>();
    public int textIndex = 0;

    [field:SerializeField] public bool isAchieved { get; set ; }

    public void OnValidate()
    {
        List<ScriptableObject> requirementsToBeRemoved = new List<ScriptableObject>();
        for (int i = 0; i < requirements.Count; i++)
        {
            if (requirements[i] is not I_Requirement && requirements[i] is not null)
            {
                requirementsToBeRemoved.Add(requirements[i]);
                Debug.LogWarning("Please use a 'requirement' item.");
            }
        }
        foreach (ScriptableObject toBeRemoved in requirementsToBeRemoved)
        {
            requirements.Remove(toBeRemoved);
        }
        requirementsToBeRemoved.Clear();

    }


    public string getText()
    {
        textIndex++;
        if (textIndex >= dialogues.Count)
        {
            isAchieved = true;
            textIndex = 0;
            return dialogues[dialogues.Count - 1].text;
        }
        return dialogues[textIndex - 1].text;
    }
    public Sprite getNpcSprite()
    {
        Sprite npcSprite = Resources.Load<Sprite>("Sprites/Emotes/" + characterName + "/" + dialogues[textIndex].npcEmote.ToString());
        if (npcSprite != null)
        {
            return npcSprite;
        }
        else
        {
            Debug.LogWarning("Could not find sprite for " + characterName + " with emote " + dialogues[textIndex].npcEmote.ToString());
            return null;
        }
    }

    public Sprite getPlayerSprite()
    {
        Sprite npcSprite = Resources.Load<Sprite>("Sprites/Emotes/Player/" + dialogues[textIndex].playerEmote.ToString());
        if (npcSprite != null)
        {
            return npcSprite;
        }
        else
        {
            Debug.LogWarning("Could not find sprite for player with emote " + dialogues[textIndex].playerEmote.ToString());
            return null;
        }
    }

}

public enum PlayerEmotes
{
    Neutral,
    Happy,
    Angry,
    Sad
}

public enum NpcEmotes
{
    Neutral,
    Happy,
    Angry,
    Sad
}

public enum Priority
{
    low,
    medium,
    high,
    urgent
}