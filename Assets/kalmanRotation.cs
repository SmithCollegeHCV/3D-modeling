using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Kinect = Windows.Kinect;
using Microsoft.Kinect.Face;
public class kalmanRotation : MonoBehaviour
{
    //Import BodySourceManager class 

    private BodySourceManager bodyManager;
    //Unity requires instance of script in-game to use it
    private GameObject BodySourceManager;
    private KinectSensor kinectSensor;
    private Body[] bodies;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private BodyFrameReader bodyFrameReader;
    private FaceFrameFeatures faceFrameFeatures;
    private int bodyCount;
    private Vector3 position;
    private int scale;
    private GameObject headJointDisplay;
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
    
    void Start()
    {
    	initKinect();
    	initBodies();
    	setFaceFrameFeatures();
        createFaceFrameSources();
        scale = 8;
        headJointDisplay = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        headJointDisplay.transform.localScale = new Vector3(25,25,25);
        position = new Vector3(0,0,0);
        // BodySourceManager = GameObject.Find("BodyManager");
        // //Grab the BodySourceManager script from unity gameObject BodySourceManager
        // bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        // bodyManager
    }

    void Update()
    {
        // if (bodyManager == null){ return; }
        // //If the body data is null, stop
        // data = bodyManager.GetData();
        // if (data == null){ return; }
        //     if (bodies == null)
        // {
        //     return;
        // }

        using (var bodyFrame = bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                
                bodyFrame.GetAndRefreshBodyData(bodies);
                // iterate through each body and update face source
                for (int i = 0; i < bodyCount; i++)
                {       
                	getAndHandleFaceFrame(i);
                    coordinateMapJoints(i);
                }
            }
        }
    }
    private void initKinect()
    {
        //one sensor is currently supported
        kinectSensor = KinectSensor.GetDefault();
    }
    private void initBodies()
    {
        // set the maximum number of bodies that would be tracked by Kinect, 6
        bodyCount = kinectSensor.BodyFrameSource.BodyCount;
        bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
        // allocate storage to store body objects
        bodies = new Body[bodyCount];
    }
	private void setFaceFrameFeatures()
	    {
	        // specify the required face frame results
	        faceFrameFeatures =
	            FaceFrameFeatures.BoundingBoxInColorSpace
	                | FaceFrameFeatures.PointsInColorSpace
	                | FaceFrameFeatures.BoundingBoxInInfraredSpace
	                | FaceFrameFeatures.PointsInInfraredSpace
	                | FaceFrameFeatures.RotationOrientation
	                | FaceFrameFeatures.FaceEngagement
	                | FaceFrameFeatures.Glasses
	                | FaceFrameFeatures.Happy
	                | FaceFrameFeatures.LeftEyeClosed
	                | FaceFrameFeatures.RightEyeClosed
	                | FaceFrameFeatures.LookingAway
	                | FaceFrameFeatures.MouthMoved
	                | FaceFrameFeatures.MouthOpen;
	    }

	private void createFaceFrameSources()
	    {
	        // create a face frame source + reader to track each face in the FOV
	        faceFrameSources = new FaceFrameSource[bodyCount];
	        faceFrameReaders = new FaceFrameReader[bodyCount];

	        for (int i = 0; i < bodyCount; i++)
	        {
	            // create the face frame source with the required face frame features and an initial tracking Id of 0
	            faceFrameSources[i] = FaceFrameSource.Create(kinectSensor, 0, faceFrameFeatures);
	            // open the corresponding reader
	            faceFrameReaders[i] = faceFrameSources[i].OpenReader();
	        }
	    }
	//Acquire face face frame, do stuff with it 
    private void getAndHandleFaceFrame(int i){
    	if (faceFrameSources[i].IsTrackingIdValid)
        {
            using (FaceFrame frame = faceFrameReaders[i].AcquireLatestFrame())
            {
            	if (frame != null)
                {
                    if (frame.TrackingId == 0)
                    {
                    	return;
                    }
                    // do something with result
                    FaceFrameResult result = frame.FaceFrameResult;
                    rotateHat(result);
                    handleFaceProperties(result);
                }
            }
        }
        else
        {
            // check if the corresponding body is tracked 
            if (bodies[i].IsTracked)
            {
                // update the face frame source to track this body
                faceFrameSources[i].TrackingId = bodies[i].TrackingId;
            }
        }
    }
    
    //Method for accessing facial expressions/other properties 
    private void handleFaceProperties(FaceFrameResult result)
    {
        if (result.FaceProperties != null)
        {
            foreach (var item in result.FaceProperties)
            {
                /*faceText += item.Key.ToString() + " : ";
                // consider a "maybe" as a "no" to restrict 
                // the detection result refresh rate
                if (item.Value == DetectionResult.Maybe)
                {
                    faceText += DetectionResult.No + "\n";
                }
                else
                {
                    faceText += item.Value.ToString() + "\n";
                }    */
                //Debug.Log("face property");//
                if (item.Key == FaceProperty.Happy)
                {
                    if (item.Value == DetectionResult.Yes)
                    {
                        Debug.Log(item.Value);
                    }
                }
            }
        }
    }
    private void rotateHat(FaceFrameResult result)
    {
        if (result.FaceRotationQuaternion != null)
        {
            var rot = result.FaceRotationQuaternion;
            Quaternion rotation = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);
            GameObject.Find("tophat").transform.rotation = rotation;
        }
    }
    private void coordinateMapJoints(int i){
        if (bodies[i].IsTracked){
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++){
                if (jt.ToString() == "Head"){
                    Kinect.Joint headJoint = bodies[i].Joints[jt];
                    CameraSpacePoint cameraPoint = headJoint.Position;
                    ColorSpacePoint colorPoint2D = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint);
                    position = new Vector3(colorPoint2D.X/scale,-colorPoint2D.Y/scale,-10);
                    headJointDisplay.transform.position = position;
                    float zDistance = GetVector3FromJoint(headJoint).z * (float)5.5;
                    GameObject.Find("tophat").transform.localScale = new Vector3((float)scale - zDistance, (float)scale - zDistance, (float)scale - zDistance);
                    print(position.x + "," + position.y + "," + position.z);
                    // position.X = ;
                    // position.Y = ;
                    // position.Z = -10;
                }
            }

        }
    }


    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        float factor = 10000;
        Vector3 position = new Vector3(joint.Position.X * factor, joint.Position.Y * factor, joint.Position.Z);
        return position;
    }
}
