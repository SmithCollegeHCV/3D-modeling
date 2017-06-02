using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Kinect = Windows.Kinect;
public class coordinateMap : MonoBehaviour
{
    private BodySourceManager bodyManager;
    public GameObject BodySourceManager;
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

    /* Use this for initialization
    void Start()
    {



    }*/

    // Update is called once per frame
    GameObject jointObj;
    Vector3 position;
    KinectSensor sensor;
    void Start()
    {
        BodySourceManager = GameObject.Find("BodyManager");
        jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        jointObj.transform.localScale = new Vector3(100,100,100);
        sensor = KinectSensor.GetDefault();
    }
    void Update()
    {
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
        Body[] data = bodyManager.GetData();
        if (data == null)
        {
            print("Body Data is Null");
            return;
        }
        List<ulong> trackedIds = new List<ulong>();
        //For each body that exists
        foreach (var body in data)
        {
            //If body == null, don't do any more code, just iterate the loop
            if (body == null)
            {
                //Stop here and iterate the loop again
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        foreach (var body in data)
        {
            if (body != null)
            {
                if (body.IsTracked) //Important, used to see if this particular body instance is being tracked by the kinect at this time
                {
                    for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                    {
                        if (jt.ToString() == "Head")
                        {



                            Kinect.Joint sourceJoint = body.Joints[jt];

                            CameraSpacePoint cameraPoint = sourceJoint.Position;
                            ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint);
                            //DepthSpacePoint depthPoint = sensor.CoordinateMapper.MapCameraPointToDepthSpace(cameraPoint);
                            
                            //CameraSpacePoint headPoint = sourceJoint.Position;
                            //ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(headPoint);

                            //position = GetVector3FromJoint(sourceJoint);

                            //print(headPoint.X + "," + headPoint.Y);
                            //print(colorPoint.X + "," + colorPoint.Y);
                            //jointObj.transform.localPosition = position;
                            Vector3 position = new Vector3(colorPoint.X , -colorPoint.Y, 0);

                            //Vector3 position = new Vector3(cameraPoint.X, cameraPoint.Y, 0/*cameraPoint.Z*/);
                            
                            //Vector3 position = new Vector3(depthPoint.X, depthPoint.Y, 0);
                            //Vector3 point = new Vector3(position.x, position.y, 0);
                            jointObj.transform.position = position;
                           
                            //print("found head");//jointObj = GameObject.Find("tophat");

                            print(position.x + "," + position.y + "," + position.z);
                            //print("transforming");

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
    private static Kinect.Joint scaleJointPosition(Kinect.Joint joint, Kinect.Joint joint2) {
        float factor = 400;
        Vector3 position = new Vector3(joint.Position.X*factor, joint.Position.Y*factor, /*joint.Position.Z*factor*/0);

        return joint;
}
}
