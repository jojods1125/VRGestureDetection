using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Gesture library for alphabet ASL signs
/// <br></br><b>Add to scene within a single GameObject and initialize manager in editor to use</b>
/// </summary>
public class ASLphabetGestures : GestureLibrary
{
    // The HandManager
    public HandManager manager;

    // List of activated gestures
    [Header("Active Gestures")]
    public bool a = true;
    public bool b = true;
    public bool c = true;
    public bool d = true;
    public bool e = true;
    public bool f = true;
    public bool g = true;
    public bool h = true;
    public bool i = true;
    public bool j = true;
    public bool k = true;
    public bool l = true;
    public bool m = true;
    public bool n = true;
    public bool o = true;
    public bool p = true;
    public bool q = true;
    public bool r = true;
    public bool s = true;
    public bool t = true;
    public bool u = true;
    public bool v = true;
    public bool w = true;
    public bool x = true;
    public bool y = true;
    public bool z = true;

    private Vector3 zStart;

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

        // ASL - A
        if (a &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("A");
            return;
        }

        // ASL - B
        if (b &&
            handShape[0] >= (int)Finger.FingerShape.Bent &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] == (int)Finger.FingerShape.Extended &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("B");
            return;
        }

        // ASL - C
        if (c &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Curved &&
            handShape[2] == (int)Finger.FingerShape.Curved &&
            handShape[3] == (int)Finger.FingerShape.Curved &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            orient[4] != HandManager.Orientation.Thumb_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) <= 0.25
            )
        {
            manager.SetCurrentGesture("C");
            return;
        }

        // ASL - D
        if (d &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Curved &&
            handShape[4] >= (int)Finger.FingerShape.Curved &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Middle) == 1
            )
        {
            manager.SetCurrentGesture("D");
            return;
        }

        // ASL - E
        if (e &&
            handShape[0] != (int)Finger.FingerShape.Extended &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Bent, Finger.FingerShape.Bent, Finger.FingerShape.Bent, Finger.FingerShape.Bent }, 1, 0.25f) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("E");
            return;
        }

        // ASL - F
        if (f &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] == (int)Finger.FingerShape.Extended &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) == 1
            )
        {
            manager.SetCurrentGesture("F");
            return;
        }

        // ASL - G
        if (g &&
            handShape[0] <= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle) &&
            orient[0] == HandManager.Orientation.Palm_Back &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture("G");
            return;
        }

        // ASL - H
        if (h &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Ring) &&
            orient[0] == HandManager.Orientation.Palm_Back &&
            orient[3] == HandManager.Orientation.Knuckles_In
            )
        {
            manager.SetCurrentGesture("H");
            return;
        }

        // ASL - I / J_Start
        if ((i || j) &&
            handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            if (j)
            {
                manager.AddSnapshot(null, "J_Start", false, 2);
            }
            if (i)
            {
                manager.SetCurrentGesture("I");
            }
            if (!i)
            {
                manager.SetCurrentGesture(null);
            }
            return;
        }

        // ASL - J
        if (j &&
            handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] == (int)Finger.FingerShape.Extended &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            orient[0] == HandManager.Orientation.Palm_Back &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture(manager.AddSnapshot("J_Start", "J", true, 2));
            return;
        }

        // ASL - K
        if (k &&
            handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            (manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) ||
             manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle)) &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("K");
            return;
        }

        // ASL - L
        if (l &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("L");
            return;
        }

        // ASL - M/N
        if ((m || n) &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Inward, Finger.FingerShape.Inward, Finger.FingerShape.Inward, 0 }, 2, 0.66f) &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("M/N");
            return;
        }

        //// ASL - N
        //if (n &&
        //    handShape[0] >= (int)Finger.FingerShape.Curved &&

        //    manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Folded, Finger.FingerShape.Folded, 0, 0 }, 2, 0.5f) &&

        //    //handShape[1] == (int)Finger.FingerShape.Curved &&
        //    //handShape[2] == (int)Finger.FingerShape.Curved &&
        //    handShape[3] >= (int)Finger.FingerShape.Folded &&
        //    handShape[4] >= (int)Finger.FingerShape.Folded &&
        //    //manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) &&
        //    manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
        //    //manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
        //    //manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
        //    //orient[0] == HandManager.Orientation.Palm_Front &&
        //    //orient[1] != HandManager.Orientation.Palm_Out &&
        //    //orient[2] == HandManager.Orientation.Palm_Up &&
        //    orient[3] == HandManager.Orientation.Knuckles_Up
        //    //orient[4] == HandManager.Orientation.Thumb_Down &&
        //    //manager.PrimaryFingerPinch(Finger.FingerType.Middle) == 1
        //    )
        //{
        //    manager.SetCurrentGesture("N");
        //    return;
        //}

        // ASL - O
        if (o &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Curved, Finger.FingerShape.Curved, Finger.FingerShape.Curved, Finger.FingerShape.Curved }, 2, 0.5f) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[4] != HandManager.Orientation.Thumb_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) >= 0.5f
            )
        {
            manager.SetCurrentGesture("O");
            return;
        }

        // ASL - P
        if (p &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Folded, Finger.FingerShape.Folded }, 2, 0.25f) &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            orient[2] == HandManager.Orientation.Palm_Down
            )
        {
            manager.SetCurrentGesture("P");
            return;
        }

        // ASL - Q
        if (q &&
            //handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            //handShape[2] >= (int)Finger.FingerShape.Folded &&
            //handShape[3] >= (int)Finger.FingerShape.Folded &&
            //handShape[4] >= (int)Finger.FingerShape.Folded &&
            //(manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index) ||
            // manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle)) &&
            //!manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            //manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            //manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            //orient[0] == HandManager.Orientation.Palm_Front &&
            //orient[1] != HandManager.Orientation.Palm_Out &&
            orient[2] == HandManager.Orientation.Palm_Down &&
            (orient[3] == HandManager.Orientation.Knuckles_Mid || orient[3] == HandManager.Orientation.Knuckles_In) &&
            //orient[4] == HandManager.Orientation.Thumb_Down &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) >= 0.1
            )
        {
            manager.SetCurrentGesture("Q");
            return;
        }

        // ASL - R
        if (r &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            //manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            //orient[0] == HandManager.Orientation.Palm_Front &&
            //orient[1] != HandManager.Orientation.Palm_Out &&
            //orient[2] == HandManager.Orientation.Palm_Up &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            //orient[4] == HandManager.Orientation.Thumb_Down &&
            //manager.PrimaryFingerPinch(Finger.FingerType.Middle) == 1
            )
        {
            manager.SetCurrentGesture("R");
            return;
        }

        // ASL - S
        if (s &&

            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Folded, Finger.FingerShape.Folded, Finger.FingerShape.Folded, Finger.FingerShape.Folded }, 1, 0.75f) &&

            handShape[0] >= (int)Finger.FingerShape.Curved &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("S");
            return;
        }

        // ASL - T
        if (t &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Curved, Finger.FingerShape.Folded, Finger.FingerShape.Folded, 0 }, 2, 0.66f) &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring) &&
            orient[3] == HandManager.Orientation.Knuckles_Up &&
            manager.PrimaryFingerPinch(Finger.FingerType.Index) >= 0.75
            )
        {
            manager.SetCurrentGesture("T");
            return;
        }

        // ASL - U  [OVERRIDDEN BY R]
        if (u &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Bent &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Ring) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("U");
            return;
        }

        // ASL - V
        if (v &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Bent &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Ring) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("V");
            return;
        }

        // ASL - W
        if (w &&
            handShape[0] >= (int)Finger.FingerShape.Bent &&
            manager.PrimaryTolerantHandShape(new Finger.FingerShape[5] { 0, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Extended, Finger.FingerShape.Bent }, 2, 0.6f) &&
            (!manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) ||
            !manager.PrimaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring)) &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("W");
            return;
        }

        // ASL - X
        if (x &&
            handShape[0] >= (int)Finger.FingerShape.Bent &&
            (handShape[1] == (int)Finger.FingerShape.Curved || handShape[1] == (int)Finger.FingerShape.Bent) &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            manager.PrimaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Middle) &&
            orient[1] == HandManager.Orientation.Palm_In &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("X");
            return;
        }

        // ASL - Y
        if (y &&
            handShape[0] == (int)Finger.FingerShape.Extended &&
            handShape[1] >= (int)Finger.FingerShape.Folded &&
            handShape[2] >= (int)Finger.FingerShape.Folded &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] <= (int)Finger.FingerShape.Curved &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("Y");
            return;
        }

        // ASL - Z (Static - Ignores Movement)
        if (z &&
            handShape[0] >= (int)Finger.FingerShape.Curved &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] >= (int)Finger.FingerShape.Bent &&
            handShape[3] >= (int)Finger.FingerShape.Bent &&
            handShape[4] >= (int)Finger.FingerShape.Bent &&
            orient[3] == HandManager.Orientation.Knuckles_Mid
            //!manager.GetSnapshotTimes().ContainsKey("Z_Start")
            )
        {
            //zStart = manager.PrimaryLocation();
            //manager.AddSnapshot(null, "Z_Start", false, 3);
            //manager.SetCurrentGesture("Z_Start");
            manager.SetCurrentGesture("Z_Static");
            return;
        }

        // ASL - Z
        //if (z &&
        //    handShape[0] >= (int)Finger.FingerShape.Curved &&
        //    handShape[1] == (int)Finger.FingerShape.Extended &&
        //    handShape[2] >= (int)Finger.FingerShape.Bent &&
        //    handShape[3] >= (int)Finger.FingerShape.Bent &&
        //    handShape[4] >= (int)Finger.FingerShape.Bent &&
        //    orient[3] == HandManager.Orientation.Knuckles_Mid &&
        //    manager.PrimaryLocation().x > zStart.x &&
        //    manager.PrimaryLocation().y > zStart.z
        //    )
        //{
        //    manager.AddSnapshot("Z_Start", "Z", true, 3);
        //    manager.SetCurrentGesture("Z (Ignores movement)");
        //    return;
        //}


        // ASL - K (ALTERNATIVE)
        if (k &&
            handShape[0] <= (int)Finger.FingerShape.Bent &&
            handShape[1] == (int)Finger.FingerShape.Extended &&
            handShape[2] == (int)Finger.FingerShape.Extended &&
            handShape[3] >= (int)Finger.FingerShape.Folded &&
            handShape[4] >= (int)Finger.FingerShape.Folded &&
            !manager.PrimaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle) &&
            orient[0] == HandManager.Orientation.Palm_Front &&
            orient[3] == HandManager.Orientation.Knuckles_Up
            )
        {
            manager.SetCurrentGesture("K Alt");
            return;
        }

        manager.SetCurrentGesture(null);
    }
}
