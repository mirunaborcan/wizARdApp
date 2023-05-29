using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using TMPro;
public class RectangleArea : MonoBehaviour
{
    public LineRenderer lineRenderer;
    void Start()
    {
        // get screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // calculate size and position of square
        float squareSize = Mathf.Min(screenWidth, screenHeight) * 0.5f;
        float squareX = screenWidth * 0.5f - squareSize * 0.5f;
        float squareY = screenHeight * 0.5f - squareSize * 0.5f;

        // set position and size of square
        lineRenderer.SetPosition(0, new Vector3(squareX, squareY, 0));
        lineRenderer.SetPosition(1, new Vector3(squareX + squareSize, squareY, 0));
        lineRenderer.SetPosition(2, new Vector3(squareX + squareSize, squareY + squareSize, 0));
        lineRenderer.SetPosition(3, new Vector3(squareX, squareY + squareSize, 0));
        lineRenderer.SetPosition(4, new Vector3(squareX, squareY, 0));
    }
}

