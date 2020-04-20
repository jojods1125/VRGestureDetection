using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGUI : MonoBehaviour
{
    public HandManager manager;
    public int boneId = 0;

    public GameObject fakeHand;
    public GameObject fakePlayer;

    private TextMesh text3D;

    void Start()
    {
        text3D = GetComponent<TextMesh>();
    }

    void Update()
    {
        switch (boneId)
        {

            /// FINGER STATUS
            case 0:
                text3D.text = "";
                for (int i = 0; i < 5; i++)
                {
                    text3D.text += manager.PrimaryHandShape()[i];
                }
                break;

            /// ORIENTATION
            case 1:
                HandManager.Orientation[] vec = manager.PrimaryOrientation();
                string output = vec[0].ToString() + " " + vec[1].ToString() + " " + vec[2].ToString() + " " + vec[3].ToString() + " " + vec[4].ToString();
                //if (vec.x == 1) { output += "forward, "; }  else if (vec.x == -1) { output += "backward, "; }   else { output += "middle, "; }
                //if (vec.y == 1) { output += "in, "; }       else if (vec.y == -1) { output += "out, "; }        else { output += "middle, "; }
                //if (vec.z == 1) { output += "down, "; }     else if (vec.z == -1) { output += "up, "; }         else { output += "middle, "; }
                text3D.text = output;
                break;

            /// LOCATION
            case 2:
                text3D.text = manager.PrimaryLocation().ToString("F3");
                break;

            /// NON-VR TEST
            case 3:
                text3D.text = manager.SecondaryLocation().ToString("F3");
                //float forward = Vector3.Dot(manager.player.transform.forward, manager.GetPrimaryHand().transform.right);
                //text3D.text = forward.ToString("F3");

                //float right = Vector3.Dot(manager.player.transform.right, manager.GetPrimaryHand().transform.right);
                //text3D.text += " " + right.ToString("F3");

                //float up = Vector3.Dot(manager.player.transform.up, manager.GetPrimaryHand().transform.right);
                //text3D.text += " " + up.ToString("F3");

                //float forward = Vector3.Dot(fakePlayer.transform.forward, fakeHand.transform.forward);
                //text3D.text = forward.ToString("F3");

                //float right = Vector3.Dot(fakePlayer.transform.right, fakeHand.transform.right);
                //text3D.text = right.ToString("F3");

                //float up = Vector3.Dot(fakePlayer.transform.up, fakeHand.transform.up);
                //text3D.text = up.ToString("F3");
                break;

            /// GESTURE RECOGNITION
            case 4:
                /// lul
                break;

            // FINGER TOUCHING
            case 5:
                bool thmInd = manager.SecondaryFingerTouch(Finger.FingerType.Thumb, Finger.FingerType.Index);
                bool indMid = manager.SecondaryFingerTouch(Finger.FingerType.Index, Finger.FingerType.Middle);
                bool midRin = manager.SecondaryFingerTouch(Finger.FingerType.Middle, Finger.FingerType.Ring);
                bool rinPin = manager.SecondaryFingerTouch(Finger.FingerType.Ring, Finger.FingerType.Pinky);
                text3D.text = thmInd + " " + indMid + " " + midRin + " " + rinPin;
                break;
            
            // FINGER PINCHING
            case 6:
                float indPinc = manager.PrimaryFingerPinch(Finger.FingerType.Index);
                float midPinc = manager.PrimaryFingerPinch(Finger.FingerType.Middle);
                float rinPinc = manager.PrimaryFingerPinch(Finger.FingerType.Ring);
                float pinPinc = manager.PrimaryFingerPinch(Finger.FingerType.Pinky);
                text3D.text = indPinc + " " + midPinc + " " + rinPinc + " " + pinPinc;
                break;

            case 7:
                text3D.text = manager.GetCurrentGesture();

                break;

            case 8:
                text3D.text = "" + manager.PrimaryOrientation()[3];
                break;

            default:
                text3D.text = "ERROR ID";
                break;
        }
    }
}
