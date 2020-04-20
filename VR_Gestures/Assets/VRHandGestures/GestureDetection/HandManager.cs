using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all additional behavior for VR hand-tracking
/// <br></br><b>Add to scene within a single GameObject and initialize centerEyeAnchor, leftOVRHandPrefab, and rightOVRHandPrefab in editor to use</b>
/// </summary>
public class HandManager : MonoBehaviour
{
    /// <summary> The player's camera center; CenterEyeAnchor in OVRCameraRig </summary>
    public GameObject centerEyeAnchor;

    /// <summary> The player's left hand; OVRHandPrefab under LeftHandAnchor in OVRCameraRig </summary>
    public OVRHand leftOVRHandPrefab;

    /// <summary> The player's right hand; OVRHandPrefab under RightHandAnchor in OVRCameraRig </summary>
    public OVRHand rightOVRHandPrefab;

    /// <summary> Left hand skeleton used to access bone information </summary>
    private OVRSkeleton leftSkele;

    /// <summary> Right hand skeleton used to access bone information </summary>
    private OVRSkeleton rightSkele;

    /// <summary> Primary hand, set with SetPrimaryHand(); automatically set to right on start() </summary>
    private OVRHand primaryHand;

    /// <summary> Secondary hand, set with SetPrimaryHand(); automatically set to left on start() </summary>
    private OVRHand secondaryHand;

    /// <summary> Primary hand skeleton used to access bone information </summary>
    private OVRSkeleton primarySkele;

    /// <summary> Secondary hand skeleton used to access bone information </summary>
    private OVRSkeleton secondarySkele;

    // Primary fingers
    private Finger primaryThumb;
    private Finger primaryIndex;
    private Finger primaryMiddle;
    private Finger primaryRing;
    private Finger primaryPinky;

    // Secondary fingers
    private Finger secondaryThumb;
    private Finger secondaryIndex;
    private Finger secondaryMiddle;
    private Finger secondaryRing;
    private Finger secondaryPinky;

    /// <summary> Current gesture being detected by a GestureLibrary </summary>
    private string currentGesture;

    /// <summary> Number of gesture libraries in the project </summary>
    private int libraryCount = 0;

    /// <summary> Number of libraries reporting no gesture on update() </summary>
    private int emptyCount = 0;

    /// <summary> Number of libraries reporting a gesture on update() </summary>
    private int trackedCount = 0;

    /// <summary> List of existing snapshot gesture names </summary>
    private string[] snapshots;

    /// <summary> Dictionary of multi-part gesture snapshots and the time remaining for the next snapshot </summary>
    private Dictionary<string, float> snapshotTimes = new Dictionary<string, float>();


    /// <summary>
    /// Enum for hand orientation. Put into array form in PrimaryOrientation() and SecondaryOrientation()
    /// </summary>
    public enum Orientation
    {
        /// <summary> Palm facing the player </summary>
        Palm_Back,
        /// <summary> Palm facing between the player and away from the player </summary>
        Palm_Mid_Fr,
        /// <summary> Palm facing away from the player </summary>
        Palm_Front,


        /// <summary> Palm facing to the outside (right on right hand, left on left hand) </summary>
        Palm_Out,
        /// <summary> Palm facing between the outside and inside </summary>
        Palm_Mid_In,
        /// <summary> Palm facing to the inside (left on right hand, right on left hand) </summary>
        Palm_In,


        /// <summary> Palm facing upward </summary>
        Palm_Up,
        /// <summary> Palm facing between upward and downward </summary>
        Palm_Mid_Dw,
        /// <summary> Palm facing downward </summary>
        Palm_Down,


        /// <summary> Knuckles facing downward </summary>
        Knuckles_Down,
        /// <summary> Knuckles facing between upward and downward </summary>
        Knuckles_Mid,
        /// <summary> Knuckles facing upward </summary>
        Knuckles_Up,

        /// <summary> Knuckles facing to the inside (left on right hand, right on left hand) </summary>
        Knuckles_In,

        /// <summary> Thumb facing downward </summary>
        Thumb_Down,
        /// <summary> Thumb facing between upward and downward </summary>
        Thumb_Mid,
        /// <summary> Thumb facing upward </summary>
        Thumb_Up,

        /// <summary> Knuckles facing to the inside (left on right hand, right on left hand) </summary>
        Thumb_In,

        /// <summary> Error condition </summary>
        Invalid
    }


    /// <summary>
    /// Enum for hand type. Used in SetPrimaryHand() to set the primary and secondary hands.
    /// </summary>
    public enum HandType
    {
        Left,
        Right,
    }


    void Start()
    {
        SetPrimaryHand(HandType.Right); // DEFAULT VALUE: primaryHand is set to right
                                        // It is recommended to incorporate your own check to set this value
    }

    void Update()
    {
        libraryCount = 0;
        foreach (GestureLibrary lib in FindObjectsOfType<GestureLibrary>())
        {
            if (lib.enabled)
            {
                libraryCount++;
            }
        }
        trackedCount = 0;
        emptyCount = 0;
        DecrementSnapshots();
    }


    /// <summary>
    /// Sets the PrimaryHand to "hand"'s corresponding OVRHand, and the SecondaryHand to the other type's OVRHand.
    /// <br></br> Primary is right and secondary is left by default.
    /// </summary>
    /// 
    /// <param name="hand"> The type of hand being set to primary </param>
    public void SetPrimaryHand(HandType hand)
    {
        // Set the primary and secondary skeletons/hands
        if (hand.Equals(HandType.Left))
        {
            primaryHand = leftOVRHandPrefab;
            primarySkele = leftOVRHandPrefab.GetComponent<OVRSkeleton>();
            secondaryHand = rightOVRHandPrefab;
            secondarySkele = rightOVRHandPrefab.GetComponent<OVRSkeleton>();
            leftSkele = primarySkele;
            rightSkele = secondarySkele;
        }
        else
        {
            primaryHand = rightOVRHandPrefab;
            primarySkele = rightOVRHandPrefab.GetComponent<OVRSkeleton>();
            secondaryHand = leftOVRHandPrefab;
            secondarySkele = leftOVRHandPrefab.GetComponent<OVRSkeleton>();
            rightSkele = primarySkele;
            leftSkele = secondarySkele;
        }

        // Set the primary fingers
        primaryThumb = new Finger(Finger.FingerType.Thumb, primarySkele);
        primaryIndex = new Finger(Finger.FingerType.Index, primarySkele);
        primaryMiddle = new Finger(Finger.FingerType.Middle, primarySkele);
        primaryRing = new Finger(Finger.FingerType.Ring, primarySkele);
        primaryPinky = new Finger(Finger.FingerType.Pinky, primarySkele);

        // Set the secondary fingers
        secondaryThumb = new Finger(Finger.FingerType.Thumb, secondarySkele);
        secondaryIndex = new Finger(Finger.FingerType.Index, secondarySkele);
        secondaryMiddle = new Finger(Finger.FingerType.Middle, secondarySkele);
        secondaryRing = new Finger(Finger.FingerType.Ring, secondarySkele);
        secondaryPinky = new Finger(Finger.FingerType.Pinky, secondarySkele);
    }


