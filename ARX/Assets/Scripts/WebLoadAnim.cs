using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLoadAnim : MonoBehaviour {

    bool normalTracking;
    bool UDTTracking;

    public GameObject child;

	void Start ()
    {
        child.gameObject.GetComponent<MeshRenderer>().enabled = false;
	}
	
	void Update ()
    {
       
	}

    private void LateUpdate()
    {
        if(MainManager.GetInstance.WebPlaneBehavior.NormalTracking)
        {
            this.transform.localPosition = MainManager.GetInstance.NormalImageTarget.transform.localPosition;
            this.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
            this.transform.localRotation = MainManager.GetInstance.NormalImageTarget.transform.localRotation;
            child.transform.Rotate(0.0f, 200.0f * Time.deltaTime, 0.0f);

            return;
        }
        if (MainManager.GetInstance.WebPlaneBehavior.UDTIsTracking)
        {
            this.transform.localPosition = MainManager.GetInstance.WebPlaneBehavior.UDTGameObject.transform.localPosition;
            this.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            this.transform.rotation = MainManager.GetInstance.WebPlaneBehavior.UDTGameObject.transform.rotation;
            this.transform.Rotate(0, -90, 0);
            child.transform.Rotate(0.0f, 200.0f * Time.deltaTime, 0.0f);
            return;
        }
        child.transform.Rotate(0.0f, 200.0f * Time.deltaTime, 0.0f);
    }
}
