using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Gesture library for non-alphabet ASL signs
/// <br></br><b>Add to scene within a single GameObject and initialize manager in editor to use</b>
/// </summary>
public class ASLGestures : GestureLibrary
{
    // The HandManager
    public HandManager manager;

    // List of activated gestures
    [Header("Active Gestures")]
    public bool iLoveYou = true;

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

        // ASL - I Love You
        if (iLoveYou &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("I Love You");
            return;
        }

        manager.SetCurrentGesture(null);
    }
}
