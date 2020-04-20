using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Gesture library for frequently used social/fun gestures
/// <br></br><b>Add to scene within a single GameObject and initialize manager in editor to use</b>
/// </summary>
public class SocialGestures : GestureLibrary
{
    // The HandManager
    public HandManager manager;

    // List of activated gestures
    [Header("Active Gestures")]
    public bool thumbsUp = true;
    public bool thumbsDown = true;
    public bool paper = true;
    public bool rock = true;
    public bool scissors = true;
    public bool okay = true;
    public bool spiderMan = true;
    public bool hangTen = true;
    public bool telephone = true;
    public bool middleFinger = true;
    public bool vulcanSalute = true;
    public bool peace = true;
    public bool scoutsHonor = true;
    public bool pinkyPromise = true;
    public bool rockAndRoll = true;
    public bool fingerGun = true;
    public bool waving = true;
    public bool wolfie = true;
    public bool snap = true;

    // Accessible list of all gesture names for this library
    private List<string> gestureList = new List<string>();

    // Initializes gestureList
    void Start()
    {
        foreach (FieldInfo f in GetType().GetFields())
        {
            if (f.FieldType == typeof(bool))
            {
                gestureList.Add(f.Name);
            }
        }
    }

    /// <summary>
    /// Gets the list of gesture names so the library can have gestures turned on/off through SetGestureActive()
    /// </summary>
    /// <returns> String list of all gesture names in this library </returns>
    public List<string> GetGestureList()
    {
        return gestureList;
    }

    /// <summary>
    /// Sets a given gesture to active or inactive
    /// </summary>
    /// <param name="var"> Gesture variable name </param>
    /// <param name="isActive"> True if gesture should be active, false otherwise </param>
    public void SetGestureActive(string var, bool isActive)
    {
        GetType().GetField(var).SetValue(this, isActive);
    }



    /// <summary>
    /// Handles gesture detection per update
    /// </summary>
    void LateUpdate()
    {
        int[] handShape = manager.PrimaryHandShape();
        HandManager.Orientation[] orient = manager.PrimaryOrientation();

        // Thumb's Up
        if (thumbsUp &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[4] == HandManager.Orientation.Thumb_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) == 0
            )
        {
            manager.SetCurrentGesture("Thumb's Up");
            return;
        }

