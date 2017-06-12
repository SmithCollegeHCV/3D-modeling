using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet = MathNet;

public class KalmanFilter : MonoBehaviour {
    //shorthand for getters and setters
    public double KalmanGain { get; set; }
    public double X0 { get; set; } //predicted state
    public double P0 { get; set; } //predicted covariance, error in the estimate
    public double State { get; set; }
    public double Covariance { get; set; }

    public void SetState(double state, double covariance)
    {
        State = state;
        Covariance = covariance;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
