using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Renders all points in an <see cref="ARPointCloud"/> as a <c>ParticleSystem</c>, persisting them all.
    /// </summary>
    [RequireComponent(typeof(ARPointCloud))]
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ARAllPointCloudPointsParticleVisualizer : MonoBehaviour
    {
        public enum Mode
        {
            /// <summary>
            /// Draw all the feature points from the start of the session
            /// </summary>
            All,

            /// <summary>
            /// Only draw the feature points from the current frame
            /// </summary>
            CurrentFrame,
        }

        [SerializeField]
        [Tooltip("Whether to draw all the feature points or only the ones from the current frame.")]
        Mode m_Mode;

        public Mode mode
        {
            get => m_Mode;
            set
            {
                m_Mode = value;
                RenderPoints();
            }
        }

        public int totalPointCount => m_Points.Count;

        void OnPointCloudChanged(ARPointCloudUpdatedEventArgs eventArgs)
        {
            RenderPoints();
        }

        void SetParticlePosition(int index, Vector3 position)
        {
            m_Particles[index].startColor = m_ParticleSystem.main.startColor.color;
            m_Particles[index].startSize = m_ParticleSystem.main.startSize.constant;
            m_Particles[index].position = position;
            m_Particles[index].remainingLifetime = 1f;
        }

        void RenderPoints()
        {
            if (!m_PointCloud.positions.HasValue)
                return;

            var positions = m_PointCloud.positions.Value;

            // Store all the positions over time associated with their unique identifiers
            if (m_PointCloud.identifiers.HasValue)
            {
                var identifiers = m_PointCloud.identifiers.Value;
                for (int i = 0; i < positions.Length; ++i)
                {
                    m_Points[identifiers[i]] = positions[i];
                }
            }
            m_Points = FilterPointCloud(m_Points);

            CreateBoundingBox(m_Points);

            // Make sure we have enough particles to store all the ones we want to draw
            int numParticles = (mode == Mode.All) ? m_Points.Count : positions.Length;
            if (m_Particles == null || m_Particles.Length < numParticles)
            {
                m_Particles = new ParticleSystem.Particle[numParticles];
            }

            switch (mode)
            {
                case Mode.All:
                    {
                        // Draw all the particles
                        int particleIndex = 0;
                        foreach (var kvp in m_Points)
                        {
                            SetParticlePosition(particleIndex++, kvp.Value);
                        }
                        break;
                    }
                case Mode.CurrentFrame:
                    {
                        // Only draw the particles in the current frame
                        for (int i = 0; i < positions.Length; ++i)
                        {
                            SetParticlePosition(i, positions[i]);
                        }
                        break;
                    }
            }

            // Remove any existing particles by setting remainingLifetime
            // to a negative value.
            for (int i = numParticles; i < m_NumParticles; ++i)
            {
                m_Particles[i].remainingLifetime = -1f;
            }

            m_ParticleSystem.SetParticles(m_Particles, Math.Max(numParticles, m_NumParticles));
            m_NumParticles = numParticles;
        }

        void Awake()
        {
            m_PointCloud = GetComponent<ARPointCloud>();
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        void OnEnable()
        {
            m_PointCloud.updated += OnPointCloudChanged;
            UpdateVisibility();
        }

        void OnDisable()
        {
            m_PointCloud.updated -= OnPointCloudChanged;
            UpdateVisibility();
        }

        void Update()
        {
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            SetVisible(enabled && (m_PointCloud.trackingState != TrackingState.None));
        }

        void SetVisible(bool visible)
        {
            if (m_ParticleSystem == null)
                return;

            var renderer = m_ParticleSystem.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = visible;
        }

        Dictionary<ulong, Vector3> FilterPointCloud(Dictionary<ulong, Vector3> points)
        {
            var filteredPoints = new Dictionary<ulong, Vector3>();

            // Convert the dictionary to a list of points for easier manipulation
            var pointList = new List<Vector3>(points.Values);

            // Loop through each point
            for (int i = 0; i < pointList.Count; i++)
            {
                var currentPoint = pointList[i];
                bool isEdgePoint = false;

                // Check the distances between this point and all other points
                for (int j = 0; j < pointList.Count; j++)
                {
                    // Skip if it's the same point
                    if (i == j) continue;

                    var otherPoint = pointList[j];

                    // Calculate the distance between the two points
                    var distance = Vector3.Distance(currentPoint, otherPoint);

                    // If the distance is small enough, this point is not on the edge
                    if (distance < 0.05f)
                    {
                        isEdgePoint = false;
                        break;
                    }

                    // If the distance is large enough, this point is on the edge
                    if (distance > 0.1f)
                    {
                        isEdgePoint = true;
                    }
                }

                // Add this point to the filtered list if it's an edge point
                if (isEdgePoint)
                {
                    var key = points.FirstOrDefault(x => x.Value == currentPoint).Key;
                    filteredPoints.Add(key, currentPoint);
                }
            }

            return filteredPoints;
        }

        void CreateBoundingBox(Dictionary<ulong, Vector3> filteredPointCloud)
        {
            // Calculate the min and max points of the filtered point cloud
            Vector3 minPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (Vector3 point in filteredPointCloud.Values)
            {
                if (point.x < minPoint.x)
                    minPoint.x = point.x;
                if (point.y < minPoint.y)
                    minPoint.y = point.y;
                if (point.z < minPoint.z)
                    minPoint.z = point.z;

                if (point.x > maxPoint.x)
                    maxPoint.x = point.x;
                if (point.y > maxPoint.y)
                    maxPoint.y = point.y;
                if (point.z > maxPoint.z)
                    maxPoint.z = point.z;
            }

            // Calculate the center point of the bounding box
            Vector3 center = (maxPoint + minPoint) / 2f;

            // Calculate the size of the bounding box
            Vector3 size = maxPoint - minPoint;

            // Create the bounding box
            GameObject boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boundingBox.transform.position = center;
            boundingBox.transform.localScale = size;
        }

        ARPointCloud m_PointCloud;

        ParticleSystem m_ParticleSystem;

        ParticleSystem.Particle[] m_Particles;

        int m_NumParticles;

        Dictionary<ulong, Vector3> m_Points = new Dictionary<ulong, Vector3>();
    }
}
