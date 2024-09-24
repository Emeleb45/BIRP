using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public bool DialogueIsPlaying { get; private set; }
    private static DialogueManager instance;

    private Story currentStory;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Another instance of DialogueManager already exists");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        DialogueIsPlaying = false;
    }
    private void Update()
    {
        if (!DialogueIsPlaying)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ContinueStory();
        }
    }
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }
    public void ExitDialogueMode()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        StartCoroutine(WaitAndSetDialogueIsPlayingFalse());

        IEnumerator WaitAndSetDialogueIsPlayingFalse()
        {
            yield return new WaitForSeconds(1f); 
            DialogueIsPlaying = false;
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }
}
