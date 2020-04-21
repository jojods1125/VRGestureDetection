# VR Hand Tracking Gesture Detection Package for use with Oculus Integration for Unity
#### Created by Joseph Dasilva
This package gives developers the ability to create and track hand gestures using Oculus's hand tracking technology. Use of this package requires packages that
come with Oculus Integration for Unity, available on the Unity Asset Store. It is required to have Version 14.0 or newer.

## File Structure
	GestureDetection              (Files necessary for gesture detection)
		Libraries             (GestureLibrary components)
			*Gestures.cs
		HandManager.cs        (Uses OVRHand data to create hand/gesture information)
		GestureLibrary.cs     (Root class for gesture libraries)

	DemoScene                     (Optional demo files)
		Audio
		Materials
		Scripts
		Text
		DemoGame.unity        (Demo scene)
		RawTest.unity         (Testing scene)

## How to Set Up
Download the Oculus Integration package from the Unity Asset Store. Make sure all Oculus plugins and packages are up to date!

When creating a scene, ensure that there is an OVRCameraRig in the Hierarchy similar to the one found in DemoScene/DemoGame.unity. On older versions of the Oculus Integration, the OVRManager component
should have Quest checked as the Target Device, as well as Quest Features > Hand Tracking Support set to Hands Only.** If these fields are not available on your version of OVRManager, then you should be okay with the default values.

To obtain hand tracking data that is used in gesture detection, place an empty GameObject in the scene with a HandManager component. Initialize the Center Eye
Actor, Left OVR Hand Prefab, and Right OVR Hand Prefab fields in the editor with the accordingly titled objects found within the OVRCameraRig object.

HandManager assigns one hand as the PrimaryHand and the other as the SecondaryHand. By default, the PrimaryHand is the right hand and the SecondaryHand is the
left hand. This can be changed with HandManager.SetPrimaryHand(hand), where hand is HandType.Left or HandType.Right.

To use gesture detection, attach an existing GestureLibrary component (found in GestureDetection/Libraries) to the same GameObject that has the HandManager 
component. Initialize the Manager field with the HandManager component found within the scene. You can set which gestures can be detected using the public 
boolean fields found under Active Gestures.

To obtain the currently detected gesture, call HandManager.GetCurrentGesture().

**At the time of this package's creation, Oculus has not released hand tracking support for any devices outside of the Oculus Quest. As a result, this
information may be subject to change as new devices are supported.

## How to Create Gestures
Gestures are detected by GestureLibrary components. It is recommended that developers modify a duplicate of the provided ExampleGestures.cs file found
in GestureDetection/Libraries.

GestureLibrary components use the data given by functions within the HandManager component to detect gestures. These functions are all described in the code
documentation, and a select few will be described in the documentation provided below. Combining the outputs of these functions, as shown in the existing
GestureLibrary components, allows for gestures to be fully defined.


#### SetCurrentGesture(string) : string
Sets the current gesture being performed. Each active GestureLibrary must call this function on every update, as it expects a number of calls equal to the
number of active GestureLibrary components. Passing null is equivalent to saying no gesture detected.


#### GetCurrentGesture() : string
Retrieves the currently detected gesture's name. Use this to tell which gesture is detected. Will default to "None" if no gestures are found, or "Number of
libraries invalid" if there is an error concerning the number of libraries reporting gestures.


#### PrimaryLocation(), SecondaryLocation() : Vector3
Finds the location of the specified hand (Primary/Secondary) relative to the center of the player's eyesight. Output vector has the format (forward/behind,
right/left, above/below), where forward, right of, and above the origin are positive values, and behind, left of, and below the origin are negative values.


#### PrimaryOrientation(), SecondaryOrientation() : Orientation[ ]
Gets the orientation of the primary hand as an Orientation array, with format [forwardVal, inVal, downVal, rotationVal, thumbVal].

    Where forwardVal is whether the palm of the hand faces forward (Palm_Front), backward (Palm_Back), or between (Palm_Mid_Fr)
    Where inVal is whether the palm of the hand faces inward (Palm_In), outward (Palm_Out), or between (Palm_Mid_In)
    Where downVal is whether the palm of the hand faces upward (Palm_Up), downward (Palm_Down), or between (Palm_Mid_Dw)
    Where rotationVal is whether the main 4 knuckles of the hand face up (Knuckles_Up), down (Knuckles_Down), between (Knuckles_Mid), or in (Knuckles_In)
    Where thumbVal is whether the thumb knuckle faces upward (Thumb_Up), downward (Thumb_Down), between (Thumb_Mid), or inward (Thumb_In)


#### PrimaryFingerShape(Finger.FingerType), SecondaryFingerShape(Finger.FingerType) : Finger.FingerShape
Gets the FingerShape for the given FingerType on the specified hand. FingerShapes are defined as follows:

    1 - Extended  (Extended outward, straight from the knuckle to the tip)
    2 - Curved    (Curved slightly down from extended position)
    3 - Bent      (Bent down from curved position, where the tip touches the back of the knuckle)
    4 - Folded    (Folded in from bent position, where the tip is firmly planted in the palm)
    5 - Inward    (Pointed inward from bent position, where the tip is pointing at the wrist)


#### PrimaryHandShape(), SecondaryHandShape() : int[ ]
Gets the shape ID for the entire specified hand, where each index contains the FingerShape of a finger, starting with the thumb at index 0 and ending with the
pinky at index 4.


#### PrimaryExactHandShape(Finger.FingerShape[ ]), SecondaryExactHandShape(Finger.FingerShape[ ]) : bool
Determines if the actual HandShape of the specified hand is exactly the same as the given desired HandShape. Indexes that contain a value of 0 are skipped over
to allow for checking specific fingers.

#### PrimaryTolerantHandShape(Finger.FingerShape[ ], int, float), SecondaryTolerantHandShape(Finger.FingerShape[ ], int, float) : bool
Determines if the actual HandShape of the specified hand is close enough to the desired HandShape, which is calculated as:

    minimum of float percentage of FingerShapes correct, with the rest within a range of int

Indexes that contain a value of 0 are skipped over to allow for checking specific fingers, and are not taken into account when finding percentage. (ex: 50% of
an array with two non-zero values is one)


#### PrimaryFingerTouch(Finger.FingerType, Finger.FingerType), SecondaryFingerTouch(Finger.FingerType, Finger.FingerType) : bool
Determines if two FingerTypes are touching on the specified hand. It is recommended to use PrimaryFingerPinch() or SecondaryFingerPinch() for detecting
fingertip to thumb-tip interactions.


#### PrimaryFingerPinch(Finger.FingerType), SecondaryFingerPinch(Finger.FingerType) : float
Retrieves the pinch strength between a passed FingerType and the thumb on the specified hand, as given by as given by Oculus' GetFingerPinchStrength().


#### AddSnapshot(string, string, bool, float) : string
Adds a snapshot of part of a multi-part gesture and detects if the full gesture is completed. Parameters are as follows:

    string - The name of the snapshot that must be gestured before this one
    string - The name of the snapshot currently being gestured
    bool   - True if it's the final snapshot in a multi-part gesture, false otherwise
    float  - The time in seconds that the user has to gesture the next snapshot

Calling this as the parameter for SetCurrentGesture() will allow implementation of multi-part gestures. Returns the name of the snapshot if it's the final
gesture in a multi-part gesture, null otherwise.
