using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private GameObject[] choices;
    [SerializeField] private TextAsset loadGlobalsJSON;
    private TextMeshProUGUI[] choicesText;
    private Animator layoutAnimator;
    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
    private static DialogueManager instance;
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private DialogueVariable dialogueVariables;


    private void Awake(){
        if(instance != null){
            Debug.LogWarning("Found more than one");
        }
        instance = this;

        dialogueVariables = new DialogueVariable(loadGlobalsJSON);
    }

    public static DialogueManager GetInstance(){
        return instance;
    }

    private void Start()
    {
       dialogueIsPlaying = false;
       dialoguePanel.SetActive(false); 

       layoutAnimator = dialoguePanel.GetComponent<Animator>();

       choicesText = new TextMeshProUGUI[choices.Length];
       int index = 0;
       foreach (GameObject choice in choices){
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update(){
        if(!dialogueIsPlaying){
            return;
        }

        if(currentStory.currentChoices.Count == 0 && InputManager.GetInstance().GetSubmitPressed()){
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    { 
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);

        displayNameText.text = "???";
        portraitAnimator.Play("DefaultIcon");
        layoutAnimator.Play("Right");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode(){

        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

    }

    private void ContinueStory(){
        if(currentStory.canContinue){
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
            HandleTags(currentStory.currentTags);
        }
        else{
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(List<string> currentTags){
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if(splitTag.Length != 2){
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch(tagKey){
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came but not successfully handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices(){
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length){
            Debug.LogError("More choices than the UI could support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices){
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for(int i=index; i<choices.Length; i++){
            choices[i].gameObject.SetActive(false);
        }

        //StartCoroutine(SelectFirstChoice());
    }

    // private IEnumerator SelectFirstChoice(){
    //     EventSystem.current.SetSelectedGameObject(null);
    //     yield return new WaitForEndOfFrame();
    //     EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    // }

    public void MakeChoice(int choiceIndex){
        currentStory.ChooseChoiceIndex(choiceIndex);
        InputManager.GetInstance().RegisterSubmitPressed();
        ContinueStory();
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }
}
