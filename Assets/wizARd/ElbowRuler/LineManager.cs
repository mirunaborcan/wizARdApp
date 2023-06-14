using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LineManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public ARPlacementInteractable placementInteractable;
    public TextMeshPro dText;
    public TextMeshPro aText;
    public TMP_Text totalDistanceText;

    private float totalDistance;
    private bool placementEnabled;
    //unit of measurement
    Units currentUnit = Units.m;

    // Start is called before the first frame update
    void Start()
    {
        //this is a reference to an ARPlacementInteractable component,
        //that triggers the DrawLine function when an object is placed
        placementInteractable.objectPlaced.AddListener(DrawLine);
    }
    private void Update()
    {
        TotalDistance();
    }
    void DrawLine(ARObjectPlacementEventArgs args)
    {
        // in this function we want to increase point count
        // and let the points location in line renderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, args.placementObject.transform.position);

        //looking for at least two points to draw a line 
        if (lineRenderer.positionCount > 1)
        {
            Vector3 pointA = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            Vector3 pointB = lineRenderer.GetPosition(lineRenderer.positionCount - 2);

            //calculate distance between them
            float distance = Vector3.Distance(pointA, pointB) * 100f;
            string distanceText = distance.ToString("0.00");

            TextMeshPro distObj = Instantiate(dText);
            distObj.text = "" + distanceText + " cm";
            totalDistance += distance;
            Debug.Log($"Distance is {totalDistance}");


            //make the distance text parallel with line
            Vector3 directionVect = (pointB - pointA);
            Vector3 normal = args.placementObject.transform.up;
            Vector3 upd = Vector3.Cross(directionVect, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, upd);
            distObj.transform.rotation = rotation;
            distObj.transform.position = (pointA + directionVect * 0.5f) + upd * 0.05f;

            //calculate angle between any two lines which make an angle smaller than 180 degrees 
            if (lineRenderer.positionCount > 2)
            {
                Vector3 pointC = lineRenderer.GetPosition(lineRenderer.positionCount - 3);
                float angle = Vector3.Angle(pointA - pointB, pointC - pointB);
                if (angle < 180)
                {
                    TextMeshPro angleObj = Instantiate(aText);
                    angleObj.text = angle.ToString("F2") + "°";

                    // Calculează poziția textului în exteriorul triunghiului
                    Vector3 dir = (pointC - pointA).normalized;
                    Vector3 normalAngle = args.placementObject.transform.up;
                    Vector3 downAngle = Vector3.Cross(normalAngle, dir).normalized;
                    Quaternion angleRotation = Quaternion.LookRotation(-normalAngle, downAngle);
                    Vector3 textPosition = pointB - downAngle * 0.05f;
                    textPosition += normalAngle * 0.1f; 
                    angleObj.transform.rotation = angleRotation;
                    angleObj.transform.position = textPosition;
                }
            }
        }
        
    }
    public void TotalDistance()
    {
        //convert units
        float convertedDistance = 0f; 

        switch (currentUnit)
        {
            case Units.m:
                convertedDistance = totalDistance / 100f;
                break;
            case Units.cm:
                convertedDistance = totalDistance;
                break;
            case Units.i:
                convertedDistance = (totalDistance / 100f) / 0.0254f;
                break;
            case Units.f:
                convertedDistance = (totalDistance / 100f) * 3.2808f;
                break;
            default:
                break;
        }

        string distanceStr = convertedDistance.ToString("#.##") + currentUnit;

        totalDistanceText.text = distanceStr;
    }
    //casting string (from the inspector) into a Unit so we can act upon it
    public void ChangeUnits(string unit)
    {
        currentUnit = (Units)System.Enum.Parse(typeof(Units), unit);
    }

}
public enum Units
{
    m,
    cm,
    i,
    f
}
