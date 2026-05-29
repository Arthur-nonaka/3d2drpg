using System.Collections;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    private Story story;
    public TMP_Text name;
    public TMP_Text dialogueText;
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    [Header("Typing Effect")]
    public float typingSpeed = 0.08f;
    private Coroutine typingCoroutine;

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    public int blipEveryNChars = 2;

    void Start()
    {
        story = new Story(inkJSONAsset.text);
        NextLine();
    }

    void Update()
    {
        if (
            Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Z)
            || Input.GetMouseButtonDown(0)
        )
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = story.currentText;
                typingCoroutine = null;
                ShowChoices();
                audioSource.PlayOneShot(typingSound);
                return;
            }

            if (choicesPanel.transform.childCount > 0)
            {
                return;
            }

            NextLine();
        }
    }

    void ShowText()
    {
        dialogueText.text = "";
        name.text =
            story.currentTags.Count > 0 ? story.currentTags[0].Replace("speaker:", "") : "Narrador";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(story.currentText));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];

            if (typingSound && audioSource && i % blipEveryNChars == 0)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;

        ShowChoices();
    }

    void ShowChoices()
    {
        ClearChoices();
        if (story.currentChoices.Count == 0)
        {
            return;
        }

        foreach (var choice in story.currentChoices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.text;

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        ClearChoices();
        NextLine();
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void NextLine()
    {
        if (!story.canContinue)
        {
            // dialogueText.text = "";
            // ClearChoices();
            return;
        }
        story.Continue();
        ShowText();
    }
}