    /// <summary>
    /// Gets the primary hand OVRHand
    /// </summary>
    /// 
    /// <returns> The primary hand as set by the program; defaults to the right hand </returns>
    public OVRHand GetPrimaryHand()
    {
        return primaryHand;
    }

    /// <summary>
    /// Gets the secondary hand OVRHand
    /// </summary>
    /// 
    /// <returns> The secondary hand as set by the program; defaults to the left hand </returns>
    public OVRHand GetSecondaryHand()
    {
        return secondaryHand;
    }

    /// <summary>
    /// Gets the primary hand's HandType: Right or Left
    /// </summary>
    /// 
    /// <returns> HandType.Left if left hand, HandType.Right if right hand </returns>
    public HandType GetPrimaryHandType()
    {
        if (primaryHand.Equals(rightOVRHandPrefab))
        {
            return HandType.Right;
        }

        return HandType.Left;
    }

    /// <summary>
    /// Gets the secondary hand's HandType: Right or Left
    /// </summary>
    /// 
    /// <returns> HandType.Left if left hand, HandType.Right if right hand </returns>
    public HandType GetSecondaryHandType()
    {
        if (primaryHand.Equals(rightOVRHandPrefab))
        {
            return HandType.Right;
        }

        return HandType.Left;
    }

    /// <summary>
    /// Gets the primary hand OVRSkeleton
    /// </summary>
    /// 
    /// <returns> The primary hand skeleton as set by the program; defaults to the right hand </returns>
    public OVRSkeleton GetPrimarySkele()
    {
        return primarySkele;
    }

    /// <summary>
    /// Gets the secondary hand OVRSkeleton
    /// </summary>
    /// 
    /// <returns> The secondary hand skeleton as set by the program; defaults to the left hand </returns>
    public OVRSkeleton GetSecondarySkele()
    {
        return secondarySkele;
    }

    /// <summary>
    /// Gets the left hand OVRSkeleton
    /// </summary>
    /// 
    /// <returns> The left hand skeleton </returns>
    public OVRSkeleton GetLeftSkele()
    {
        return leftSkele;
    }

    /// <summary>
    /// Gets the right hand OVRSkeleton
    /// </summary>
    /// 
    /// <returns> The right hand skeleton </returns>
    public OVRSkeleton GetRightSkele()
    {
        return rightSkele;
    }

    /// <summary>
    /// Gets the current gesture being detected
    /// </summary>
    /// 
    /// <returns> The current gesture written as a string </returns>
    public string GetCurrentGesture()
    {
        return currentGesture;
    }

    /// <summary>
    /// Gets the snapshot gesture names
    /// </summary>
    /// 
    /// <returns> Array of strings representing snapshot gestures </returns>
    public string[] GetSnapshots()
    {
        return snapshots;
    }

    /// <summary>
    /// Gets a dictionary of snapshot gestures to seconds remaining for each
    /// </summary>
    /// 
    /// <returns> Dictionary of gestures as strings to time as float </returns>
    public Dictionary<string, float> GetSnapshotTimes()
    {
        return snapshotTimes;
    }

    /// <summary>
    /// Sets the current gesture being performed
    /// </summary>
    /// 
    /// <param name="newGesture"> The gesture being detected </param>
    public void SetCurrentGesture(string newGesture)
    {
        if (newGesture != null)
        {
            //Debug.Log(newGesture);
            currentGesture = newGesture;
            trackedCount++;
        }
        else
        {
            emptyCount++;
            if (emptyCount == libraryCount)
            {
                currentGesture = "None";
                //Debug.LogWarning("emptyCount: " + emptyCount + "   libraryCount: " + libraryCount);
            } else
            {
                if (emptyCount + trackedCount > libraryCount)
                {
                    currentGesture = "Number of libraries invalid";
                    //Debug.LogWarning("emptyCount: " + emptyCount + "   libraryCount: " + libraryCount);
                }
            }
        }
    }

    /// <summary>
    /// Gets the name of a passed bone in the primary hand
    /// </summary>
    /// 
    /// <param name="boneId"> The numeric bone ID, using Oculus' bone data </param>
    /// 
    /// <returns> The bone name as a string </returns>
    public string PrimaryBoneName(int boneId)
    {
        if (boneId > -1 && boneId < 24)
        {
            return primarySkele.Bones[boneId].Id.ToString();
        }
        else
        {
            return "INVALID ID";
        }
    }

    /// <summary>
    /// Gets the name of a passed bone in the secondary hand
    /// </summary>
    /// 
    /// <param name="boneId"> The numeric bone ID, using Oculus' bone data </param>
    /// 
    /// <returns> The bone name as a string </returns>
    public string SecondaryBoneName(int boneId)
    {
        if (boneId > -1 && boneId < 24)
        {
            return secondarySkele.Bones[boneId].Id.ToString();
        }
        else
        {
            return "INVALID ID";
        }
    }

    /// <summary>
    /// Finds the location of the primary hand relative to the center of the player's eyesight
    /// <br></br>Positive vector is (forward, right, up)
    /// </summary>
    /// 
    /// <returns> Hand location as a vector in the format (x, y, z), with origin roughly in the center of the player's eyesight </returns>
    public Vector3 PrimaryLocation()
    {
        Vector3 distance = centerEyeAnchor.transform.position - primaryHand.transform.position;
        
        float forward = Vector3.Dot(centerEyeAnchor.transform.forward, distance) * -1;
        float side = Vector3.Dot(centerEyeAnchor.transform.right, distance) * -1;
        float up = Vector3.Dot(centerEyeAnchor.transform.up, distance) * -1;

        return new Vector3(forward, side, up);
    }

    /// <summary>
    /// Finds the location of the secondary hand relative to the center of the player's eyesight
    /// <br></br>Positive vector is (forward, right, above) the origin
    /// <br></br>Negative vector is (behind, left, below) the origin
    /// </summary>
    /// 
    /// <returns> Hand location as a vector, with origin roughly in the center of the player's eyesight </returns>
    public Vector3 SecondaryLocation()
    {
        Vector3 distance = centerEyeAnchor.transform.position - secondaryHand.transform.position;

        float forward = Vector3.Dot(centerEyeAnchor.transform.forward, distance) * -1;
        float side = Vector3.Dot(centerEyeAnchor.transform.right, distance) * -1;
        float up = Vector3.Dot(centerEyeAnchor.transform.up, distance) * -1;

        return new Vector3(forward, side, up);
    }

