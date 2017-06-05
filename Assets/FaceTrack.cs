using UnityEngine;
using System.Collections;
using Windows.Kinect;//一定要引用这个
using Microsoft.Kinect.Face;//还有这个

public class FaceTrack : MonoBehaviour
{
    private KinectSensor kinectSensor;
    private int bodyCount;
    private Body[] bodies;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private BodyFrameReader bodyFrameReader;

    void Start()
    {
        // one sensor is currently supported
        kinectSensor = KinectSensor.GetDefault();

        // set the maximum number of bodies that would be tracked by Kinect
        bodyCount = kinectSensor.BodyFrameSource.BodyCount;

        bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
        //      bodyCount = 1;
        // allocate storage to store body objects
        bodies = new Body[bodyCount];

        // specify the required face frame results
        FaceFrameFeatures faceFrameFeatures =
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

    void Update()
    {
        // get bodies either from BodySourceManager object get them from a BodyReader
        //var bodySourceManager = bodyManager.GetComponent<BodySourceManager>();
        //bodies = bodySourceManager.GetData();
        if (bodies == null)
        {
            return;
        }

        using (var bodyFrame = bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(bodies);



                // iterate through each body and update face source
                for (int i = 0; i < bodyCount; i++)
                {
                    //          Debug.Log(i);
                    // check if a valid face is tracked in this face source             
                    if (faceFrameSources[i].IsTrackingIdValid)
                    {
                        using (FaceFrame frame = faceFrameReaders[i].AcquireLatestFrame())
                        {
                            if (frame != null)
                            {
                                if (frame.TrackingId == 0)
                                {
                                    continue;
                                }
                                // do something with result
                                var result = frame.FaceFrameResult;
                                if (result.FaceRotationQuaternion != null)
                                {
                                    Debug.Log(result.FaceRotationQuaternion.X);
                                }
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
                                                Debug.Log("Got here");
                                            }
                                        }
                                        

                                    }
                                }

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
            }
        }


    }
}