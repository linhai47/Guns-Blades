using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
[RequireComponent(typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
    private Rigidbody2D rbPlayer;

    [Header("Springs")]
    [SerializeField] private float spriteConstant = 1.4f;
    [SerializeField] private float damping = 1.1f;
    [SerializeField] private float spread = 6.5f;
    [SerializeField, Range(1, 10)] private int wavePropogationIterations = 8;
    [SerializeField, Range(0f, 20f)] private float speedMult = 5.5f;
    [SerializeField] private float buoyancyMultiplier = .2f;
    [Header("Force")]
    public float ForceMultiplier = .2f;
    [Range(1f, 50f)] public float MaxForce = 5f;

    [Header("Collision")]
    [SerializeField, Range(1f, 10f)] private float playerCollisionRadiusMult = 4.15f;



    [Header("Mesh Generation")]
    [Range(2, 500)] public int NumOfVertives = 70;
    public float Width = 10f;
    public float Height = 4f;
    public Material WaterMaterial;
    private const int NUM_OF_Y_VERTICES = 2;

    [Header("Gizmo")]
    public Color GizmoColor = Color.white;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Vector3[] vertices;
    private int[] topVerticesIndex;

    private EdgeCollider2D coll;


    private class WaterPoint
    {
        public float velocity, acceleration, pos, targetHeight;
    }
    private List<WaterPoint> waterPoints = new List<WaterPoint>();

    private void Start()
    {
        coll = GetComponent<EdgeCollider2D>();
        rbPlayer = Player.instance.rb;
        GenerateMesh();
        CreateWaterPoints();
    }

    private void Reset()
    {
        coll = GetComponent<EdgeCollider2D>();
        rbPlayer = Player.instance.rb;
        coll.isTrigger = true;
    }

    private void FixedUpdate()
    {
        if(rbPlayer == null)
        {
            rbPlayer = Player.instance.rb;
        }
        if (rbPlayer != null)
        {
            float waterSurfaceY = GetWaterSurfaceYAt(rbPlayer.position);

            float playerBottomY = rbPlayer.position.y - rbPlayer.GetComponent<Collider2D>().bounds.extents.y;

            if (rbPlayer != null && IsPlayerInWater(rbPlayer)) // 人物在水中
            {
                float depth = waterSurfaceY - playerBottomY;
                float maxBuoyancy = 20f;
                float buoyancyForce = Mathf.Min(depth * buoyancyMultiplier, maxBuoyancy);

                rbPlayer.AddForce(Vector2.up * buoyancyForce, ForceMode2D.Force);
              
            }
        }


        for (int i = 1; i< waterPoints.Count-1; i++)
        {
            WaterPoint point = waterPoints[i];
            float x = point.pos - point.targetHeight;
            float acceleration = -spriteConstant * x - damping*point.velocity;
            point.pos += point.velocity * speedMult * Time.fixedDeltaTime;
            vertices[topVerticesIndex[i]].y = point.pos;
            point.velocity += acceleration * speedMult * Time.fixedDeltaTime;

        }

        for(int j = 0; j < wavePropogationIterations; j++)
        {
            for(int  i=1; i< waterPoints.Count-1; i++)
            {
                float leftDelta = spread * (waterPoints[i].pos - waterPoints[i - 1].pos) * speedMult * Time.fixedDeltaTime;
                waterPoints[i - 1].velocity += leftDelta;
                float rightDelta = spread * (waterPoints[i].pos - waterPoints[i + 1].pos) * speedMult * Time.fixedDeltaTime;
                waterPoints[i + 1].velocity += rightDelta;
            }

           
        }
        mesh.vertices = vertices;
    }

    public void Splash(Collider2D collision, float force)
    {
        float radius = collision.bounds.extents.x * playerCollisionRadiusMult;
        Vector2 center = collision.transform.position;

        for(int i = 0; i < waterPoints.Count; i++)
        {
            Vector2 vertexWorldPos = transform.TransformPoint(vertices[topVerticesIndex[i]]);
    
            if(IsPointInsideCircle(vertexWorldPos, center, radius))
            {
                waterPoints[i].velocity = force;
            }
        }
    }

    private bool IsPointInsideCircle(Vector2 point, Vector2 center , float radius)
    {
        float distanceSquared = (point - center).sqrMagnitude;
        return distanceSquared < radius * radius;
    }

    private bool IsPlayerInWater(Rigidbody2D rb)
    {
        Vector2 playerPos = rb.position;
        Collider2D playerCollider = rb.GetComponent<Collider2D>();

        // 获取玩家水平范围
        float playerLeft = playerPos.x - playerCollider.bounds.extents.x;
        float playerRight = playerPos.x + playerCollider.bounds.extents.x;

        // 获取水体水平范围
        float waterLeft = transform.position.x - Width / 2f;
        float waterRight = transform.position.x + Width / 2f;

        // 水面高度
        float waterSurfaceY = GetWaterSurfaceYAt(playerPos);

        // 玩家底部
        float playerBottomY = playerPos.y - playerCollider.bounds.extents.y;

        // 同时满足水平范围和底部在水面以下
        bool insideX = playerRight > waterLeft && playerLeft < waterRight;
        bool belowSurface = playerBottomY < waterSurfaceY;

        return insideX && belowSurface;
    }
    public float GetWaterSurfaceYAt(Vector2 worldPos)
    {
        // 转成 local space
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        // 找到对应的水顶点索引
        float step = Width / (NumOfVertives - 1);
        int index = Mathf.Clamp(Mathf.RoundToInt((localPos.x + Width / 2) / step), 0, NumOfVertives - 1);

        return transform.TransformPoint(vertices[topVerticesIndex[index]]).y;
    }

    public void ResetEdgeCollider()
    {
        coll = GetComponent<EdgeCollider2D>();
        Vector2[] newPoints = new Vector2[2];
        Vector2 firstPoint = new Vector2(vertices[topVerticesIndex[0]].x, vertices[topVerticesIndex[0]].y);
        newPoints[0] = firstPoint;

        Vector2 secondPoint = new Vector2(vertices[topVerticesIndex[topVerticesIndex.Length - 1]].x, vertices[topVerticesIndex[topVerticesIndex.Length - 1]].y);
        newPoints[1] = secondPoint;
        coll.offset = Vector2.zero;
        coll.points = newPoints;

    }


    public void GenerateMesh()
    {
        mesh = new Mesh();
        vertices = new Vector3[NumOfVertives * NUM_OF_Y_VERTICES];
        topVerticesIndex = new int[NumOfVertives];

        for (int y = 0; y < NUM_OF_Y_VERTICES; y++)
        {
            for (int x = 0; x < NumOfVertives; x++)
            {
                float xPos = (x / (float)(NumOfVertives - 1)) * Width - Width / 2;
                float yPos = (y / (float)(NUM_OF_Y_VERTICES - 1)) * Height - Height / 2;
                vertices[y * NumOfVertives + x] = new Vector3(xPos, yPos, 0f);
                if (y == NUM_OF_Y_VERTICES - 1)
                {
                    topVerticesIndex[x] = y * NumOfVertives + x;
                }
            }
        }

        // 构成三角
        int[] triangles = new int[(NumOfVertives - 1) * (NUM_OF_Y_VERTICES - 1) * 6];
        int index = 0;
        for (int y = 0; y < NUM_OF_Y_VERTICES - 1; y++)
        {
            for (int x = 0; x < NumOfVertives - 1; x++)
            {
                int bottomLeft = y * NumOfVertives + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + NumOfVertives;
                int topRight = topLeft + 1;

                // first triangle 
                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft;
                triangles[index++] = bottomRight;

                // second triangle
                triangles[index++] = bottomRight;
                triangles[index++] = topLeft;
                triangles[index++] = topRight;
            }
        }
        // UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2((vertices[i].x + Width / 2) / Width, (vertices[i].y + Height / 2) / Height);

        }
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        meshRenderer.material = WaterMaterial;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    private void CreateWaterPoints()
    {
        waterPoints.Clear();
        for(int i = 0;i<topVerticesIndex.Length;i++)
        {
            waterPoints.Add(new WaterPoint
            {
                pos = vertices[topVerticesIndex[i]].y,
                targetHeight = vertices[topVerticesIndex[i]].y,
            });
        }

    }

}

[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
    private InteractableWater water;
    private void OnEnable()
    {
        water = (InteractableWater)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        root.Add(new VisualElement { style = { height = 10 } });

        Button generateMeshButton = new Button(() => water.GenerateMesh())
        {
            text = "Generate Mesh"
        };

        root.Add(generateMeshButton);
        Button placeEdgeColliderButton = new Button(() => water.ResetEdgeCollider())
        {
            text = "Place Edge Collider"
        };
        root.Add(placeEdgeColliderButton);

        return root;
    }

    private void ChangeDimensions(ref float width, ref float height, float calculatedWidthMax, float calculatedHeightMax)
    {
        width = Mathf.Max(0.1f, calculatedWidthMax);
        height = Mathf.Max(0.1f, calculatedHeightMax);
    }


    private void OnSceneGUI()
    {
        // Draw the wireframeBox

        Handles.color = water.GizmoColor;
        Vector3 center = water.transform.position;
        Vector3 size = new Vector3(water.Width, water.Height, .1f);
        Handles.DrawWireCube(center, size);

        // Handles for width and Height;
        float handleSize = HandleUtility.GetHandleSize(center) * .1f;
        Vector3 snap = Vector3.one * .1f;


        // Cornor handles
        Vector3[] corners = new Vector3[4];
        corners[0] = center + new Vector3(-water.Width / 2, -water.Height / 2, 0);
        corners[1] = center + new Vector3(water.Width / 2, -water.Height / 2, 0);
        corners[2] = center + new Vector3(-water.Width / 2, water.Height / 2, 0);
        corners[3] = center + new Vector3(water.Width / 2, water.Height / 2, 0);
        // Handle for each corner
        EditorGUI.BeginChangeCheck();
        Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water.Width, ref water.Height, corners[1].x - newBottomLeft.x, corners[3].y - newBottomLeft.y);

            water.transform.position += new Vector3((newBottomLeft.x - corners[0].x) / 2, (newBottomLeft.y - corners[0].y) / 2, 0);
        }
        EditorGUI.BeginChangeCheck();
        Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], handleSize, snap, Handles.CubeHandleCap);
        if(EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water.Width, ref water.Height, newBottomRight.x - corners[0].x, corners[3].y - newBottomRight.y);
            water.transform.position += new Vector3((newBottomRight.x - corners[1].x) / 2, (newBottomRight.y - corners[1].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water.Width, ref water.Height,
                corners[3].x - newTopLeft.x,
                newTopLeft.y - corners[0].y);
            water.transform.position += new Vector3(
                (newTopLeft.x - corners[2].x) / 2,
                (newTopLeft.y - corners[2].y) / 2,
                0);
        }
        EditorGUI.BeginChangeCheck();
        Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water.Width, ref water.Height,
                newTopRight.x - corners[2].x,
                newTopRight.y - corners[1].y);
            water.transform.position += new Vector3(
                (newTopRight.x - corners[3].x) / 2,
                (newTopRight.y - corners[3].y) / 2,
                0);
        }
        // Update the mesh if the handles are moved
        if(GUI.changed)
        {
            water.GenerateMesh();
        }
    }

}