    /// <summary>
    /// Gets the FingerShape for a specified FingerType on the primary hand
    /// </summary>
    /// 
    /// <param name="finger"> The FingerType being examined, connected to a specific Finger instance </param>
    /// 
    /// <returns> The FingerShape that the Finger is using </returns>
    public Finger.FingerShape PrimaryFingerShape(Finger.FingerType finger)
    {
        switch (finger)
        {
            case Finger.FingerType.Thumb:
                return primaryThumb.ShapeThumb();
            case Finger.FingerType.Index:
                return primaryIndex.Shape();
            case Finger.FingerType.Middle:
                return primaryMiddle.Shape();
            case Finger.FingerType.Ring:
                return primaryRing.Shape();
            case Finger.FingerType.Pinky:
                return primaryPinky.Shape();
        }

        return Finger.FingerShape.Max;
    }

    /// <summary>
    /// Gets the FingerShape for a specified FingerType on the secondary hand
    /// </summary>
    /// 
    /// <param name="finger"> The FingerType being examined, connected to a specific Finger instance </param>
    /// 
    /// <returns> The FingerShape that the Finger is using </returns>
    public Finger.FingerShape SecondaryFingerShape(Finger.FingerType finger)
    {
        switch (finger)
        {
            case Finger.FingerType.Thumb:
                return secondaryThumb.ShapeThumb();
            case Finger.FingerType.Index:
                return secondaryIndex.Shape();
            case Finger.FingerType.Middle:
                return secondaryMiddle.Shape();
            case Finger.FingerType.Ring:
                return secondaryRing.Shape();
            case Finger.FingerType.Pinky:
                return secondaryPinky.Shape();
        }

        return Finger.FingerShape.Max;
    }

    /// <summary>
    /// Gets the Shape ID for the entire primary hand, where each index contains the FingerShape
    /// <br></br>of a finger, starting with the thumb at index 0 and ending with the pinky at index 4.
    /// </summary>
    /// 
    /// <returns> FingerShape ID for the primary hand, described above </returns>
    public int[] PrimaryHandShape()
    {
        int[] ret = new int[5];
        ret[0] = (int)PrimaryFingerShape(Finger.FingerType.Thumb);
        ret[1] = (int)PrimaryFingerShape(Finger.FingerType.Index);
        ret[2] = (int)PrimaryFingerShape(Finger.FingerType.Middle);
        ret[3] = (int)PrimaryFingerShape(Finger.FingerType.Ring);
        ret[4] = (int)PrimaryFingerShape(Finger.FingerType.Pinky);

        return ret;
    }

    /// <summary>
    /// Gets the Shape ID for the entire secondary hand, where each index contains the FingerShape
    /// <br></br>of a finger, starting with the thumb at index 0 and ending with the pinky at index 4.
    /// </summary>
    /// 
    /// <returns> FingerShape ID for the secondary hand, described above </returns>
    public int[] SecondaryHandShape()
    {
        int[] ret = new int[5];
        ret[0] = (int)SecondaryFingerShape(Finger.FingerType.Thumb);
        ret[1] = (int)SecondaryFingerShape(Finger.FingerType.Index);
        ret[2] = (int)SecondaryFingerShape(Finger.FingerType.Middle);
        ret[3] = (int)SecondaryFingerShape(Finger.FingerType.Ring);
        ret[4] = (int)SecondaryFingerShape(Finger.FingerType.Pinky);

        return ret;
    }

