using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{

    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    private bool showingParts = true;
    public TextMeshProUGUI showtext;
    public List<Canvas> canvasobj = new List<Canvas>();
    public GameObject leftdoor, rightdoor;
    public GameObject showbutton;
    public GameObject engineon;
    public AudioSource engineaudio;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }


        UpdatePlacementPose();
        UpdatePlacementIndicator();


    }
    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    void ARPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
        leftdoor = spawnedObject.transform.GetChild(1).gameObject;
        rightdoor = spawnedObject.transform.GetChild(2).gameObject;
        Canvas[] canvasobjs = spawnedObject.GetComponentsInChildren<Canvas>();

        canvasobj.AddRange(canvasobjs);

        foreach (Canvas canvas in canvasobj)
        {
            canvas.gameObject.SetActive(false);
        }

        showbutton.SetActive(true);
        engineon.SetActive(true);
    }

    public void show()
    {
        if (showingParts)
        {
            showingParts = false;
            showtext.text = "Hide Parts Name";
            Vector3 currentRotationleft = leftdoor.transform.eulerAngles;
            Vector3 currentRotationright = rightdoor.transform.eulerAngles;
            currentRotationleft.y = 40;
            currentRotationright.y = -40f;
            leftdoor.transform.eulerAngles = currentRotationleft;
            rightdoor.transform.eulerAngles = currentRotationright;
            foreach (Canvas canvas in canvasobj)
            {
                canvas.gameObject.SetActive(true);
            }
        }
        else
        {
            showingParts = true;
            showtext.text = "Show Parts Name";
            Vector3 currentRotationleft = leftdoor.transform.eulerAngles;
            Vector3 currentRotationright = rightdoor.transform.eulerAngles;
            currentRotationleft.y = 0;
            currentRotationright.y = 0;
            leftdoor.transform.eulerAngles = currentRotationleft;
            rightdoor.transform.eulerAngles = currentRotationright;
            foreach (Canvas canvas in canvasobj)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    public void engine()
    {
        engineaudio.Play();
    }
}

