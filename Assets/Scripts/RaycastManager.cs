using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RaycastManager : MonoBehaviour
{
    public ShapesManager _shapesManager;
    public Button _placePointButton;
    public Button _movePointButton;
    public Button _deleteVertexButton;

    private RaycastHit _hit;
    private bool _isVertexHover = false;
    private GameObject _vertexHoverObj;

    private bool _moveVertexButtonPressed = false;

    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        //this finds and assigns a UIManager component in the scene
        _uiManager = FindObjectOfType<UIManager>();
    }

    //this performs a raycast from the center of the screen using Camera.main.ViewportPointToRay
    //if the raycast hits an object, the script performs different actions based on following cases
    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if(Physics.Raycast(ray, out _hit))
        {
            //CASE 1 - if Raycast hit ARPlane
            if (_hit.transform.CompareTag("ARPlane"))
            {
                // this checks if the moveVertexButton is pressed
                // if so, it calls MoveVertex on the ShapesManager with the hit point,
                // and update UI accordingly by calling RaycastHitPlane on the UIManager
                if (_moveVertexButtonPressed)
                {
                    _shapesManager.MoveVertex(_vertexHoverObj, _hit.point);
                    _uiManager.RaycastHitPlane(true);
                }
                else
                {   //this resets the vertex hover state
                    //if the user was previously raycasting a vertex
                    if (_isVertexHover)
                    {
                        _isVertexHover = false;
                        _shapesManager.VertexHover(_vertexHoverObj, false);
                        _vertexHoverObj = null;
                    }

                    _uiManager.RaycastHitPlane(false);
                }
            }


            //CASE 2 - if Raycast hit a vertex
            else if (_hit.transform.CompareTag("Vertices"))
            {
                //if user is already moving another vertex
                if (_moveVertexButtonPressed)
                    return;

                //if not, updates the UI accordingly by calling RaycastHitVertex on the UIManager,
                //and calls the VertexHover method on the ShapesManager
                _uiManager.RaycastHitVertex();
                _isVertexHover = true;
                _vertexHoverObj = _hit.collider.gameObject;
                _shapesManager.VertexHover(_vertexHoverObj, true);
            }

            //CASE 3 - if Raycast hit something but we don't care 
            else
            {
                //resets the vertex hover state, updates UI accordingly
                _uiManager.RaycastHitNothing();
                if(_isVertexHover)
                {
                    _shapesManager.VertexHover(_vertexHoverObj, false);
                    _vertexHoverObj = null;
                }

                _isVertexHover = false;
            }
        }

        //CASE 4 - if Raycast did not hit any object
        else
            //update UI accordingly
            _uiManager.RaycastHitNothing();

    }

    //if user wants to place a point
    public void PlacePointButtonClicked()
    {
        //call placing method on ShapeManager
        _shapesManager.PlaceVertex(_hit.point);
    }

    //if user starts moving a vertex
    public void MoveVertexButtonPressed()
    {
        //call moving method on ShapeManager
        _moveVertexButtonPressed = true;
        Debug.Log("Move button PRESSED");
        if (_vertexHoverObj)
            _shapesManager.StartMovingVertex(_vertexHoverObj);
    }

    //if user finishes moving a vertext
    public void MoveVertexButtonReleased()
    {
        _moveVertexButtonPressed = false;
        Debug.Log("Move button RELEASED");
        // call stop moving method on ShapeManager
        if (_vertexHoverObj)
            _shapesManager.StopMovingVertex(_vertexHoverObj);
    }

    //if user wants to delete a vertex
    public void DeleteButtonClicked()
    {
        if (_vertexHoverObj)
            _shapesManager.DeleteVertex(_vertexHoverObj);
    }

    public void EndLineButtonClicked()
    {
        _shapesManager.EndLine();
    }

}
