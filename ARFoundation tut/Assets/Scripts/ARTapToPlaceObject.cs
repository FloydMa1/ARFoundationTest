using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToBePlaced;
    [SerializeField] private GameObject placementInd;

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool poseIsValid = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }
    

    void Update()
    {
        GetInput();
        UpdatePlacement();
        UpdatePlacementInd();
    }

    private void GetInput()
    {
        if(poseIsValid && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToBePlaced, placementInd.transform.position, placementInd.transform.rotation);
    }

    private void UpdatePlacementInd()
    {
        if (poseIsValid)
        {
            placementInd.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            placementInd.SetActive(true);
        }
        else
        {
            placementInd.SetActive(false);
        }
    }

    private void UpdatePlacement()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        poseIsValid = hits.Count >= 0;

        if (poseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
