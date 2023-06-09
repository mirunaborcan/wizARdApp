using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.EventSystems;

public class SimpleRulerManager : MonoBehaviour
{
    ARRaycastManager aRRaycastManager;

    //points placed by user on the screen by tapping
    public GameObject[] tapePoints;
    // the reticle
    public GameObject reticle;

    //unit of measurement
    Unit currentUnit = Unit.m;
    
    int currentTapePoint = 0;
    float distanceBetweenPoints = 0f;

    public TMP_Text distanceText;
    public TMP_Text floatingDistanceText;
    public GameObject floatingDistanceObject;
    public LineRenderer line;

    void Start()
    {
        //look for a ARRaycastManager component
        //this manager is responsible for performing raycasting in AR environment
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }
    void Update()
    {
        UpdateDistance();
        PlaceFloatingText();

        /* RAYCASTING */
        
        // create a new empty list called hits to store the raycast hits
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // shoot a raycast from the center of the screen
        aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);
        //if the raycast hits a plane, update the position and rotation of the reticle,
        //enables the reticle if necessary, and allows the user to place tape points by tapping on the detected planes
        if (hits.Count > 0)
        {
            reticle.transform.position = hits[0].pose.position;
            reticle.transform.rotation = hits[0].pose.rotation;

            //draw the line to the reticle if the first point is placed
            if (currentTapePoint == 1)
            {
                DrawLine();
            }
            // enable the reticle if its disabled and the tape points aren't placed yet
            if (!reticle.activeInHierarchy && currentTapePoint < 2)
            {
                reticle.SetActive(true);
            }
            //if the user taps, place a tape point
            //disable more placements until the end of the touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // check if the touch is within the bounds of any button
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    // touch is not over a button, place the point
                    if (currentTapePoint < 2)
                    {
                        PlacePoint(hits[0].pose.position, currentTapePoint);
                    }
                }
            }
        }

        //if the raycast isn't hitting anything, don't display the reticle
        else if (hits.Count == 0 || currentTapePoint == 2)
        {
            reticle.SetActive(false);
        }

    }

    //change the position of the approperiate tape point and make it active
    public void PlacePoint(Vector3 pointPosition, int pointIndex)
    {
        tapePoints[pointIndex].SetActive(true);

        //this moves the tape point to the desired location in the scene
        tapePoints[pointIndex].transform.position = pointPosition;

        //this draws a line from the first tape point to the reticle, indicating the distance being measured
        if (currentTapePoint == 1)
        {
            DrawLine();
        }
        //keeps track of the number of tape points placed in the scene.
        currentTapePoint += 1;
    }

    //set the positions of the line to the tape points (or reticle)
    void DrawLine()
    {
        //enable line rendering
        line.enabled = true;

        //set the position of the first vertex of the line (Position 0) to the position of the first tape point (tapePoints[0].transform.position)
        //this connects the line to the first tape point
        line.SetPosition(0, tapePoints[0].transform.position);

        //set the position of the second vertex of the line (Position 1) to the position of the reticle (reticle.transform.position)
        //this completes the line from the first tape point to the reticle
        if (currentTapePoint == 1)
        {
            line.SetPosition(1, reticle.transform.position);

        }
        //set the position of the second vertex of the line(Position 1) to the position of the second tape point(tapePoints[1].transform.position)
        //this completes the line from the first tape point to the second tape point
        else if (currentTapePoint == 2)
        {
            line.SetPosition(1, tapePoints[1].transform.position);

        }
    }
    void UpdateDistance()
    {
        if (currentTapePoint == 0)
        {
            distanceBetweenPoints = 0f;
        }
        else if (currentTapePoint == 1)
        {
            distanceBetweenPoints = Vector3.Distance(tapePoints[0].transform.position, reticle.transform.position);
        }
        else if (currentTapePoint == 2)
        {
            distanceBetweenPoints = Vector3.Distance(tapePoints[0].transform.position, tapePoints[1].transform.position);
        }

        //convert units
        float convertedDistance = 0f;

        switch (currentUnit)
        {
            case Unit.m:
                convertedDistance = distanceBetweenPoints;
                break;
            case Unit.cm:
                convertedDistance = distanceBetweenPoints * 100;
                break;
            case Unit.i:
                convertedDistance = distanceBetweenPoints / 0.0254f;
                break;
            case Unit.f:
                convertedDistance = distanceBetweenPoints * 3.2808f;
                break;
            default:
                break;
        }

        //change the text to display the distance
        string distanceStr = convertedDistance.ToString("#.##") + currentUnit;

        distanceText.text = distanceStr;
        floatingDistanceText.text = distanceStr;

    }

    //place the distance measured above the line
    void PlaceFloatingText()
    {
        if (currentTapePoint == 0)
        {
            floatingDistanceObject.SetActive(false);
        }
        else if (currentTapePoint == 1)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, reticle.transform.position, 0.5f);
        }
        else if (currentTapePoint == 2)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, tapePoints[1].transform.position, 0.5f);
        }

        floatingDistanceObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

    }

    //casting string (from the inspector) into a Unit so we can act upon it
    public void ChangeUnit(string unit)
    {
        currentUnit = (Unit)System.Enum.Parse(typeof (Unit), unit);
    }

}
public enum Unit {
    m,
    cm,
    i,
    f
}
