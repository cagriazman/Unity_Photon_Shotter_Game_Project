using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class AiSensor : MonoBehaviour
{
    public float distance = 10f;
    public float angle = 30f;
    public float height = 1f;
    public int segments = 10; 
    public Color meshColor = Color.white;
    public float scanFrequency = 30f;
    public LayerMask layerMask;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    private int count;
    private float scanInterval;
    private float scanTimer;
    private Mesh mesh;

    private void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layerMask,
            QueryTriggerInteraction.Collide);

        Objects.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (InSight(obj))
            {
                Objects.Add(obj);
            }
        }
    }

    public bool InSight(GameObject obj)
    {
            Vector3 originUpdate = transform.forward * 0.4f;
            Vector3 origin = transform.position+originUpdate;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;

            if (direction.y < 0 || direction.y > height)
            {
                return false;
            }

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);
            if (deltaAngle > angle)
            {
                return false;
            }

            origin.y += height / 2;
            dest.y = origin.y;
            if (Physics.Linecast(origin, dest, occlusionLayers))
            {
                return false;
            }
            
            return true;
        }

        Mesh CreateWedgeMesh()
        {
            if (mesh == null) mesh = new Mesh();
            else mesh.Clear();


            int totalTriangles = (segments * 4) + 4;
            int numVertices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 topCenter = bottomCenter + Vector3.up * height;

            int vert = 0;
            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;


            Vector3 firstBottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            Vector3 firstTopLeft = firstBottomLeft + Vector3.up * height;

            vertices[vert++] = bottomCenter;
            vertices[vert++] = firstBottomLeft;
            vertices[vert++] = firstTopLeft;

            vertices[vert++] = firstTopLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            for (int i = 0; i < segments; i++)
            {
                Vector3 bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                Vector3 bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                Vector3 topLeft = bottomLeft + Vector3.up * height;
                Vector3 topRight = bottomRight + Vector3.up * height;

                // Ön Duvar 1
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                // Ön Duvar 2
                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                // Üst Kapak
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                // Alt Kapak
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }


            Vector3 lastBottomRight = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            Vector3 lastTopRight = lastBottomRight + Vector3.up * height;

            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = lastTopRight;

            vertices[vert++] = lastTopRight;
            vertices[vert++] = lastBottomRight;
            vertices[vert++] = bottomCenter;

            for (int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate()
        {
            mesh = CreateWedgeMesh();
            scanInterval = 1.0f / scanFrequency;

        }

        private void OnDrawGizmos()
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position+transform.forward*0.4f, transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position, distance);
            
        }

    
}