using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class UserTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public bool TrackActive;
    public bool isTracking;

    protected TrackableBehaviour mTrackableBehaviour;

    TrackableBehaviour.Status CurrentState;

    private void Awake()
    {
        isTracking = false;
        TrackActive = false;
    }

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

    }

    private void Update()
    {
        Debug.Log(CurrentState);
    }

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged
        (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
       // if(TrackActive)
        {
            CurrentState = newStatus;

            if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)// ||
           // newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
                OnTrackingFound();
            }
            else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                     newStatus == TrackableBehaviour.Status.NOT_FOUND)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
                OnTrackingLost();
            }
            else
            {
                // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
                // Vuforia is starting, but tracking has not been lost or found yet
                // Call OnTrackingLost() to hide the augmentations
                OnTrackingLost();
            }
        }
    }



    protected virtual void OnTrackingFound()
    {
        //
       // MainManager.GetInstance.WebPlaneBehavior.UDTIsTracking = true;
        MainManager.GetInstance.UseUserDefinedTarget = true;
        isTracking = true;

    }


    protected virtual void OnTrackingLost()
    {
        //
       // MainManager.GetInstance.WebPlaneBehavior.UDTIsTracking = false;
        MainManager.GetInstance.UseUserDefinedTarget = false;
        isTracking = false;

    }
}
