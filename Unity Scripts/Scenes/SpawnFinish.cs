using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFinish : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start() 
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void Update() 
    {
        string quizAnswer = ((Ink.Runtime.StringValue) DialogueManager
            .GetInstance()
            .GetVariableState("quiz_answer")).value;

        switch (quizAnswer) 
        {
            case "Hendrik":
                spriteRenderer.enabled = true;
                break;
        }
    }
}
