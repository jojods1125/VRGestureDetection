using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Manager References")]
    public HandManager handManager;
    public SoundManager soundManager;

    [Header("Displayed Dialogue")]
    [TextArea]
    public string[] dialogueText;

    [Header("Manipulated Objects")]
    public TextMesh speechText;
    public GameObject gestureText;
    public GameObject slidingWallN;
    public GameObject slidingWallW;
    public GameObject slidingWallS;
    public GameObject slidingWallE;
    public GameObject leftHandPic;
    public GameObject rightHandPic;
    public GameObject rpsPic;
    public GameObject[] gestureListObjs;
    public GameObject[] aslGuides;
    public Material[] guideMats;

    /// <summary> Dialogue index that needs to be played next </summary>
    private int currentDialogue = 0;

    /// <summary> Distance the player's arm must be extended for primary hand setting </summary>
    private readonly float extendedValue = 0.44f;

    /// <summary> Used in the demo to track how many gestures have been made in the first test </summary>
    private int numGestures = 0;

    /// <summary>
    /// Tracks if the demo is running. If the demo is not active, gesture coroutines stay on and turn themselves on after running
    /// </summary>
    private bool demoActive = true;

    /// <summary>
    /// Tracks if the narrator is speaking due to gesture detection and prevents overlap if so
    /// </summary>
    private bool speaking = false;

    /// <summary> Rounds of rock paper scissors completed in the demo </summary>
    private int roundsRPSComplete = 0;

    /// <summary> What part of ASL portion the player is currently on </summary>
    private int repeatASL = 0;

    // Sets up libraries for demo
    void Start()
    {
        // Disables all GestureLibrary
        foreach (GestureLibrary g in handManager.gameObject.GetComponents<GestureLibrary>())
        {
            g.enabled = false;
        }

        // Enables SocialGestures library
        handManager.gameObject.GetComponent<SocialGestures>().enabled = true;

        // Opens wall
        StartCoroutine(ScaleOverTime(slidingWallN, 3, 0.5f));
        StartCoroutine(DelayDialogue(3, currentDialogue));
    }

    /// <summary>
    /// Called when dialogue finishes playing in SoundManager
    /// </summary>
    /// <param name="i"> Dialogue index that just played </param>
    public void DialogueFinished(int i)
    {
        switch (i)
        {
            case 3: // Select Primary Hand
                StartCoroutine(DetectPrimaryHand());
                break;

            case 4: // Jump over right dialogue if left
                currentDialogue = 6;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 7: // Confirm Primary Hand
                StartCoroutine(ConfirmPrimaryHand());
                break;

            case 9: // ConfirmContinue for Demo
                StartCoroutine(ConfirmContinue(i));
                break;

            case 12: // First dialogue of list portion
                StartCoroutine(ScaleOverTime(slidingWallW, 3, 0.5f));
                StartCoroutine(DelayGestureLists(3));
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 14: // Start of list portion
                handManager.gameObject.GetComponent<ASLphabetGestures>().enabled = false;
                StartCoroutine(DelayGestureLists(0));
                speechText.text = "";
                StartCoroutine(DetectWaving());
                StartCoroutine(DetectThumbsUp());
                StartCoroutine(DetectThumbsDown());
                StartCoroutine(DetectOkay());
                StartCoroutine(DetectPeace());
                StartCoroutine(DetectPinkyPromise());
                StartCoroutine(DetectWolfie());
                StartCoroutine(DetectMiddleFinger());
                break;

            case 15: // Waving Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[6].GetComponent<MeshRenderer>().enabled = true;
                } else
                {
                    StartCoroutine(DetectWaving());
                }
                break;

            case 16: // Thumb's Up Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[2].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    StartCoroutine(DetectThumbsUp());
                }
                break;

            case 17: // Thumb's Down Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[3].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    StartCoroutine(DetectThumbsDown());
                }
                break;

            case 18: // Okay Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[5].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    StartCoroutine(DetectOkay());
                }
                break;

            case 19: // Peace Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[7].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    StartCoroutine(DetectPeace());
                }
                break;

            case 20: // Pinky Promise Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    IncrementNumGestures();
                    gestureListObjs[4].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    StartCoroutine(DetectPinkyPromise());
                }
                break;

            case 21: // Wolfie Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {

                }
                else
                {
                    StartCoroutine(DetectWolfie());
                }
                break;

            case 22: // Middle Finger Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {

                }
                else
                {
                    StartCoroutine(DetectMiddleFinger());
                }
                break;

            case 23: // First dialogue of picture portion
                StartCoroutine(ScaleOverTime(slidingWallE, 3, 0.5f));
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 24: // Start of picture portion
                if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                {
                    rightHandPic.GetComponent<MeshRenderer>().enabled = true;
                    rightHandPic.GetComponent<MeshRenderer>().material = guideMats[0];
                } else
                {
                    leftHandPic.GetComponent<MeshRenderer>().enabled = true;
                    leftHandPic.GetComponent<MeshRenderer>().material = guideMats[0];
                }
                speechText.text = "";
                StartCoroutine(DetectHangTen());
                break;

            case 25: // Hang Ten Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Display picture
                    if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                    {
                        rightHandPic.GetComponent<MeshRenderer>().material = guideMats[1];
                    }
                    else
                    {
                        leftHandPic.GetComponent<MeshRenderer>().material = guideMats[1];
                    }
                    StartCoroutine(DetectRockAndRoll());
                }
                else
                {
                    StartCoroutine(DetectHangTen());
                }
                break;

            case 26: // Rock and Roll Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Display picture
                    if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                    {
                        rightHandPic.GetComponent<MeshRenderer>().material = guideMats[2];
                    }
                    else
                    {
                        leftHandPic.GetComponent<MeshRenderer>().material = guideMats[2];
                    }
                    StartCoroutine(DetectSpiderMan());
                }
                else
                {
                    StartCoroutine(DetectRockAndRoll());
                }
                break;

            case 27: // Spider Man Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Display picture
                    if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                    {
                        rightHandPic.GetComponent<MeshRenderer>().material = guideMats[3];
                    }
                    else
                    {
                        leftHandPic.GetComponent<MeshRenderer>().material = guideMats[3];
                    }
                    StartCoroutine(DetectVulcanSalute());
                }
                else
                {
                    StartCoroutine(DetectSpiderMan());
                }
                break;

            case 28: // Vulcan Salute Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Display picture
                    if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                    {
                        rightHandPic.GetComponent<MeshRenderer>().material = guideMats[4];
                    }
                    else
                    {
                        leftHandPic.GetComponent<MeshRenderer>().material = guideMats[4];
                    }
                    StartCoroutine(DetectScoutsHonor());
                }
                else
                {
                    StartCoroutine(DetectVulcanSalute());
                }
                break;

            case 29: // Scout's Honor Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Display picture
                    if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
                    {
                        rightHandPic.GetComponent<MeshRenderer>().material = guideMats[5];
                    }
                    else
                    {
                        leftHandPic.GetComponent<MeshRenderer>().material = guideMats[5];
                    }
                    StartCoroutine(DetectFingerGun());
                }
                else
                {
                    StartCoroutine(DetectScoutsHonor());
                }
                break;

            case 30: // Finger Gun Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    currentDialogue++;
                    speechText.text = dialogueText[currentDialogue];
                    soundManager.PlayDialogue(currentDialogue);
                }
                else
                {
                    StartCoroutine(DetectFingerGun());
                }
                break;

            case 31: // First dialogue in RPS portion
                rpsPic.GetComponent<MeshRenderer>().material = guideMats[10];
                rpsPic.GetComponent<MeshRenderer>().enabled = true;
                StartCoroutine(ScaleOverTime(rpsPic, 2, 0.5f, 0.5f, 0));
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 37: // First RPS match
                StartCoroutine(DetectRock());
                StartCoroutine(DetectPaper());
                StartCoroutine(DetectScissors());
                speechText.text = "...";
                break;

            case 38: // Paper Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StopAllCoroutines();
                    roundsRPSComplete++;
                    if (roundsRPSComplete == 1)
                    {
                        currentDialogue = 39;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 2)
                    {
                        currentDialogue = 40;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 3)
                    {
                        currentDialogue = 41;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                }
                else
                {
                    StartCoroutine(DetectPaper());
                }
                break;

            case 46: // Scissors Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StopAllCoroutines();
                    roundsRPSComplete++;
                    if (roundsRPSComplete == 1)
                    {
                        currentDialogue = 39;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 2)
                    {
                        currentDialogue = 40;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 3)
                    {
                        currentDialogue = 41;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                }
                else
                {
                    StartCoroutine(DetectScissors());
                }
                break;

            case 47: // Rock Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StopAllCoroutines();
                    roundsRPSComplete++;
                    if (roundsRPSComplete == 1)
                    {
                        currentDialogue = 39;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 2)
                    {
                        currentDialogue = 40;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                    if (roundsRPSComplete == 3)
                    {
                        currentDialogue = 41;
                        speechText.text = dialogueText[currentDialogue];
                        soundManager.PlayDialogue(currentDialogue);
                    }
                }
                else
                {
                    StartCoroutine(DetectRock());
                }
                break;

            case 39: // Second RPS match
                currentDialogue = 35;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 40: // Final RPS match
                currentDialogue = 35;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 45: // ConfirmContinue that leads to ASL portion
                StartCoroutine(ConfirmContinue(47));
                break;

            case 48: // First dialogue in ASL portion
                // Cleans room
                foreach (GameObject g in gestureListObjs)
                {
                    g.GetComponent<MeshRenderer>().enabled = false;
                }
                leftHandPic.GetComponent<MeshRenderer>().enabled = false;
                rightHandPic.GetComponent<MeshRenderer>().enabled = false;
                rpsPic.GetComponent<MeshRenderer>().enabled = false;
                repeatASL = 0;
                // Make the snap gesture the only one that can be detected from SocialGestures
                foreach (string g in handManager.gameObject.GetComponent<SocialGestures>().GetGestureList())
                {
                    if (g != "snap")
                    {
                        handManager.gameObject.GetComponent<SocialGestures>().SetGestureActive(g, false);
                    }
                }
                // Starts ASL portion
                handManager.gameObject.GetComponent<ASLphabetGestures>().enabled = true;
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;




            case 51: // Spawns ASL List 1
                StartCoroutine(ScaleOverTime(rpsPic, 2, 0.5f, 0.5f, 0));
                rpsPic.GetComponent<MeshRenderer>().material = guideMats[6];
                rpsPic.GetComponent<MeshRenderer>().enabled = true;
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 52: // ASL List 1 Begin
                StartCoroutine(DetectA());
                break;

            case 53: // A Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectB());
                }
                else
                {
                    StartCoroutine(DetectA());
                }
                break;

            case 54: // B Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectC());
                }
                else
                {
                    StartCoroutine(DetectB());
                }
                break;

            case 55: // C Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectD());
                }
                else
                {
                    StartCoroutine(DetectC());
                }
                break;

            case 56: // D Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectE());
                }
                else
                {
                    StartCoroutine(DetectD());
                }
                break;

            case 57: // E Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectF());
                }
                else
                {
                    StartCoroutine(DetectE());
                }
                break;

            case 58: // F Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    if (repeatASL == 0) { currentDialogue++; } else { currentDialogue = 60; }
                    speechText.text = dialogueText[currentDialogue];
                    soundManager.PlayDialogue(currentDialogue);
                }
                else
                {
                    StartCoroutine(DetectF());
                }
                break;

            case 59: // ASL List 1 Repeat
                repeatASL++;
                // REMOVE LIST OF ASL
                rpsPic.GetComponent<MeshRenderer>().enabled = false;
                StopAllCoroutines();
                // ADD COROUTINE FOR SNAPPING
                StartCoroutine(DetectSnap());
                if (repeatASL == 1)
                {
                    StartCoroutine(DetectA());
                }
                else if (repeatASL == 2)
                {
                    StartCoroutine(DetectG());
                }
                else if (repeatASL == 3)
                {
                    StartCoroutine(DetectMN());
                }
                else
                {
                    StartCoroutine(DetectT());
                }
                break;

            case 60: // ASL List 2 Begin
                // ADD LIST OF ASL
                rpsPic.GetComponent<MeshRenderer>().material = guideMats[7];
                rpsPic.GetComponent<MeshRenderer>().enabled = true;
                StartCoroutine(DetectG());
                break;

            case 61: // G Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectH());
                }
                else
                {
                    StartCoroutine(DetectG());
                }
                break;

            case 62: // H Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectI());
                }
                else
                {
                    StartCoroutine(DetectH());
                }
                break;

            case 63: // I Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectJ());
                }
                else
                {
                    StartCoroutine(DetectI());
                }
                break;

            case 64: // J Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectK());
                }
                else
                {
                    StartCoroutine(DetectJ());
                }
                break;

            case 65: // K Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectL());
                }
                else
                {
                    StartCoroutine(DetectK());
                }
                break;

            case 66: // L Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    if (repeatASL == 1) { currentDialogue = 59; } else { currentDialogue = 67; }
                    speechText.text = dialogueText[currentDialogue];
                    soundManager.PlayDialogue(currentDialogue);
                }
                else
                {
                    StartCoroutine(DetectL());
                }
                break;

            case 67: // ASL List 3 Spawn
                // ADD LIST OF ASL
                rpsPic.GetComponent<MeshRenderer>().material = guideMats[8];
                rpsPic.GetComponent<MeshRenderer>().enabled = true;
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            case 69: // ASL List 3 Begin
                StartCoroutine(DetectMN());
                break;

            case 70: // M/N Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectO());
                }
                else
                {
                    StartCoroutine(DetectMN());
                }
                break;

            case 72: // O Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectP());
                }
                else
                {
                    StartCoroutine(DetectO());
                }
                break;

            case 73: // P Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectQ());
                }
                else
                {
                    StartCoroutine(DetectP());
                }
                break;

            case 74: // Q Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectR());
                }
                else
                {
                    StartCoroutine(DetectQ());
                }
                break;

            case 75: // R Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectS());
                }
                else
                {
                    StartCoroutine(DetectR());
                }
                break;

            case 76: // S Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    if (repeatASL == 2) { currentDialogue = 59; } else { currentDialogue = 77; }
                    speechText.text = dialogueText[currentDialogue];
                    soundManager.PlayDialogue(currentDialogue);
                }
                else
                {
                    StartCoroutine(DetectS());
                }
                break;

            case 77: // ASL List 4 Spawn/Begin
                // ADD LIST OF ASL
                rpsPic.GetComponent<MeshRenderer>().material = guideMats[9];
                rpsPic.GetComponent<MeshRenderer>().enabled = true;
                StartCoroutine(DetectT());
                break;

            case 78: // ASL List 4 Placeholder
                break;

            case 79: // T Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Help with U detection
                    handManager.gameObject.GetComponent<ASLphabetGestures>().SetGestureActive("r", false);
                    StartCoroutine(DetectU());
                }
                else
                {
                    StartCoroutine(DetectT());
                }
                break;

            case 80: // U Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    // Help with U detection
                    handManager.gameObject.GetComponent<ASLphabetGestures>().SetGestureActive("r", true);
                    StartCoroutine(DetectV());
                }
                else
                {
                    StartCoroutine(DetectU());
                }
                break;

            case 81: // V Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectW());
                }
                else
                {
                    StartCoroutine(DetectV());
                }
                break;

            case 82: // W Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectX());
                }
                else
                {
                    StartCoroutine(DetectW());
                }
                break;

            case 83: // X Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectY());
                }
                else
                {
                    StartCoroutine(DetectX());
                }
                break;

            case 84: // Y Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    StartCoroutine(DetectZ());
                }
                else
                {
                    StartCoroutine(DetectY());
                }
                break;

            case 85: // Z Gesture
                speechText.text = "";
                speaking = false;
                if (demoActive)
                {
                    if (repeatASL == 3) { currentDialogue = 59; } else { currentDialogue = 86; }
                    speechText.text = dialogueText[currentDialogue];
                    soundManager.PlayDialogue(currentDialogue);
                }
                else
                {
                    StartCoroutine(DetectZ());
                }
                break;

            case 88: // Final, loops to selection
                StopAllCoroutines();
                rpsPic.GetComponent<MeshRenderer>().enabled = false;
                foreach (string g in handManager.gameObject.GetComponent<SocialGestures>().GetGestureList())
                {
                    handManager.gameObject.GetComponent<SocialGestures>().SetGestureActive(g, true);
                }
                currentDialogue = 9;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;

            default:
                currentDialogue++;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
                break;
        }
    }

    /// <summary>
    /// Ends the demo and activates Free Play mode
    /// </summary>
    public void EndDemo()
    {
        demoActive = false;
        speechText.text = "...";

        // Cleans room
        foreach (GameObject g in gestureListObjs)
        {
            g.GetComponent<MeshRenderer>().enabled = false;
        }
        leftHandPic.GetComponent<MeshRenderer>().enabled = false;
        rightHandPic.GetComponent<MeshRenderer>().enabled = false;
        rpsPic.GetComponent<MeshRenderer>().enabled = false;
        gestureText.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine(ScaleOverTime(slidingWallE, 2, 0f));
        StartCoroutine(ScaleOverTime(slidingWallW, 2, 0f));
        foreach (GameObject g in aslGuides)
        {
            g.GetComponent<MeshRenderer>().enabled = true;
        }

        // Turns on all gestures
        handManager.gameObject.GetComponent<SocialGestures>().enabled = true;
        foreach (string g in handManager.gameObject.GetComponent<SocialGestures>().GetGestureList())
        {
            handManager.gameObject.GetComponent<SocialGestures>().SetGestureActive(g, true);
        }

        handManager.gameObject.GetComponent<ASLphabetGestures>().enabled = true;
        foreach (string g in handManager.gameObject.GetComponent<ASLphabetGestures>().GetGestureList())
        {
            handManager.gameObject.GetComponent<ASLphabetGestures>().SetGestureActive(g, true);
        }

        StartCoroutine(DetectWaving());
        StartCoroutine(DetectThumbsUp());
        StartCoroutine(DetectThumbsDown());
        StartCoroutine(DetectOkay());
        StartCoroutine(DetectPeace());
        StartCoroutine(DetectPinkyPromise());
        StartCoroutine(DetectWolfie());
        StartCoroutine(DetectMiddleFinger());
        StartCoroutine(DetectHangTen());
        StartCoroutine(DetectRockAndRoll());
        StartCoroutine(DetectSpiderMan());
        StartCoroutine(DetectVulcanSalute());
        StartCoroutine(DetectScoutsHonor());
        StartCoroutine(DetectFingerGun());
        StartCoroutine(DetectRock());
        StartCoroutine(DetectPaper());
        StartCoroutine(DetectScissors());

        StartCoroutine(DetectA());
        StartCoroutine(DetectB());
        StartCoroutine(DetectC());
        StartCoroutine(DetectD());
        StartCoroutine(DetectE());
        StartCoroutine(DetectF());
        StartCoroutine(DetectG());
        StartCoroutine(DetectH());
        StartCoroutine(DetectI());
        StartCoroutine(DetectJ());
        StartCoroutine(DetectK());
        StartCoroutine(DetectL());
        StartCoroutine(DetectMN());
        StartCoroutine(DetectO());
        StartCoroutine(DetectP());
        StartCoroutine(DetectQ());
        StartCoroutine(DetectR());
        StartCoroutine(DetectS());
        StartCoroutine(DetectT());
        StartCoroutine(DetectU());
        StartCoroutine(DetectV());
        StartCoroutine(DetectW());
        StartCoroutine(DetectX());
        StartCoroutine(DetectY());
        StartCoroutine(DetectZ());
    }




    /// <summary>
    /// Delays dialogue by secs seconds
    /// </summary>
    /// <param name="secs"> Number of seconds to delay dialogue </param>
    /// <param name="dialogue"> Dialogue index to play </param>
    /// <returns></returns>
    IEnumerator DelayDialogue(int secs, int dialogue)
    {
        yield return new WaitForSeconds(secs);

        currentDialogue = dialogue;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Scales wall over time seconds up to destY y
    /// </summary>
    /// <param name="wall"> Object to scale (preferably wall) </param>
    /// <param name="time"> Number of seconds </param>
    /// <param name="destY"> Desired Y value </param>
    /// <returns></returns>
    IEnumerator ScaleOverTime(GameObject wall, float time, float destY)
    {
        Vector3 originalScale = wall.transform.localScale;
        Vector3 destinationScale = new Vector3(0.5f, destY, 1f);

        float currentTime = 0.0f;

        do {
            wall.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

    }

    /// <summary>
    /// Scales obj over time seconds up to destX x, destY y, destZ z
    /// </summary>
    /// <param name="obj"> Object to scale </param>
    /// <param name="time"> Number of seconds </param>
    /// <param name="destY"> Desired Y value </param>
    /// <returns></returns>
    IEnumerator ScaleOverTime(GameObject obj, float time, float destX, float destY, float destZ)
    {
        Vector3 originalScale = obj.transform.localScale;
        Vector3 destinationScale = new Vector3(destX, destY, destZ);

        float currentTime = 0.0f;

        do
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

    }






    /// <summary>
    /// Grabs the hand that is extended and sets it as the PrimaryHand
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectPrimaryHand()
    {
        yield return new WaitUntil(() => handManager.SecondaryLocation().x > extendedValue || handManager.PrimaryLocation().x > extendedValue);

        soundManager.PlayDing();

        if (handManager.SecondaryLocation().x > extendedValue)
        {
            handManager.SetPrimaryHand(HandManager.HandType.Left);
            currentDialogue = 4;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
        }
        else
        {
            handManager.SetPrimaryHand(HandManager.HandType.Right);
            currentDialogue = 5;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
        }
    }

    /// <summary>
    /// Confirms that the chosen hand should be the PrimaryHand with a thumb's up
    /// </summary>
    /// <returns></returns>
    IEnumerator ConfirmPrimaryHand()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Thumb's Up" || handManager.GetCurrentGesture() == "Thumb's Down");

        soundManager.PlayDing();

        if (handManager.GetCurrentGesture() == "Thumb's Up")
        {
            currentDialogue = 8;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
        }
        else
        {
            if (handManager.GetPrimaryHandType() == HandManager.HandType.Right)
            {
                handManager.SetPrimaryHand(HandManager.HandType.Left);
                currentDialogue = 4;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
            }
            else
            {
                handManager.SetPrimaryHand(HandManager.HandType.Right);
                currentDialogue = 5;
                speechText.text = dialogueText[currentDialogue];
                soundManager.PlayDialogue(currentDialogue);
            }
        }
    }

    /// <summary>
    /// Confirms that the demo should go on with a thumb's up
    /// </summary>
    /// <param name="dialogue"> Dialogue index before the next dialogue </param>
    /// <returns></returns>
    IEnumerator ConfirmContinue(int dialogue)
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Thumb's Up" || handManager.GetCurrentGesture() == "Thumb's Down" || handManager.GetCurrentGesture() == "Okay");

        soundManager.PlayDing();

        if (handManager.GetCurrentGesture() == "Thumb's Up")
        {
            currentDialogue = dialogue + 1;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
        }
        else if (handManager.GetCurrentGesture() == "Thumb's Down")
        {
            EndDemo();
        } else
        {
            currentDialogue = 48;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
        }
    }

    /// <summary>
    /// Delays the lists of gestures appearing in the list portion of the demo
    /// </summary>
    /// <param name="secs"> Number of seconds to delay by </param>
    /// <returns></returns>
    IEnumerator DelayGestureLists(int secs)
    {
        yield return new WaitForSeconds(secs);

        gestureListObjs[0].GetComponent<MeshRenderer>().enabled = true;
        gestureListObjs[1].GetComponent<MeshRenderer>().enabled = true;
    }

    /// <summary>
    /// Increments the number of gestures performed during list portion and handles exit condition
    /// </summary>
    private void IncrementNumGestures()
    {
        numGestures++;

        if (numGestures == 6)
        {
            StopAllCoroutines();
            currentDialogue = 23;
            speechText.text = dialogueText[currentDialogue];
            soundManager.PlayDialogue(currentDialogue);
            numGestures = 0;
        }
    }





    /// <summary>
    /// Detects the waving gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectWaving()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Waving" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 15;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the thumb's up gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectThumbsUp()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Thumb's Up" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 16;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the thumb's down gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectThumbsDown()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Thumb's Down" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 17;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the okay gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectOkay()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Okay" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 18;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the peace gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectPeace()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Peace" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 19;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the pinky promise gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectPinkyPromise()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Pinky Promise" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 20;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the wolfie gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectWolfie()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Wolfie" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 21;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the middle finger gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectMiddleFinger()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Middle Finger" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 22;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the hang ten gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectHangTen()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Hang Ten" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 25;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the rock and roll gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectRockAndRoll()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Rock and Roll" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 26;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the spider man gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectSpiderMan()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Spider Man" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 27;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the vulcan salute gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectVulcanSalute()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Vulcan Salute" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 28;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the scout's honor gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectScoutsHonor()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Scout's Honor" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 29;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the finger gun gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectFingerGun()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Finger Gun" && !speaking);
        soundManager.PlayDing();
        speaking = true;
        currentDialogue = 30;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the rock gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectRock()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Rock" && !speaking);
        speaking = true;
        currentDialogue = 47;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the paper gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectPaper()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Paper" && !speaking);
        speaking = true;
        currentDialogue = 38;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the scissors gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectScissors()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Scissors" && !speaking);
        speaking = true;
        currentDialogue = 46;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }



    /// <summary>
    /// Detects the A gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectA()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "A" && !speaking);
        speaking = true;
        currentDialogue = 53;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the B gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectB()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "B" && !speaking);
        speaking = true;
        currentDialogue = 54;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the C gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectC()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "C" && !speaking);
        speaking = true;
        currentDialogue = 55;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the D gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectD()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "D" && !speaking);
        speaking = true;
        currentDialogue = 56;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the E gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectE()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "E" && !speaking);
        speaking = true;
        currentDialogue = 57;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the F gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectF()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "F" && !speaking);
        speaking = true;
        currentDialogue = 58;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the G gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectG()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "G" && !speaking);
        speaking = true;
        currentDialogue = 61;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the H gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectH()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "H" && !speaking);
        speaking = true;
        currentDialogue = 62;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the I gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectI()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "I" && !speaking);
        speaking = true;
        currentDialogue = 63;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the J gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectJ()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "J" && !speaking);
        speaking = true;
        currentDialogue = 64;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the K gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectK()
    {
        yield return new WaitUntil(() => (handManager.GetCurrentGesture() == "K" || handManager.GetCurrentGesture() == "K Alt") && !speaking);
        speaking = true;
        currentDialogue = 65;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the L gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectL()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "L" && !speaking);
        speaking = true;
        currentDialogue = 66;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the M/N gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectMN()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "M/N" && !speaking);
        speaking = true;
        currentDialogue = 70;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the O gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectO()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "O" && !speaking);
        speaking = true;
        currentDialogue = 72;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the P gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectP()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "P" && !speaking);
        speaking = true;
        currentDialogue = 73;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the Q gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectQ()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Q" && !speaking);
        speaking = true;
        currentDialogue = 74;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the R gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectR()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "R" && !speaking);
        speaking = true;
        currentDialogue = 75;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the S gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectS()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "S" && !speaking);
        speaking = true;
        currentDialogue = 76;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the T gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectT()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "T" && !speaking);
        speaking = true;
        currentDialogue = 79;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the U gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectU()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "U" && !speaking);
        speaking = true;
        currentDialogue = 80;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the V gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectV()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "V" && !speaking);
        speaking = true;
        currentDialogue = 81;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the W gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectW()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "W" && !speaking);
        speaking = true;
        currentDialogue = 82;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the X gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectX()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "X" && !speaking);
        speaking = true;
        currentDialogue = 83;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the Y gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectY()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Y" && !speaking);
        speaking = true;
        currentDialogue = 84;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the Z gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectZ()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Z_Static" && !speaking);
        speaking = true;
        currentDialogue = 85;
        speechText.text = dialogueText[currentDialogue];
        soundManager.PlayDialogue(currentDialogue);
    }

    /// <summary>
    /// Detects the snap gesture
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectSnap()
    {
        yield return new WaitUntil(() => handManager.GetCurrentGesture() == "Snap" && !speaking);
        soundManager.PlayDing();
        rpsPic.GetComponent<MeshRenderer>().enabled = true;
        //speaking = true;
        //currentDialogue = 71;
        //speechText.text = dialogueText[currentDialogue];
        //soundManager.PlayDialogue(currentDialogue);
    }
}