    /// <summary>
    /// Determines if the actual HandShape of the primary hand is exactly the same as the desired HandShape
    /// </summary>
    /// 
    /// <param name="desiredHandShape"> The desired FingerShapes, with the Thumb FingerShape at index 0 (Any indexes containing a value of 0 are skipped over) </param>
    /// 
    /// <returns> True if the HandShape is exactly the same, false otherwise </returns>
    public bool PrimaryExactHandShape(Finger.FingerShape[] desiredHandShape)
    {
        if (desiredHandShape.Length != 5)
        {
            return false;
        }

        int[] actualHandShape = PrimaryHandShape();

        for (int i = 0; i < desiredHandShape.Length; i++)
        {
            if (desiredHandShape[i] != 0)
            {
                if (actualHandShape[i] != (int)desiredHandShape[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if the actual HandShape of the secondary hand is exactly the same as the desired HandShape
    /// </summary>
    /// 
    /// <param name="desiredHandShape"> The desired FingerShapes, with the Thumb FingerShape at index 0 (Any indexes containing a value of 0 are skipped over) </param>
    /// 
    /// <returns> True if the HandShape is close enough, false otherwise </returns>
    public bool SecondaryExactHandShape(Finger.FingerShape[] desiredHandShape)
    {
        if (desiredHandShape.Length != 5)
        {
            return false;
        }

        int[] actualHandShape = SecondaryHandShape();

        for (int i = 0; i < desiredHandShape.Length; i++)
        {
            if (desiredHandShape[i] != 0)
            {
                if (actualHandShape[i] != (int)desiredHandShape[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if the actual HandShape of the primary hand is close enough to the desired HandShape
    /// <br></br>(tolerancePercent of FingerShapes correct, with the rest within a range of tolerance)
    /// <br></br><i>[ex: tolerance of 2 from Bent goes to Extended and Inward, inclusive]</i>
    /// </summary>
    /// 
    /// <param name="desiredHandShape"> The desired FingerShapes, with the Thumb FingerShape at index 0 (Any indexes containing a value of 0 are skipped over) </param>
    /// <param name="tolerance"> How far away a FingerShape can be from the desired FingerShape </param>
    /// <param name="tolerancePercent"> Minimum percentage of FingerShapes that have to be exactly correct, inclusive </param>
    /// 
    /// <returns> True if the HandShape is close enough, false otherwise or if an array of length other than 5 is passed </returns>
    public bool PrimaryTolerantHandShape(Finger.FingerShape[] desiredHandShape, int tolerance, float tolerancePercent)
    {
        if (desiredHandShape.Length != 5)
        {
            return false;
        }

        int[] actualHandShape = PrimaryHandShape();

        int numCorrect = 0;
        int numZero = 0;

        for (int i = 0; i < desiredHandShape.Length; i++)
        {
            if (desiredHandShape[i] != 0)
            {
                if (Math.Abs(actualHandShape[i] - (int)desiredHandShape[i]) > tolerance)
                {
                    return false;
                }

                if (actualHandShape[i] == (int)desiredHandShape[i])
                {
                    numCorrect++;
                }
            } else { numZero++; }
        }

        float total = desiredHandShape.Length - numZero;

        if (numCorrect / total >= tolerancePercent - 0.01f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the actual HandShape of the secondary hand is close enough to the desired HandShape
    /// <br></br>(tolerancePercent of FingerShapes correct, with the rest within a range of tolerance)
    /// </summary>
    /// 
    /// <param name="desiredHandShape"> The desired FingerShapes, with the Thumb FingerShape at index 0 (Any indexes containing a value of 0 are skipped over) </param>
    /// <param name="tolerance"> How far away a FingerShape can be from the desired FingerShape </param>
    /// <param name="tolerancePercent"> Minimum percentage of FingerShapes that have to be exactly correct, inclusive </param>
    /// 
    /// <returns> True if the HandShape is close enough, false otherwise or if an array of length other than 5 is passed </returns>
    public bool SecondaryTolerantHandShape(Finger.FingerShape[] desiredHandShape, int tolerance, float tolerancePercent)
    {
        if (desiredHandShape.Length != 5)
        {
            return false;
        }

        int[] actualHandShape = SecondaryHandShape();

        int numCorrect = 0;
        int numZero = 0;

        for (int i = 0; i < desiredHandShape.Length; i++)
        {
            if (desiredHandShape[i] != 0)
            {
                if (Math.Abs(actualHandShape[i] - (int)desiredHandShape[i]) > tolerance)
                {
                    return false;
                }

                if (actualHandShape[i] == (int)desiredHandShape[i])
                {
                    numCorrect++;
                }
            }
            else { numZero++; }
        }

        float total = desiredHandShape.Length - numZero;

        if (numCorrect / total >= tolerancePercent - 0.01f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if two FingerTypes are touching on the primary hand
    /// <br></br>(It is recommended to use <see cref="HandManager.PrimaryFingerPinch"/> for detecting fingertip to thumb-tip)
    /// 
    /// </summary>
    /// 
    /// <param name="fing1"> The first finger being examined </param>
    /// <param name="fing2"> The second finger being examined </param>
    /// 
    /// <returns> True if touching, false if not </returns>
    public bool PrimaryFingerTouch(Finger.FingerType fing1, Finger.FingerType fing2)
    {
        Vector4 fing1_bones = new Vector4();
        Vector4 fing2_bones = new Vector4();

        switch (fing1)
        {
            case Finger.FingerType.Thumb:
                fing1_bones = primaryThumb.GetBones();
                break;
            case Finger.FingerType.Index:
                fing1_bones = primaryIndex.GetBones();
                break;
            case Finger.FingerType.Middle:
                fing1_bones = primaryMiddle.GetBones();
                break;
            case Finger.FingerType.Ring:
                fing1_bones = primaryRing.GetBones();
                break;
            case Finger.FingerType.Pinky:
                fing1_bones = primaryPinky.GetBones();
                break;
        }

        switch (fing2)
        {
            case Finger.FingerType.Thumb:
                fing2_bones = primaryThumb.GetBones();
                break;
            case Finger.FingerType.Index:
                fing2_bones = primaryIndex.GetBones();
                break;
            case Finger.FingerType.Middle:
                fing2_bones = primaryMiddle.GetBones();
                break;
            case Finger.FingerType.Ring:
                fing2_bones = primaryRing.GetBones();
                break;
            case Finger.FingerType.Pinky:
                fing2_bones = primaryPinky.GetBones();
                break;
        }

        float distance;

        distance = Vector4.Magnitude(primarySkele.Bones[(int)fing1_bones.z].Transform.position - primarySkele.Bones[(int)fing2_bones.z].Transform.position) * 100;

        if (distance >= 4 && fing1 != Finger.FingerType.Thumb && fing2 != Finger.FingerType.Thumb)
        {
            return false;
        }
        else if (distance >= 4 && (fing1 == Finger.FingerType.Thumb || fing2 == Finger.FingerType.Thumb))
        {
            if (fing1 == Finger.FingerType.Thumb && fing2 == Finger.FingerType.Index)
            {
                distance = Vector4.Magnitude(primarySkele.Bones[(int)fing1_bones.w].Transform.position - primarySkele.Bones[(int)fing2_bones.x].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            if (fing2 == Finger.FingerType.Thumb && fing1 == Finger.FingerType.Index)
            {
                distance = Vector4.Magnitude(primarySkele.Bones[(int)fing1_bones.x].Transform.position - primarySkele.Bones[(int)fing2_bones.w].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }


            if (fing1 == Finger.FingerType.Thumb && fing2 == Finger.FingerType.Middle)
            {
                distance = Vector4.Magnitude(primarySkele.Bones[(int)fing1_bones.w].Transform.position - primarySkele.Bones[(int)fing2_bones.y].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            if (fing2 == Finger.FingerType.Thumb && fing1 == Finger.FingerType.Middle)
            {
                distance = Vector4.Magnitude(primarySkele.Bones[(int)fing1_bones.y].Transform.position - primarySkele.Bones[(int)fing2_bones.w].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if two FingerTypes are touching on the secondary hand
    /// <br></br>(It is recommended to use SecondaryFingerPinch() for detecting fingertip to thumb-tip)
    /// </summary>
    /// 
    /// <param name="fing1"> The first finger being examined </param>
    /// <param name="fing2"> The second finger being examined </param>
    /// 
    /// <returns> True if touching, false if not </returns>
    public bool SecondaryFingerTouch(Finger.FingerType fing1, Finger.FingerType fing2)
    {
        Vector4 fing1_bones = new Vector4();
        Vector4 fing2_bones = new Vector4();

        switch (fing1)
        {
            case Finger.FingerType.Thumb:
                fing1_bones = secondaryThumb.GetBones();
                break;
            case Finger.FingerType.Index:
                fing1_bones = secondaryIndex.GetBones();
                break;
            case Finger.FingerType.Middle:
                fing1_bones = secondaryMiddle.GetBones();
                break;
            case Finger.FingerType.Ring:
                fing1_bones = secondaryRing.GetBones();
                break;
            case Finger.FingerType.Pinky:
                fing1_bones = secondaryPinky.GetBones();
                break;
        }

        switch (fing2)
        {
            case Finger.FingerType.Thumb:
                fing2_bones = secondaryThumb.GetBones();
                break;
            case Finger.FingerType.Index:
                fing2_bones = secondaryIndex.GetBones();
                break;
            case Finger.FingerType.Middle:
                fing2_bones = secondaryMiddle.GetBones();
                break;
            case Finger.FingerType.Ring:
                fing2_bones = secondaryRing.GetBones();
                break;
            case Finger.FingerType.Pinky:
                fing2_bones = secondaryPinky.GetBones();
                break;
        }

        float distance;

        distance = Vector4.Magnitude(secondarySkele.Bones[(int)fing1_bones.z].Transform.position - secondarySkele.Bones[(int)fing2_bones.z].Transform.position) * 100;

        if (distance >= 4 && fing1 != Finger.FingerType.Thumb && fing2 != Finger.FingerType.Thumb)
        {
            return false;
        }
        else if (distance >= 4 && (fing1 == Finger.FingerType.Thumb || fing2 == Finger.FingerType.Thumb))
        {
            if (fing1 == Finger.FingerType.Thumb && fing2 == Finger.FingerType.Index)
            {
                distance = Vector4.Magnitude(secondarySkele.Bones[(int)fing1_bones.w].Transform.position - secondarySkele.Bones[(int)fing2_bones.x].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            if (fing2 == Finger.FingerType.Thumb && fing1 == Finger.FingerType.Index)
            {
                distance = Vector4.Magnitude(secondarySkele.Bones[(int)fing1_bones.x].Transform.position - secondarySkele.Bones[(int)fing2_bones.w].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }


            if (fing1 == Finger.FingerType.Thumb && fing2 == Finger.FingerType.Middle)
            {
                distance = Vector4.Magnitude(secondarySkele.Bones[(int)fing1_bones.w].Transform.position - secondarySkele.Bones[(int)fing2_bones.y].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            if (fing2 == Finger.FingerType.Thumb && fing1 == Finger.FingerType.Middle)
            {
                distance = Vector4.Magnitude(secondarySkele.Bones[(int)fing1_bones.y].Transform.position - secondarySkele.Bones[(int)fing2_bones.w].Transform.position) * 100;
                if (distance < 4.2)
                {
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Retrieves the pinch strength between a passed FingerType and the thumb on the primary hand
    /// </summary>
    /// 
    /// <param name="fing"> The FingerType being examined </param>
    /// 
    /// <returns> The pinch strength, as given by Oculus' GetFingerPinchStrength() </returns>
    public float PrimaryFingerPinch(Finger.FingerType fing)
    {
        OVRHand.HandFinger finger;
        switch (fing)
        {
            case Finger.FingerType.Thumb:
                finger = OVRHand.HandFinger.Thumb;
                break;
            case Finger.FingerType.Index:
                finger = OVRHand.HandFinger.Index;
                break;
            case Finger.FingerType.Middle:
                finger = OVRHand.HandFinger.Middle;
                break;
            case Finger.FingerType.Ring:
                finger = OVRHand.HandFinger.Ring;
                break;
            case Finger.FingerType.Pinky:
                finger = OVRHand.HandFinger.Pinky;
                break;
            default:
                finger = OVRHand.HandFinger.Max;
                break;
        }

        return (primaryHand.GetFingerPinchStrength(finger));
    }

    /// <summary>
    /// Retrieves the pinch strength between a passed FingerType and the thumb on the secondary hand
    /// </summary>
    /// 
    /// <param name="fing"> The FingerType being examined </param>
    /// 
    /// <returns> The pinch strength, as given by Oculus' GetFingerPinchStrength() </returns>
    public float SecondaryFingerPinch(Finger.FingerType fing)
    {
        OVRHand.HandFinger finger;
        switch (fing)
        {
            case Finger.FingerType.Thumb:
                finger = OVRHand.HandFinger.Thumb;
                break;
            case Finger.FingerType.Index:
                finger = OVRHand.HandFinger.Index;
                break;
            case Finger.FingerType.Middle:
                finger = OVRHand.HandFinger.Middle;
                break;
            case Finger.FingerType.Ring:
                finger = OVRHand.HandFinger.Ring;
                break;
            case Finger.FingerType.Pinky:
                finger = OVRHand.HandFinger.Pinky;
                break;
            default:
                finger = OVRHand.HandFinger.Max;
                break;
        }

        return (secondaryHand.GetFingerPinchStrength(finger));
    }

    /// <summary>
    /// Gets the orientation of the primary hand as an Orientation array, with format [forwardVal, inVal, downVal, rotationVal, thumbVal]
    /// <br></br>
    /// <br></br>Where forwardVal is whether the palm of the hand faces forward (Palm_Front), backward (Palm_Back), or between (Palm_Mid_Fr)
    /// <br></br>Where inVal is whether the palm of the hand faces inward (Palm_In), outward (Palm_Out), or between (Palm_Mid_In)
    /// <br></br>Where downVal is whether the palm of the hand faces upward (Palm_Up), downward (Palm_Down), or between (Palm_Mid_Dw)
    /// <br></br>Where rotationVal is whether the main 4 knuckles of the hand face up (Knuckles_Up), down (Knuckles_Down), between (Knuckles_Mid), or in (Knuckles_In)
    /// <br></br>Where thumbVal is whether the thumb knuckle faces upward (Thumb_Up), downward (Thumb_Down), between (Thumb_Mid), or inward (Thumb_In)
    /// </summary>
    /// 
    /// <returns> Hand orientation, described above </returns>
    public Orientation[] PrimaryOrientation()
    {
        Orientation[] orients = new Orientation[5];
        Orientation fwdVal;
        Orientation inVal;
        Orientation dwnVal;
        Orientation rotVal;
        Orientation thmVal;

        // Dot products between player camera and back of the hand
        float x = Vector3.Dot(centerEyeAnchor.transform.forward, primaryHand.transform.up);
        float y = Vector3.Dot(centerEyeAnchor.transform.right, primaryHand.transform.up);
        float z = Vector3.Dot(centerEyeAnchor.transform.up, primaryHand.transform.up);

        // Dot products between player camera and the knuckles of the hand
        float rotY = Vector3.Dot(centerEyeAnchor.transform.right, -primaryHand.transform.right);
        float rotZ = Vector3.Dot(centerEyeAnchor.transform.up, -primaryHand.transform.right);

        // Dot products between player camera and the knuckles of the hand
        float thmY = Vector3.Dot(centerEyeAnchor.transform.forward, -primaryHand.transform.forward);
        float thmZ = Vector3.Dot(centerEyeAnchor.transform.up, -primaryHand.transform.forward);


        if (primaryHand.Equals(rightOVRHandPrefab))
        {
            // If palm is facing forward...
            if (x >= 0.25) { fwdVal = Orientation.Palm_Back; } else if (x <= -0.25) { fwdVal = Orientation.Palm_Front; } else { fwdVal = Orientation.Palm_Mid_Fr; }

            // If palm is facing inward...
            if (y <= -0.25) { inVal = Orientation.Palm_Out; } else if (y >= 0.25) { inVal = Orientation.Palm_In; } else { inVal = Orientation.Palm_Mid_In; }

            // If palm is facing down...
            if (z >= 0.5) { dwnVal = Orientation.Palm_Down; } else if (z <= -0.5) { dwnVal = Orientation.Palm_Up; } else { dwnVal = Orientation.Palm_Mid_Dw; }

            // If knuckles are facing up...
            if (rotZ >= 0.8) { rotVal = Orientation.Knuckles_Up; } else if (rotZ <= -0.5) { rotVal = Orientation.Knuckles_Down; } else { rotVal = Orientation.Knuckles_Mid; }
            
            // If knuckles are middle and facing in...
            if (rotVal == Orientation.Knuckles_Mid && rotY <= -0.5) { rotVal = Orientation.Knuckles_In; }

            // If thumb is facing up...
            if (thmZ >= 0.5) { thmVal = Orientation.Thumb_Up; } else if (thmZ <= -0.5) { thmVal = Orientation.Thumb_Down; } else { thmVal = Orientation.Thumb_Mid; }

            // If thumb is middle and facing in...
            if (thmVal == Orientation.Thumb_Mid && thmY <= -0.5) { thmVal = Orientation.Thumb_In; }
        }
        else
        {
            // If palm is facing forward...
            if (x >= 0.25) { fwdVal = Orientation.Palm_Front; } else if (x <= -0.25) { fwdVal = Orientation.Palm_Back; } else { fwdVal = Orientation.Palm_Mid_Fr; }

            // If palm is facing inward...
            if (y <= -0.25) { inVal = Orientation.Palm_Out; } else if (y >= 0.25) { inVal = Orientation.Palm_In; } else { inVal = Orientation.Palm_Mid_In; }

            // If palm is facing down...
            if (z >= 0.5) { dwnVal = Orientation.Palm_Up; } else if (z <= -0.5) { dwnVal = Orientation.Palm_Down; } else { dwnVal = Orientation.Palm_Mid_Dw; }

            // If knuckles are facing up...
            if (rotZ >= 0.8) { rotVal = Orientation.Knuckles_Down; } else if (rotZ <= -0.5) { rotVal = Orientation.Knuckles_Up; } else { rotVal = Orientation.Knuckles_Mid; }

            // If knuckles are middle and facing in...
            if (rotVal == Orientation.Knuckles_Mid && rotY <= -0.5) { rotVal = Orientation.Knuckles_In; }

            // If thumb is facing up...
            if (thmZ >= 0.5) { thmVal = Orientation.Thumb_Down; } else if (thmZ <= -0.5) { thmVal = Orientation.Thumb_Up; } else { thmVal = Orientation.Thumb_Mid; }

            // If thumb is middle and facing in...
            if (thmVal == Orientation.Thumb_Mid && thmY <= -0.5) { thmVal = Orientation.Thumb_In; }
        }

        // Create orients[]
        orients[0] = fwdVal;
        orients[1] = inVal;
        orients[2] = dwnVal;
        orients[3] = rotVal;
        orients[4] = thmVal;
        return orients;
    }

    /// <summary>
    /// Gets the orientation of the secondary hand as an Orientation array, with format [forwardVal, inVal, downVal, rotationVal, thumbVal]
    /// <br></br>
    /// <br></br>Where forwardVal is whether the palm of the hand faces forward (Palm_Front), backward (Palm_Back), or between (Palm_Mid_Fr)
    /// <br></br>Where inVal is whether the palm of the hand faces inward (Palm_In), outward (Palm_Out), or between (Palm_Mid_In)
    /// <br></br>Where downVal is whether the palm of the hand faces upward (Palm_Up), downward (Palm_Down), or between (Palm_Mid_Dw)
    /// <br></br>Where rotationVal is whether the main 4 knuckles of the hand face up (Knuckles_Up), down (Knuckles_Down), between (Knuckles_Mid), or in (Knuckles_In)
    /// <br></br>Where thumbVal is whether the thumb knuckle faces upward (Thumb_Up), downward (Thumb_Down), between (Thumb_Mid), or inward (Thumb_In)
    /// </summary>
    /// 
    /// <returns> Hand orientation, described above </returns>
    public Orientation[] SecondaryOrientation()
    {
        Orientation[] orients = new Orientation[5];
        Orientation fwdVal;
        Orientation inVal;
        Orientation dwnVal;
        Orientation rotVal;
        Orientation thmVal;

        // Dot products between player camera and back of the hand
        float x = Vector3.Dot(centerEyeAnchor.transform.forward, secondaryHand.transform.up);
        float y = Vector3.Dot(centerEyeAnchor.transform.right, secondaryHand.transform.up);
        float z = Vector3.Dot(centerEyeAnchor.transform.up, secondaryHand.transform.up);

        // Dot products between player camera and the knuckles of the hand
        float rotY = Vector3.Dot(centerEyeAnchor.transform.right, -secondaryHand.transform.right);
        float rotZ = Vector3.Dot(centerEyeAnchor.transform.up, -secondaryHand.transform.right);

        // Dot products between player camera and the knuckles of the hand
        float thmY = Vector3.Dot(centerEyeAnchor.transform.forward, -secondaryHand.transform.forward);
        float thmZ = Vector3.Dot(centerEyeAnchor.transform.up, -secondaryHand.transform.forward);


        if (secondaryHand.Equals(rightOVRHandPrefab))
        {
            // If palm is facing forward...
            if (x >= 0.25) { fwdVal = Orientation.Palm_Back; } else if (x <= -0.25) { fwdVal = Orientation.Palm_Front; } else { fwdVal = Orientation.Palm_Mid_Fr; }

            // If palm is facing inward...
            if (y <= -0.25) { inVal = Orientation.Palm_Out; } else if (y >= 0.25) { inVal = Orientation.Palm_In; } else { inVal = Orientation.Palm_Mid_In; }

            // If palm is facing down...
            if (z >= 0.5) { dwnVal = Orientation.Palm_Down; } else if (z <= -0.5) { dwnVal = Orientation.Palm_Up; } else { dwnVal = Orientation.Palm_Mid_Dw; }

            // If knuckles are facing up...
            if (rotZ >= 0.8) { rotVal = Orientation.Knuckles_Up; } else if (rotZ <= -0.5) { rotVal = Orientation.Knuckles_Down; } else { rotVal = Orientation.Knuckles_Mid; }

            // If knuckles are middle and facing in...
            if (rotVal == Orientation.Knuckles_Mid && rotY <= -0.5) { rotVal = Orientation.Knuckles_In; }

            // If thumb is facing up...
            if (thmZ >= 0.5) { thmVal = Orientation.Thumb_Up; } else if (thmZ <= -0.5) { thmVal = Orientation.Thumb_Down; } else { thmVal = Orientation.Thumb_Mid; }

            // If thumb is middle and facing in...
            if (thmVal == Orientation.Thumb_Mid && thmY <= -0.5) { thmVal = Orientation.Thumb_In; }
        }
        else
        {
            // If palm is facing forward...
            if (x >= 0.25) { fwdVal = Orientation.Palm_Front; } else if (x <= -0.25) { fwdVal = Orientation.Palm_Back; } else { fwdVal = Orientation.Palm_Mid_Fr; }

            // If palm is facing inward...
            if (y <= -0.25) { inVal = Orientation.Palm_Out; } else if (y >= 0.25) { inVal = Orientation.Palm_In; } else { inVal = Orientation.Palm_Mid_In; }

            // If palm is facing down...
            if (z >= 0.5) { dwnVal = Orientation.Palm_Up; } else if (z <= -0.5) { dwnVal = Orientation.Palm_Down; } else { dwnVal = Orientation.Palm_Mid_Dw; }

            // If knuckles are facing up...
            if (rotZ >= 0.8) { rotVal = Orientation.Knuckles_Down; } else if (rotZ <= -0.5) { rotVal = Orientation.Knuckles_Up; } else { rotVal = Orientation.Knuckles_Mid; }

            // If knuckles are middle and facing in...
            if (rotVal == Orientation.Knuckles_Mid && rotY <= -0.5) { rotVal = Orientation.Knuckles_In; }

            // If thumb is facing up...
            if (thmZ >= 0.5) { thmVal = Orientation.Thumb_Down; } else if (thmZ <= -0.5) { thmVal = Orientation.Thumb_Up; } else { thmVal = Orientation.Thumb_Mid; }

            // If thumb is middle and facing in...
            if (thmVal == Orientation.Thumb_Mid && thmY <= -0.5) { thmVal = Orientation.Thumb_In; }
        }

        // Create orients[]
        orients[0] = fwdVal;
        orients[1] = inVal;
        orients[2] = dwnVal;
        orients[3] = rotVal;
        orients[4] = thmVal;
        return orients;
    }

    /// <summary>
    /// Adds a snapshot of part of a multi-part gesture and detects if the full gesture is completed.
    /// <br></br>Calling this as the parameter for SetCurrentGesture() will allow implementation of multi-part gestures
    /// </summary>
    /// 
    /// <param name="previousGesture"> The name of the snapshot that must be gestured before this one </param>
    /// <param name="currentGesture"> The name of the snapshot currently being gestured </param>
    /// <param name="isFinal"> True if it's the final snapshot in a multi-part gesture, false otherwise </param>
    /// <param name="time"> The time in seconds that the user has to gesture the next snapshot </param>
    /// 
    /// <returns> currentGesture if multi-part gesture is complete, null if otherwise </returns>
    public string AddSnapshot(String previousGesture, String currentGesture, bool isFinal, float time)
    {
        // If the Snapshot is already being tracked...
        if (snapshotTimes.ContainsKey(currentGesture))
        {
            // ...reset its time
            snapshotTimes[currentGesture] = time;

            // If the Snapshot is the final gesture, return currentGesture
            if (isFinal)
            {
                return currentGesture;
            }

            return null;
        }

        // If the Snapshot is the first in a sequence and is not being tracked...
        if (previousGesture == null)
        {
            // ...track the Snapshot and exit
            snapshotTimes.Add(currentGesture, time);
            return null;
        }

        // If the Snapshot is in the middle of a sequence and is not being tracked...
        if (previousGesture != null && !isFinal)
        {
            // If the previous gesture has been detected recently, replace it with the new gesture
            if (snapshotTimes.ContainsKey(previousGesture))
            {
                snapshotTimes.Remove(previousGesture);
                snapshotTimes.Add(currentGesture, time);
            }
            return null;
        }

        // If the Snapshot is the last in a sequence and is not being tracked...
        if (isFinal)
        {
            // If the previous gesture has been detected recently, replace it with the new gesture and set HandManager's currentGesture
            if (snapshotTimes.ContainsKey(previousGesture))
            {
                snapshotTimes.Remove(previousGesture);
                snapshotTimes.Add(currentGesture, time);
                return currentGesture;
            }
            return null;
        }

        return null;
    }

    /// <summary>
    /// Decreases the time of all currently running snapshots by deltaTime. Called in update()
    /// </summary>
    private void DecrementSnapshots()
    {
        if (snapshotTimes.Count != 0)
        {
            snapshots = new string[snapshotTimes.Count];
            snapshotTimes.Keys.CopyTo(snapshots, 0);

            foreach (string snap in snapshots)
            {
                snapshotTimes[snap] -= Time.deltaTime;

                if (snapshotTimes[snap] <= 0)
                {
                    snapshotTimes.Remove(snap);
                }
            }
        }
    }


}




/// <summary>
/// A class used to refer to the fingers on a hand, primarily for precise gesture detection.
/// <br></br>
/// <br></br><b>Useful public qualities include, but are not limited to:</b>
/// <br></br>enum Finger.FingerType      ==> Types of Fingers (Thumb, Index, Middle, Ring, Pinky)
/// <br></br>enum Finger.FingerShape     ==> Types of FingerShapes (Extended, Curved, Bent, Folded, Inward)
/// <br></br>Vector4 GetBones()          ==> Retrieves four Oculus boneIDs for a Finger (proximal, intermediate, distal, tip)
/// <br></br>FingerShape Shape()         ==> Retrieves the FingerShape for Index, Middle, Ring, and Pinky Fingers
/// <br></br>FingerShape ShapeThumb()    ==> Retrieves the FingerShape for Thumb Fingers
/// </summary>
public class Finger
{
    // Constants used in finger shape detection
    readonly double LITSTRAIGHT_VAL = 0.9;
    readonly double DIRECTION_VAL = 0.0;
    readonly double BIGSTRAIGHT_VAL = 0.9;
    readonly double LITSTRAIGHT_THUMB_VAL = 0.75;
    readonly double BIGSTRAIGHT_THUMB_VAL = 0.7;

    /// <summary> Type of finger of the current Finger object </summary>
    public FingerType finger;

    /// <summary> Skeleton of the hand the Finger belongs to </summary>
    private OVRSkeleton skeleton;

    /// <summary> Dictionary of bones corresponding to each FingerType </summary>
    private readonly Dictionary<FingerType, Vector4> bones = new Dictionary<FingerType, Vector4>() 
    {
        { FingerType.Thumb, new Vector4(2, 4, 5, 19) },
        { FingerType.Index, new Vector4(6, 7, 8, 20) },
        { FingerType.Middle, new Vector4(9, 10, 11, 21) },
        { FingerType.Ring, new Vector4(12, 13, 14, 22) },
        { FingerType.Pinky, new Vector4(16, 17, 18, 23) }
    };

    /// <summary> 
    /// The types of fingers that can be used
    /// <br></br>Used as parameters for functions in HandManager
    /// </summary>
    public enum FingerType
    {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Pinky = 4,
        Max = 5,
    }

    /// <summary> 
    /// The types of fingershapes that can be detected
    /// </summary>
    public enum FingerShape
    {
        /// <summary>
        /// Extended outward, straight from the knuckle to the tip
        /// </summary>
        Extended = 1,

        /// <summary>
        /// Curved slightly down from extended position
        /// </summary>
        Curved = 2,

        /// <summary>
        /// Bent down from curved position, where the tip touches the back of the knuckle
        /// </summary>
        Bent = 3,

        /// <summary>
        /// Folded in from bent position, where the tip is firmly planted in the palm
        /// </summary>
        Folded = 4,

        /// <summary>
        /// Pointed inward from bent position, where the tip is pointing at the wrist
        /// </summary>
        Inward = 5,

        /// <summary>
        /// Error case
        /// </summary>
        Max = 6,
    }

    /// <summary>
    /// Constructor for the Finger class
    /// </summary>
    /// 
    /// <param name="type"> The FingerType of the Finger being instantiated </param>
    /// <param name="skele"> The Finger's corresponding OVRSkeleton </param>
    public Finger(FingerType type, OVRSkeleton skele)
    {
        finger = type;
        skeleton = skele;
    }

    /// <summary>
    /// Returns the bones of the finger
    /// </summary>
    /// 
    /// <returns> A Vector4 of the bones in the specified FingerType </returns>
    public Vector4 GetBones()
    {
        Vector4 ret;
        bones.TryGetValue(finger, out ret);
        return ret;
    }

    /// <summary>
    /// Determines the FingerShape being used by the Finger
    /// </summary>
    /// 
    /// <returns> The current active FingerShape </returns>
    public FingerShape Shape()
    {
        if (bigStraight() && direction())
        {
            return FingerShape.Extended;
        }
        if (litStraight() && direction())
        {
            return FingerShape.Curved;
        }
        if (!litStraight() && direction())
        {
            return FingerShape.Bent;
        }
        if (!litStraight() && !direction())
        {
            return FingerShape.Folded;
        }
        if (litStraight() && !direction())
        {
            return FingerShape.Inward;
        }

        return FingerShape.Max;
    }

    /// <summary>
    /// Determines the FingerShape being used by the Finger if FingerType is a thunb.
    /// </summary>
    /// 
    /// <returns> The current active FingerShape for the thumb </returns>
    public FingerShape ShapeThumb()
    {
        if (litStraightThumb() && directionThumb() > 0.7 && bigStraightThumb())
        {
            return FingerShape.Extended;
        }
        if ((litStraightThumb() && directionThumb() > 0.4) || (!litStraightThumb() && directionThumb() > 0.5))
        {
            return FingerShape.Curved;
        }
        if (!litStraightThumb() && directionThumb() < 0.5 && directionThumb() > 0.0)
        {
            return FingerShape.Bent;
        }
        if (!litStraightThumb() && directionThumb() < 0)
        {
            return FingerShape.Folded;
        }
        if (bigStraightThumb() && directionThumb() < 0.7)
        {
            return FingerShape.Inward;
        }

        return FingerShape.Max;
    }


    // THE FOLLOWING FUNCTIONS ARE USED TO CALCULATE FINGERSHAPE AND SHOULD NOT BE ACCESSED OTHERWISE

    /**
     * Finds the little straightness of the Finger if FingerType is a thumb.
     *   @return True if little straightness is greater than LITSTRAIGHT_THUMB_VAL, false otherwise
     */
    private bool litStraightThumb()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 mid = skeleton.Bones[(int)outp.z].Transform.position - skeleton.Bones[(int)outp.y].Transform.position;
        Vector3 top = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.z].Transform.position;

        float litStraight = Vector3.Dot(top.normalized, mid.normalized);

        if (litStraight > LITSTRAIGHT_THUMB_VAL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
     * Finds the direction of the Finger if FingerType is a thumb.
     *   @return Float representation of the direction used for comparisons
     */
    private float directionThumb()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 half = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.y].Transform.position;
        Vector3 palm = skeleton.Bones[(int)outp.x].Transform.position - skeleton.Bones[0].Transform.position;

        float direction = Vector3.Dot(half.normalized, palm.normalized);

        return direction;
    }


    /**
     * Finds the big straightness of the Finger if FingerType is a thumb.
     *   @return True if big straightness is greater than BIGSTRAIGHT_THUMB_VAL, false otherwise
     */
    private bool bigStraightThumb()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 top = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.z].Transform.position;
        Vector3 half = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.y].Transform.position;

        float bigStraight = Vector3.Dot(top.normalized, half.normalized);

        if (bigStraight > BIGSTRAIGHT_THUMB_VAL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
     * Finds the little straightness of the Finger.
     *   @return True if little straightness is greater than LITSTRAIGHT_VAL, false otherwise
     */
    private bool litStraight()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 mid = skeleton.Bones[(int)outp.z].Transform.position - skeleton.Bones[(int)outp.y].Transform.position;
        Vector3 half = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.y].Transform.position;

        float litStraight = Vector3.Dot(half.normalized, mid.normalized);

        if (litStraight > LITSTRAIGHT_VAL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
     * Finds the direction of the Finger.
     *   @return True if direction is greater than DIRECTION_VAL, false otherwise
     */
    private bool direction()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 len = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.x].Transform.position;
        Vector3 palm = skeleton.Bones[(int)outp.x].Transform.position - skeleton.Bones[0].Transform.position;

        float direction = Vector3.Dot(len.normalized, palm.normalized);

        if (direction > DIRECTION_VAL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /**
    * Finds the big straightness of the Finger.
    *   @return True if big straightness is greater than BIGSTRAIGHT_VAL, false otherwise
    */
    private bool bigStraight()
    {
        Vector4 outp;
        bones.TryGetValue(finger, out outp);

        Vector3 bot = skeleton.Bones[(int)outp.y].Transform.position - skeleton.Bones[(int)outp.x].Transform.position;
        Vector3 len = skeleton.Bones[(int)outp.w].Transform.position - skeleton.Bones[(int)outp.x].Transform.position;

        float bigStraight = Vector3.Dot(len.normalized, bot.normalized);

        if (bigStraight > BIGSTRAIGHT_VAL)
        {
           return true;
        }
        else
        {
           return false;
        }
    }
}