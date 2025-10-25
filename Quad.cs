using UnityEngine;

public static class Quad
{
    private static Vector3[] vertices;
    private static Vector2[] uvs;
    private static int[] indices;
    
    private static Mesh mesh;

    private static GraphicsBuffer vertexBuffer;
    private static GraphicsBuffer indexBuffer;
    private static GraphicsBuffer uvBuffer;

    public static Mesh Mesh
    {
        get
        {
            if (!mesh)
            {
                mesh = new Mesh()
                {
                    vertices = Vertices,
                    triangles = Indices,
                    uv = UVs
                };
            }

            return mesh;
        }
    }
    
    public static Vector3[] Vertices
    {
        get
        {
            return vertices ??= new Vector3[]
            {
                new(-0.5f, -0.5f, 0),
                new(-0.5f, 0.5f, 0),
                new(0.5f, -0.5f, 0),
                new(0.5f, 0.5f, 0),
            };
        }
    }
    
    public static Vector2[] UVs
    {
        get
        {
            return uvs ??= new Vector2[]
            {
                new(0, 0),
                new(0, 1),
                new(1, 0),
                new(1, 1),
            };
        }
    }

    public static int[] Indices
    {
        get
        {
            return indices ??= new[]
            {
                0, 1, 2,
                2, 1, 3
            };
        }
    }

    public static GraphicsBuffer VertexBuffer
    {
        get
        {
            if (vertexBuffer == null)
            {
                vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4, sizeof(float) * 3);
                vertexBuffer.SetData(Vertices);
            }

            return vertexBuffer;
        }
    }
    
    public static GraphicsBuffer IndexBuffer
    {
        get
        {
            if (indexBuffer == null)
            {
                indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 6, sizeof(int));
                indexBuffer.SetData(Indices);
            }

            return indexBuffer;
        }
    }
    
    public static GraphicsBuffer UVBuffer
    {
        get
        {
            if (uvBuffer == null)
            {
                uvBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4, sizeof(float) * 2);
                uvBuffer.SetData(UVs);
            }

            return uvBuffer;
        }
    }

    public static void Dispose()
    {
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
        uvBuffer?.Dispose();
    }
}
