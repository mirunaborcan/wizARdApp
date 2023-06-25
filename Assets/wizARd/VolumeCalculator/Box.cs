using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class Box : MonoBehaviour
{
    private GameObject _widthPoint;
    private GameObject _lengthPoint;
    private GameObject _centerPoint;
    private GameObject _heightPoint;

    private List<Vector3> _invisibleVerticesList;

    private LineRenderer _downBoxLineRenderer;
    private LineRenderer _upBoxLineRenderer;
    private List<LineRenderer> _edgesLineRenderers;

    private bool _textsAreCreated;
    private bool _heightVertexIsMoving;

    private GameObject _volumeText;

    private Vector3 _initialHeightVertexWorldPosition;
    private Vector3 _initialHeightVertexViewportPosition;
    private UIManager _uiManager;

    private static float _lastVolume;
    public TMP_Text _bottomVolumeText;

    // Update is called once per frame
    void Update()
    {
        if (_textsAreCreated)
            UpdateTexts();
    }


    public void Init(GameObject centerPoint, GameObject sidePoint)
    {
        this.transform.position = centerPoint.transform.position;

        InitVertices(centerPoint, sidePoint);

        _textsAreCreated = false;
        _heightVertexIsMoving = false;
        _uiManager = FindObjectOfType<UIManager>();

        CreateTexts();
        CreateLineRenderers();
        CreateInvisiblesVertices();
        Draw();
    }


    private void InitVertices(GameObject centerPoint, GameObject sidePoint)
    {
        //set points position
        _centerPoint = centerPoint;
        _widthPoint = sidePoint;

        //instantiates copies of _widthPoint for length and height
        _lengthPoint = Instantiate(_widthPoint);
        _heightPoint = Instantiate(_widthPoint);

        //set the type of vertex to box type
        _lengthPoint.GetComponent<VertexType>().ShapeType = VERTEX_SHAPE_TYPE.BOX;
        _heightPoint.GetComponent<VertexType>().ShapeType = VERTEX_SHAPE_TYPE.BOX;

        _widthPoint.transform.SetParent(this.transform);
        _centerPoint.transform.SetParent(this.transform);
        _heightPoint.transform.SetParent(this.transform);
        _lengthPoint.transform.SetParent(this.transform);

        //set position of lenght vertex
        PlaceLengthVertex();

        //heightPoint vertex is set to be above the _widthPoint vertex
        //it is positioned by adding a vertical offset equal to the distance between the _centerPoint and _widthPoint
        _heightPoint.transform.position = _widthPoint.transform.position + 
            new Vector3(0, Vector3.Distance(_centerPoint.transform.position, _widthPoint.transform.position), 0);
        _invisibleVerticesList = new List<Vector3>();

    }

    private void PlaceLengthVertex()
    {
        Vector3 centerPos = _centerPoint.transform.position;
        Vector3 sidePos = _widthPoint.transform.position;

        //calculates the direction vector from the side point to the center point
        Vector3 widthDirection = (centerPos - sidePos).normalized;

        //calculates the direction vector perpendicular to the width direction, which represents the length direction
        Vector3 lenghtDirection = (Quaternion.Euler(0, 90, 0) * widthDirection).normalized;

        //calculates the distance from the center point to the side point
        float demiSideLength = Vector3.Distance(centerPos, sidePos);

        _lengthPoint.transform.position = centerPos + demiSideLength * lenghtDirection;
    }


    private void CreateInvisiblesVertices()
    {
        for (int i = 0; i < 4; i++)
            _invisibleVerticesList.Add(Vector3.zero);
        
        //positions of bottom corners
        UpdateInvisibleVerticesPosition();
    }


    //Create all LineRenderers
    private void CreateLineRenderers()
    {
        _edgesLineRenderers = new List<LineRenderer>();
        _downBoxLineRenderer = CreateOneLineRenderer("DownBoxSideEdges");
        _upBoxLineRenderer = CreateOneLineRenderer("UpBoxSideEdges");

        for (int i = 0; i < 4; i++)
            _edgesLineRenderers.Add(CreateOneLineRenderer("SideEdge" + i));
    }

    //Create and set up only one LineRenderer
    private LineRenderer CreateOneLineRenderer(string gameObjectName)
    {
        LineRenderer lineModels = transform.GetComponent<LineRenderer>();

        LineRenderer lineToCreate = new GameObject(gameObjectName).AddComponent<LineRenderer>();
        lineToCreate.gameObject.transform.SetParent(this.transform);
        lineToCreate.useWorldSpace = true;
        lineToCreate.sharedMaterial = lineModels.sharedMaterial;
        lineToCreate.startWidth = lineToCreate.endWidth = lineModels.startWidth;
        lineToCreate.startColor = lineToCreate.endColor = lineModels.startColor;

        return lineToCreate;
    }

    private void CreateTexts()
    {
        if (!_uiManager)
            return;

        _volumeText = Instantiate(_uiManager.textSpecialeInfosPrefab, _uiManager.transform);

        _textsAreCreated = true;
    }

    private void UpdateTexts()
    {
        float width, length, height;

        width = Vector3.Distance(_centerPoint.transform.position, _lengthPoint.transform.position) * 2.0f;
        length = Vector3.Distance(_centerPoint.transform.position, _widthPoint.transform.position) * 2.0f;
        height = _heightPoint.transform.position.y - _centerPoint.transform.position.y;
        _lastVolume = (width * height * length);

        _volumeText.GetComponentInChildren<Text>().text = (width * height * length).ToString("#.######") + " m3";
        _volumeText.transform.position = Camera.main.WorldToScreenPoint(new Vector3(_centerPoint.transform.position.x, _heightPoint.transform.position.y, _centerPoint.transform.position.z)) + new Vector3(0,-100f,0);
        
    }

    public float getLastVolume()
    {
        UpdateTexts();
        Debug.Log($"Distance is:" + _lastVolume);
        return _lastVolume;
    }
    private void Draw()
    {
        UpdateInvisibleVerticesPosition();
        DrawEdges();
        DrawMesh();
        UpdateVertexScale();
    }

    //calculating bottom corners
    private void UpdateInvisibleVerticesPosition()
    {
        Vector3 centerPos = _centerPoint.transform.position;
        Vector3 widthPos = _widthPoint.transform.position;
        Vector3 lengthPos = _lengthPoint.transform.position;

        Vector3 widthDirection = (centerPos - widthPos).normalized;
        Vector3 lengthDirection = (centerPos - lengthPos).normalized;

        float demiWidth = Vector3.Distance(centerPos, widthPos);
        float demiLength = Vector3.Distance(centerPos, lengthPos);

        _invisibleVerticesList[0] = centerPos - (lengthDirection * demiLength) + (demiWidth * widthDirection);
        _invisibleVerticesList[1] = centerPos - (lengthDirection * demiLength) - (demiWidth * widthDirection);
        _invisibleVerticesList[2] = centerPos + (lengthDirection * demiLength) - (demiWidth * widthDirection);
        _invisibleVerticesList[3] = centerPos + (lengthDirection * demiLength) + (demiWidth * widthDirection);

    }

    private void DrawEdges()
    {
        _upBoxLineRenderer.positionCount = _downBoxLineRenderer.positionCount = _invisibleVerticesList.Count + 1;

        //draw edges of bottom basis and top basis - horizontal edges
        for (int i = 0; i < _invisibleVerticesList.Count + 1; i++)
        {
            Vector3 verticePosition = _invisibleVerticesList[i % _invisibleVerticesList.Count];
            _downBoxLineRenderer.SetPosition(i, verticePosition);
            
            //for the top corners, keep position on x and z, and replace y coordinate with position of height vertex
            _upBoxLineRenderer.SetPosition(i, new Vector3(verticePosition.x, _heightPoint.transform.position.y, verticePosition.z));
        }
        //draw the vertical edges 
        for(int i=0; i<4; i++)
        {
            _edgesLineRenderers[i].SetPosition(0, _downBoxLineRenderer.GetPosition(i));
            _edgesLineRenderers[i].SetPosition(1, _upBoxLineRenderer.GetPosition(i));
        }
    }


    private void DrawMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();

        foreach (Vector3 vertexPosition in _invisibleVerticesList)
            vertices.Add(transform.InverseTransformPoint(vertexPosition));
        
        foreach (Vector3 vertexPosition in _invisibleVerticesList)
            vertices.Add(transform.InverseTransformPoint(new Vector3(vertexPosition.x, _heightPoint.transform.position.y, vertexPosition.z)));



        List<int> triangles = new List<int> 
        { 
            0, 1, 2, //Back face
            0, 2, 3,
            2, 1, 0,
            3, 2, 0,
            4, 5, 6, //Top face
            4, 6, 7,
            6, 5, 4,
            7, 6, 4,
            0, 1, 5, //Right Face
            0, 5, 4,
            5, 1, 0,
            4, 5, 0,
            2, 3, 7, //Left Face
            2, 7, 6,
            7, 3, 2,
            6, 7, 2,
            1, 2, 6, //Back Face
            1, 6, 5,
            6, 2, 1, 
            5, 6, 1,
            3, 0, 7, //Front Face
            3, 7, 4,
            7, 0, 3,
            4, 7, 3
        };

        List<Vector3> normalsList = new List<Vector3>();

        for (int i = 0; i < vertices.Count; i++)
            normalsList.Add(transform.up);


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normalsList.ToArray();
    }


    private void UpdateVertexScale()
    {
        float demiWidthDistance = Vector3.Distance(_centerPoint.transform.position, _widthPoint.transform.position);
        float demiLengthDistance = Vector3.Distance(_centerPoint.transform.position, _lengthPoint.transform.position);

        float minDistance = Mathf.Min(demiWidthDistance, demiLengthDistance);

        _widthPoint.transform.localScale = 
        _lengthPoint.transform.localScale = 
        _heightPoint.transform.localScale = 
        _centerPoint.transform.localScale = (minDistance < 0.3f) ? 0.35f * minDistance * Vector3.one : 0.1f * Vector3.one;


        _downBoxLineRenderer.startWidth = _downBoxLineRenderer.endWidth =
        _upBoxLineRenderer.startWidth = _upBoxLineRenderer.endWidth = (minDistance < 0.2f) ? minDistance / 10.0f : 0.02f;

        foreach(LineRenderer line in _edgesLineRenderers)
            line.startWidth = line.endWidth = (minDistance < 0.2f) ? minDistance / 10.0f : 0.02f;

    }


    //One vertex can make the shape rotate, so we have to make other vertices follow the shape
    private void ReplaceVertexAfterOtherMoved(GameObject vertexMoved, GameObject vertexToMove)
    {
        float demiSideToMoveDistance = Vector3.Distance(vertexToMove.transform.position, _centerPoint.transform.position);

        //determine the rotation direction of the movement
        int rotationFactor = (vertexMoved == _widthPoint) ? 1 : -1;

        Vector3 sideMovedDirection = (_centerPoint.transform.position - vertexMoved.transform.position).normalized;
        //find the direction in which the vertexToMove will be moved
        Vector3 sideToMoveDirection = (Quaternion.Euler(0, 90 * rotationFactor, 0) * sideMovedDirection).normalized;

        //position the vertexToMove relative to the center point while maintaining the shape
        vertexToMove.transform.position = _centerPoint.transform.position + sideToMoveDirection * demiSideToMoveDistance;

        //update the position of the _heightPoint vertex by setting it to a new position
        _heightPoint.transform.position = new Vector3(_widthPoint.transform.position.x, 
                                                        _heightPoint.transform.position.y, 
                                                            _widthPoint.transform.position.z);
    }

    public void MoveVertex(GameObject vertexToMove, Vector3 position)
    {
        //take the box and move it in the plane
        if (vertexToMove == _centerPoint)
        {
            this.transform.position = position;
            _heightVertexIsMoving = false;
        }

        //change the height
        else if (vertexToMove == _heightPoint)
            MoveVerticalVertex();

        else
        {
            vertexToMove.transform.position = position;
            //change width
            if (vertexToMove == _widthPoint)
                ReplaceVertexAfterOtherMoved(_widthPoint, _lengthPoint);
            //change lenght 
            else ReplaceVertexAfterOtherMoved(_lengthPoint, _widthPoint);

            _heightVertexIsMoving = false;
        }

        Draw();
    }



    private void MoveVerticalVertex()
    {
        if (!_heightVertexIsMoving)
        {
            _initialHeightVertexWorldPosition = _heightPoint.transform.position;
            _initialHeightVertexViewportPosition = Camera.main.WorldToViewportPoint(_initialHeightVertexWorldPosition);
            _heightVertexIsMoving = true;
            return;
        }

        Vector3 currentPosition = _heightPoint.transform.position;

        //calculate the distance between the camera's position and the current vertex position 
        float distancePointCamera = Vector3.Distance(Camera.main.transform.position, currentPosition);

        Vector3 currentViewportPosition = Camera.main.WorldToViewportPoint(currentPosition);

        //calculate the new world position for the vertex based on the movement of the camera along the y-axis
        Vector3 newWorldPositionFromViewport = Camera.main.ViewportToWorldPoint(new Vector3(currentViewportPosition.x,
                                                                                            _initialHeightVertexViewportPosition.y, 
                                                                                            distancePointCamera));
        //Update the position of the _heightPoint vertex using the new world position calculated
        _heightPoint.transform.position = new Vector3(_initialHeightVertexWorldPosition.x,
                                                      newWorldPositionFromViewport.y, 
                                                      _initialHeightVertexWorldPosition.z);
    }



    public void Delete()
    {
        Destroy(_centerPoint);
        Destroy(_widthPoint);
        Destroy(_heightPoint);
        Destroy(_lengthPoint);
        Destroy(_downBoxLineRenderer.gameObject);
        Destroy(_upBoxLineRenderer.gameObject);
        Destroy(_volumeText);
        _lastVolume = 0;

        foreach (LineRenderer line in _edgesLineRenderers)
            Destroy(line.gameObject);

        Destroy(this.gameObject);
    }
}
