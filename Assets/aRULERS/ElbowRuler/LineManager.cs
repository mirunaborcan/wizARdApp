using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
public class LineManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public ARPlacementInteractable placementInteractable;
    public TextMeshPro dText;
    public TextMeshPro aText;

    // Start is called before the first frame update
    void Start()
    {
        //when a object is placed we call the DrawLine function
        placementInteractable.objectPlaced.AddListener(DrawLine);
    }
    void DrawLine(ARObjectPlacementEventArgs args)
    {
        // in this function we want to increase point count
        // and let the points location in line renderer
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, args.placementObject.transform.position);
        
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

            //make the distance text parallel with line
            Vector3 directionVect = (pointB - pointA);
            Vector3 normal = args.placementObject.transform.up;
            Vector3 upd = Vector3.Cross(directionVect, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, upd);
            distObj.transform.rotation = rotation;
            distObj.transform.position = (pointA + directionVect * 0.5f) + upd*0.05f;

            //calculate angle between any two lines which make an angle smaller than 180 degrees 
            if (lineRenderer.positionCount > 2)
            {
                Vector3 pointC = lineRenderer.GetPosition(lineRenderer.positionCount - 3);
                float angle = Vector3.Angle(pointA - pointB, pointC - pointB);
                if (angle < 180)
                {
                    TextMeshPro angleObj = Instantiate(aText);
                    angleObj.text = angle.ToString("F2") + "°";

                    // set the degree text near the apex
                    // TO DO: to be improved 
                    Vector3 dir = (pointC - pointA).normalized;
                    Vector3 normalAngle = args.placementObject.transform.up;
                    Vector3 downAngle = Vector3.Cross(normalAngle, dir).normalized;
                    Quaternion angleRotation = Quaternion.LookRotation(-normalAngle, downAngle);
                    angleObj.transform.rotation = angleRotation;
                    angleObj.transform.position = pointB - downAngle * 0.05f;
                }
            }
        }

    }
}
