using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary> GameManager object </summary>
    public GameManager manager;

    /// <summary> Audio source that plays the ding fx </summary>
    public AudioSource dingSource;

    /// <summary> Audio source that plays the dialogue </summary>
    public AudioSource dialogueSource;

    /// <summary> Other scripts can call functions in SoundManager </summary>
    public static SoundManager instance = null;

    /// <summary> List of dialogue audio </summary>
    public List<AudioClip> dialogue;

    /// <summary> Ding fx </summary>
    public AudioClip ding;

    void Awake()
    {
        // Check for instance of SoundManager
        if (instance == null)
            // If instance doesnt't exist, set to this
            instance = this;
        // If instance does exist...
        else if (instance != this)
            // Destroy this so we have a single instance of SoundManager (singleton)
            Destroy(gameObject);

        // Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading the scene
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Waits until audio is finished and then calls DialogueFinished in GameManager
    /// </summary>
    /// <param name="i"> Number of seconds </param>
    /// <returns></returns>
    private IEnumerator WaitAudio(int i)
    {
        yield return new WaitForSeconds(dialogueSource.clip.length);
        print("end of sound");
        manager.DialogueFinished(i);
    }

    /// <summary>
    /// Plays the dialogue associated with dialogueID
    /// </summary>
    /// <param name="dialogueID"> Index of the dialogue being played </param>
    public void PlayDialogue(int dialogueID)
    {
        // Select the current dialogue sound file
        dialogueSource.clip = dialogue[dialogueID];

        // Play the dialogue
        dialogueSource.Play();

        // Fire an event when the dialogue is finished
        StartCoroutine(WaitAudio(dialogueID));
    }

    /// <summary>
    /// Plays a ding sound effect
    /// </summary>
    public void PlayDing()
    {
        dingSource.clip = ding;
        dingSource.Play();
    }

}
