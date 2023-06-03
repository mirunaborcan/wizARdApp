using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Choosing Shape Type")]
    public GameObject _shapesPanel;
    public Button _lineButton;
    public Button _circleButton;
    public Button _squareButton;
    public Button _triangleButton;
    public Button _pentagoneButton;
    public Button _boxButton;
    public Image _selectedCircle;

    [Header("Shape Interaction Button")]
    public Button _placePointButton;
    public Button _movePointButton;
    public Button _deleteVertexButton;
    public Button _endLineButton;

    [Header("Utility")]
    public ShapesManager _shapesManager;

    [Header("Distance, Surface and Volume Texts Prefabs")]
    public GameObject textInfosPrefab;
    public GameObject textSpecialeInfosPrefab;
    
    //this script contains references to various UI elements,
    //such as buttons, images, and panels, as well as a reference to the ShapesManager

    // Start is called before the first frame update
    void Start()
    {
        _shapesPanel.SetActive(false);
        _placePointButton.gameObject.SetActive(false);
        _movePointButton.gameObject.SetActive(false);
        _deleteVertexButton.gameObject.SetActive(false);
        _endLineButton.gameObject.SetActive(false);
        CircleButtonClicked();
    }

    //choose wanted shape clicking the respective buttons
    public void LineButtonClicked()
    {
        _selectedCircle.transform.position = _lineButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.LINE);
    }

    public void CircleButtonClicked()
    {
        _selectedCircle.transform.position = _circleButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.CIRCLE);
    }

    public void TriangleButtonClicked()
    {
        _selectedCircle.transform.position = _triangleButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.TRIANGLE);

    }

    public void SquareButtonClicked()
    {
        _selectedCircle.transform.position = _squareButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.SQUARE);
    }


    public void PentagoneButtonClicked()
    {
        _selectedCircle.transform.position = _pentagoneButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.POLYGON);
    }

    public void BoxButtonClicked()
    {
        _selectedCircle.transform.position = _boxButton.transform.position;
        _shapesManager.ChangeWantedShape(WANTED_SHAPE.BOX);
    }

    //this toggles the visibility of the shapes panel
    public void MenuButtonClicked()
    {
        _shapesPanel.SetActive(!_shapesPanel.activeSelf);
    }


    // this handles UI updates when the raycast hits an ARPlane
    // it shows/hides the appropriate buttons based on whether the user can move a vertex or end a line
    public void RaycastHitPlane(bool showMoveButton)
    {
        _deleteVertexButton.gameObject.SetActive(false);
        _movePointButton.gameObject.SetActive(showMoveButton);
        _placePointButton.gameObject.SetActive(true);

        _endLineButton.gameObject.SetActive(_shapesManager.UserCanEndLine());
    }

    // this handles UI updates when the raycast hits a vertex
    // it shows/hides the appropriate buttons for moving and deleting the vertex
    public void RaycastHitVertex()
    {
        _movePointButton.gameObject.SetActive(true);
        _deleteVertexButton.gameObject.SetActive(true);
        _endLineButton.gameObject.SetActive(false);
    }

    // this handles UI updates when the raycast doesn't hit any object
    // it hides all interaction buttons.
    public void RaycastHitNothing()
    {
        _placePointButton.gameObject.SetActive(false);
        _movePointButton.gameObject.SetActive(false);
        _deleteVertexButton.gameObject.SetActive(false);
        _endLineButton.gameObject.SetActive(false);
    }
}
