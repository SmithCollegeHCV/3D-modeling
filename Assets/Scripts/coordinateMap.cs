using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Kinect = Windows.Kinect;
public class coordinateMap : MonoBehaviour
{
    //Dictionary not needed right now but will be useful later
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    //Import BodySourceManager class 
    private BodySourceManager bodyManager;

    //Fields 
    GameObject jointObj;
    Vector3 position;
    KinectSensor sensor;
    Body[] data;


    public GameObject BodySourceManager; 

    // Use this for initialization, only called once at the start
    void Start()
    {
        BodySourceManager = GameObject.Find("BodyManager");
        jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        jointObj.transform.localScale = new Vector3(100,100,100);
        sensor = KinectSensor.GetDefault();
    }
    // Update is called once per frame
    void Update()
    {
        //Make sure that the BodySourceManager exists in the game before we use it
        if (BodySourceManager == null)
        {
            print("bodyManagerNotFound");
            return;

        }
        bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (bodyManager == null)
        {
            return;
        }
        //If the body data is null, stop
        data = bodyManager.GetData();
        if (data == null)
        {
            print("Body Data is Null");
            return;
        }
        //For each body that exists
        foreach (var body in data)
        {
            //If body == null, don't do any more code, just iterate the loop
            if (body == null)
            {
                //Stop here and iterate the loop again
                continue;
            }
        }
        //Iterate over each body in our array
        foreach (var body in data)
        {
            //If the body exists...
            if (body != null)
            {
                //If the body is currently being tracked by Kinect...
                if (body.IsTracked) //Important, used to see if this particular body instance is being tracked by the kinect at this time
                {
                    //Loop over the joints
                    for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                    {
                        //If the joint is Head...
                        if (jt.ToString() == "Head")
                        {
                            //Reference the specific joint, not the Joint Type
                            Kinect.Joint sourceJoint = body.Joints[jt];
                            //3D point in space (x,y,z), real world coordinates (in meters)
                            CameraSpacePoint cameraPoint = sourceJoint.Position;
                            //Mapped point to 2D screen from 3D coordinates
                            ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint);
                            Vector3 position = new Vector3(colorPoint.X , colorPoint.Y, 0);
                            jointObj.transform.position = position;
                            print(position.x + "," + position.y + "," + position.z);
                        }
                    }
                }
            }
        }

    }
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        float factor = 10000;
        Vector3 position = new Vector3(joint.Position.X * factor, joint.Position.Y * factor, /*joint.Position.Z*factor*/0);
        return position;
    }
}
