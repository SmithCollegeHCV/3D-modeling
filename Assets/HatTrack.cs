using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
public class HatTrack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    GameObject head; 
	void Update () {
        GameObject Head = GameObject.Find("Sphere");

        if (Head)
        {
            gameObject.transform.position = Head.transform.position +Vector3.up*20;
        }
	}
}