        // Thumb's Down
        if (thumbsDown &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] >= (int)Finger.FingerShape.Bent &&
            handShape[2] >= (int)Finger.FingerShape.Bent &&
            handShape[3] >= (int)Finger.FingerShape.Bent &&
            handShape[4] >= (int)Finger.FingerShape.Bent &&
            orient[4] == HandManager.Orientation.Thumb_Down &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) == 0
            )
        {
            manager.SetCurrentGesture("Thumb's Down");
            return;
        }

        // Paper
        if (paper &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] <= (int)Finger.FingerShape.Curved &&
            handShape[3] <= (int)Finger.FingerShape.Curved &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[2] == HandManager.Orientation.Palm_Down &&
            orient[3] == HandManager.Orientation.Knuckles_Mid
            )
        {
            manager.SetCurrentGesture("Paper");
            return;
        }

        // Rock
        if (rock &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[3] != HandManager.Orientation.Knuckles_In &&
            orient[4] == HandManager.Orientation.Thumb_Up
            )
        {
            manager.SetCurrentGesture("Rock");
            return;
        }

        // Scissors
        if (scissors &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] <= (int)Finger.FingerShape.Curved &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[4] == HandManager.Orientation.Thumb_Up &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle)
            )
        {
            manager.SetCurrentGesture("Scissors");
            return;
        }

        // Okay
        if (okay &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] <= (int)Finger.FingerShape.Curved &&
            handShape[3] <= (int)Finger.FingerShape.Curved &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) == 1
            )
        {
            manager.SetCurrentGesture("Okay");
            return;
        }

        // Spider Man
        if (spiderMan &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            orient[2] == HandManager.Orientation.Palm_Up &&
            orient[3] == HandManager.Orientation.Knuckles_Mid
            )
        {
            manager.SetCurrentGesture("Spider Man");
            return;
        }

        // Hang Ten
        if (hangTen &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Hang Ten");
            return;
        }

        // Telephone
        if (telephone &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture("Telephone");
            return;
        }

        // Middle Finger
        if (middleFinger &&
            handShape[1] >= (int)Finger.FingerShape.Bent &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[0] == HandManager.Orientation.Palm_Back &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) > 0
            )
        {
            manager.SetCurrentGesture("Middle Finger");
            return;
        }

        // Vulcan Salute
        if (vulcanSalute &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] == (int)Finger.FingerShape.Extended &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Vulcan Salute");
            return;
        }

        // Peace
        if (peace &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            (orient[0] == HandManager.Orientation.Palm_Back || orient[0] == HandManager.Orientation.Palm_Front) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Peace");
            return;
        }

        // Scout's Honor
        if (scoutsHonor &&
            handShape[0] >= (int)Finger.FingerShape.Folded &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] == (int)Finger.FingerShape.Extended &&
            (handShape[4] == (int)Finger.FingerShape.Bent || handShape[4] == (int)Finger.FingerShape.Folded) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            (orient[0] == HandManager.Orientation.Palm_Back || orient[0] == HandManager.Orientation.Palm_Front) &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Pinky) >= 0.75
            )
        {
            manager.SetCurrentGesture("Scout's Honor");
            return;
        }

        // Pinky Promise
        if (pinkyPromise &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] >= (int)Finger.FingerShape.Bent &&
            handShape[2] >= (int)Finger.FingerShape.Bent &&
            handShape[3] >= (int)Finger.FingerShape.Bent &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            (orient[0] == HandManager.Orientation.Palm_Back || orient[0] == HandManager.Orientation.Palm_Front) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Pinky Promise");
            return;
        }

        // Rock and Roll
        if (rockAndRoll &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            (orient[0] == HandManager.Orientation.Palm_Back || orient[0] == HandManager.Orientation.Palm_Front) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Rock and Roll");
            return;
        }

        // Finger Gun START
        if (fingerGun &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot(null, "Finger Gun Temp", false, 0.5f));
            return;
        }

        // Finger Gun
        if (fingerGun &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[3] == HandManager.Orientation.Knuckles_Mid
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("Finger Gun Temp", "Finger Gun", true, 0.5f));
            return;
        }

        // Waving START
        if (waving &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended }, 1, 0.8f) &&
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            /// Allows you to return to the original snapshot and keep holding the gesture
            if (manager.AddSnapshot("Waving", "Waving", true, .25f) != null)
            {
                manager.SetCurrentGesture(manager.AddSnapshot("Waving", "Waving", true, .25f));
            }
            else
            {
                manager.SetCurrentGesture(manager.AddSnapshot(null, "Waving Temp", false, .25f));
            }

            return;
        }

        // Waving
        if (waving &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended }, 1, 0.8f) &&
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("Waving Temp", "Waving", true, .25f));
            return;
        }
        
        // Wolfie
        if (wolfie &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Middle) >= 0.5 &&
            manager.PrimaryFingerPinch(Finger.FingerType.Ring) >= 0.5
            )
        {
            manager.SetCurrentGesture("Wolfie");
            return;
        }

        // Snap START
        if (snap &&
            handShape[2] != (int)Finger.FingerShape.Inward &&
            manager.PrimaryFingerPinch(Finger.FingerType.Middle) == 1
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot(null, "Snap Temp", false, 0.5f));
            return;
        }

        // Snap [MUST BE LAST IN LIST OF GESTURES DUE TO HIGH ABILITY TO TRIGGER]
        if (snap &&
            handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] <= (int)Finger.FingerShape.Curved &&
            handShape[2] == (int)Finger.FingerShape.Inward
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("Snap Temp", "Snap", true, 0.2f));
            return;
        }

        manager.SetCurrentGesture(null);
    }
}
