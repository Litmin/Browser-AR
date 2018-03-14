using UnityEngine;
using Vuforia;

public class NormalTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public bool TrackActive;

    protected TrackableBehaviour mTrackableBehaviour;

    public TrackableBehaviour.Status CurrentStatus;
    public bool IsTracking;

    private void Awake()
    {
        TrackActive = false;
        IsTracking = false;
    }

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    private void Update()
    {
    }

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged
        (TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus)
    {
        CurrentStatus = newStatus;
        //if(TrackActive)
        //不使用extened tracking
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                        newStatus == TrackableBehaviour.Status.TRACKED) //||
                        //newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
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
      //  if(MainManager.GetInstance.WebPlaneBehavior != null)
      //      MainManager.GetInstance.WebPlaneBehavior.NormalTracking = true;
        //关闭UDT定位
        MainManager.GetInstance.StopUserDefinedTrack();
        IsTracking = true;
    }


    protected virtual void OnTrackingLost()
    {
        //
        // if (MainManager.GetInstance.WebPlaneBehavior != null)
        //       MainManager.GetInstance.WebPlaneBehavior.NormalTracking = false;
        //MainManager.GetInstance.BeginUserDefinedTrack();
        IsTracking = false;
    }
}
