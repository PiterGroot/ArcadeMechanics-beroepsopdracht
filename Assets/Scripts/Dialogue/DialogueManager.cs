using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private bool AutoPlay = false;

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
        if(dialogue.AutoPlay){
            AutoPlay = true;
        }
        PlayerPrefs.SetString("SoundFile", dialogue.SoundFileName);
        DialogueBox.SetActive(true);
        Anim.enabled = true;
        Anim.SetTrigger("AppearNow");
        FindObjectOfType<DialoguePosition>().SetPosition(dialogue.PositionsOnScreen);
        nameText.text = dialogue.name;
        sentences.Clear();
        foreach(string sentence in dialogue.Sentences)
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
        yield return new WaitForSeconds(.8f);
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            FindObjectOfType<AudioManager>().Play(PlayerPrefs.GetString("SoundFile"));
            yield return new WaitForSeconds(0.03f);
        }
        if(AutoPlay){
            yield return new WaitForSeconds(1.5f);
            DisplayNextSentence();
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
