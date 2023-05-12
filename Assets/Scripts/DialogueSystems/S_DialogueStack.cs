using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueStack")]
public class S_DialogueStack : ScriptableObject, I_Dialogue, I_Requirement
{

    public List<S_Dialogue> dialogues = new List<S_Dialogue>();
    public int dialogueIndex = -1;
    [field: SerializeField] public bool isAchieved { get; set; }

    public string getText()
    {
        //check requirements of next dialogue, if good, then increase, else use the same dialogue index
        //return the dialogue index 
        //check if we can say the same thing again
        //return text until text index has ended
        Debug.Log("Bleeeeee getting the teeeeeeext");
        //this will increase dialogue index if the next dialogue meets all requirements
        checkNextRequirements();
        //check if we already said the dialogue, and if so if we are allowed to repeat it
        Debug.Log("Trying to get text at: " + dialogueIndex);
        if (!dialogues[dialogueIndex].isAchieved || !dialogues[dialogueIndex].oneTime)
        {
            Debug.Log("Get text");
            return dialogues[dialogueIndex].getText();
        }
        return null;
    }

    private void checkNextRequirements()
    {
        //if smaller than the dialogue options, then check, else we are already at the end.
        //check if its bigger than 0, as the initial is already being check by the dialogue handler
        //and we still wanna play the dialogue text of 0
        if (dialogueIndex < dialogues.Count - 1 && dialogueIndex > 0)
        {
            bool hasAchieved = false;
            foreach (I_Requirement requirement in dialogues[dialogueIndex + 1].requirements)
            {
                if (requirement.isAchieved)
                {
                    hasAchieved = false;
                    break;
                }
                hasAchieved = true;
            }

            if (hasAchieved)
            {
                dialogueIndex++;
                if (dialogueIndex >= dialogues.Count)
                {
                    isAchieved= true;
                }
            }
        }
    }

    public void checkRequirements()
    {

    }

    public Sprite getNpcSprite()
    {
        return null;
    }

    public Sprite getPlayerSprite()
    {
        return null;
    }

}
