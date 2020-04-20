using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Example gesture library for developers to duplicate and edit
/// <br></br><b>Add to scene within a single GameObject and initialize manager in editor to use</b>
/// </summary>
public class ExampleGestures : GestureLibrary
{
    // The HandManager
    public HandManager manager;

    // List of activated gestures
    [Header("Active Gestures")]
    public bool exampleGesture = true;
    public bool exampleMultiPartGesture = true;

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

        /// EXAMPLE OF HOW TO USE SetGestureActive() AND GetGestureList() TO DEACTIVATE GESTURES FROM A GAMEMANAGER
        /// ASSUMES ExampleGestures IS IN THE SAME GAMEOBJECT AS HandManager
        // foreach (string g in handManager.gameObject.GetComponent<ExampleGestures>().GetGestureList())
        // {
        //    if (g != "exampleGesture")
        //    {
        //        handManager.gameObject.GetComponent<ExampleGestures>().SetGestureActive(g, false);
        //    }
        // }
    }



    /// <summary>
    /// Handles gesture detection per update
    /// </summary>
    void LateUpdate()
    {
        // Grabs the hand shape of the primary hand [READ DESCRIPTION]
        int[] handShape = manager.PrimaryHandShape();

        // Grabs the orientation of the primary hand [READ DESCRIPTION]
        HandManager.Orientation[] orient = manager.PrimaryOrientation();


        // Example Gesture
        /// [Include/exclude as many parameters as needed to get the desired gesture]
        if (
            // Name of the gesture
            exampleGesture &&

            // Shape of the fingers on the hand, where 0 is the thumb and 4 is the pinky
            /// Use >, <, >=, <=, !, and || with values between Extended and Inward to incorporate more values
            handShape[0] == (int)Finger.FingerShape.Extended && // Straight out
            handShape[1] == (int)Finger.FingerShape.Curved &&   // Slightly curved
            handShape[2] == (int)Finger.FingerShape.Bent &&     // Fingertip behind knuckle
            handShape[3] == (int)Finger.FingerShape.Folded &&   // Fingertip pressed into palm
            handShape[4] == (int)Finger.FingerShape.Inward &&   // Fingertip pointed down toward wrist

            // Substitution for the above, requiring exact values for FingerShapes
            manager.PrimaryExactHandShape(new Finger.FingerShape[5] { Finger.FingerShape.Extended, Finger.FingerShape.Curved, Finger.FingerShape.Bent, Finger.FingerShape.Folded, Finger.FingerShape.Inward }) &&

            // Substitution for the above, allowing for tolerance with FingerShapes
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { Finger.FingerShape.Extended, Finger.FingerShape.Curved, Finger.FingerShape.Bent, Finger.FingerShape.Folded, Finger.FingerShape.Inward }, 1, 0.6f) &&

            // Orientation of the hand using five values from three axes (Palm[0..2], Knuckles[3], Thumb[4])
            /// Read the descriptions of PrimaryOrientation(), SecondaryOrientation() for value descriptions
            /// Use >, <, >=, <=, !, and || to incorporate more values
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[1] == HandManager.Orientation.Palm_Out &&
            orient[2] == HandManager.Orientation.Palm_Up &&
            orient[3] == HandManager.Orientation.Knuckles_In &&
            orient[4] == HandManager.Orientation.Thumb_Down &&

            // Passed fingertip is pinching thumb
            /// Use >, <, >=, and <= with values between 0 and 1 to specify pinch strength
            manager.PrimaryFingerPinch(Finger.FingerType.Index) == 1 &&

            // Passed fingers are touching each other
            /// Use ! for checking if fingers are apart
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle)

            /// The final parameter must not have && at the end of it
            )
        {
            // Tell the manager that the gesture has been detected
            manager.SetCurrentGesture("Example Gesture");
            return;
        }


        // Example Multi-Part Gesture START
        if (exampleMultiPartGesture &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot(null, "Multi-Part START", false, 1f));
            return;
        }

        // Example Multi-Part Gesture MIDDLE
        if (exampleMultiPartGesture &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("Multi-Part START", "Multi-Part MIDDLE", false, 1f));
            return;
        }

        // Example Multi-Part Gesture END
        if (exampleMultiPartGesture &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("Multi-Part MIDDLE", "Multi-Part END", true, 1f));
            return;
        }

        // If no gesture is found, you MUST set CurrentGesture to null
        manager.SetCurrentGesture(null);
    }
}
