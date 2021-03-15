using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Animator Anim;
    public GameObject DialogueBox;
    public Text nameText;
    public Text dialogueText;
    public Queue<string> sentences;

    void Start()
    {
        DialogueBox.SetActive(false);
        Anim.enabled = false;
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        PlayerPrefs.SetString("SoundFile", dialogue.SoundFileName);
        DialogueBox.SetActive(true);
        Anim.enabled = true;
        Anim.SetTrigger("AppearNow");
        FindObjectOfType<DialoguePosition>().SetPosition(dialogue.PositionsOnScreen);
        nameText.text = dialogue.name;
        sentences.Clear();
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            FindObjectOfType<AudioManager>().Play(PlayerPrefs.GetString("SoundFile"));
            yield return new WaitForSeconds(0.03f);
        }
    }
    void EndDialogue()
    {
        Anim.SetTrigger("DisAppearNow");
    }
    public void MakeInvisible()
    {
        FindObjectOfType<DialoguePosition>().SetPosition(new Vector2(100, 448));
        DialogueBox.SetActive(false);
        Anim.enabled = false;
    }
}
