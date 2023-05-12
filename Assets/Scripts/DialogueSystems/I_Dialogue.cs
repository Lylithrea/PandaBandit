using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Dialogue 
{
    public bool isAchieved { get; set; }

    public string getText();
    public Sprite getNpcSprite();
    public Sprite getPlayerSprite();
}
