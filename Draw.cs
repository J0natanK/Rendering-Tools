using System;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    private static List<RenderData> renderData = new(); 
    private static List<IndirectRenderData> indirectRenderData = new(); 
    
    public static void Circle(Vector2 position, float radius, Color color)
    {
        Material mat = new(Shader.Find("Unlit/CircleShader"));
        mat.color = color;
        
        renderData.Add(new RenderData()
        {
            mesh = Quad.Mesh,
            renderParams = new RenderParams(mat),
            matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * radius * 2)
        });
    }

    public static void CircleIndirect(Matrix4x4[] matrices, Color[] colors)
    {
        if (matrices.Length != colors.Length)
            throw new Exception("Matrices and colors must be of the same size.");
        
        // Draw args
        GraphicsBuffer.IndirectDrawIndexedArgs drawArgs = new()
        {
            indexCountPerInstance = 6,
            instanceCount = (uint)matrices.Length,
            startIndex = 0,
            baseVertexIndex = 0,
            startInstance = 0
        };
        
        GraphicsBuffer drawArgsBuffer = 
            new(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        drawArgsBuffer.SetData(new[] { drawArgs });
        
        // Buffers
        ComputeBuffer matrixBuffer = new(matrices.Length, 64);
        matrixBuffer.SetData(matrices);

        ComputeBuffer colorBuffer = new(colors.Length, sizeof(float) * 4);
        colorBuffer.SetData(colors);
        
        // Render params
        Material material = new(Shader.Find("Unlit/CircleShaderIndirect"));

        RenderParams rp = new(material)
        {
            worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one),
            matProps = new MaterialPropertyBlock()
        };
            
        rp.matProps.SetBuffer("vertices", Quad.VertexBuffer);
        rp.matProps.SetBuffer("uvs", Quad.UVBuffer);
        rp.matProps.SetBuffer("objectToWorldBuffer", matrixBuffer);
        rp.matProps.SetBuffer("colors", colorBuffer);
        
        indirectRenderData.Add(new IndirectRenderData()
        {
            renderParams = rp,
            drawArgsBuffer = drawArgsBuffer,
            indexBuffer = Quad.IndexBuffer,
            buffersToDispose = new[] {matrixBuffer, colorBuffer}
        });
    }

    public static void ClearRenderData()
    {
        renderData.Clear();
    }
    
    public static void ClearIndirectRenderData()
    {
        foreach (IndirectRenderData data in indirectRenderData)
        {
            foreach (ComputeBuffer buffer in data.buffersToDispose)
            {
                buffer.Dispose();
            }
            
            data.drawArgsBuffer.Dispose();
        }
        
        indirectRenderData.Clear();
    }

    private void Update()
    {
        foreach (RenderData data in renderData)
        {
            Graphics.RenderMesh(data.renderParams, data.mesh, 0, data.matrix);
        }

        foreach (IndirectRenderData data in indirectRenderData)
        {
            Graphics.RenderPrimitivesIndexedIndirect(
                data.renderParams, MeshTopology.Triangles, data.indexBuffer, data.drawArgsBuffer);
        }
    }

    private void OnDestroy()
    {
        ClearIndirectRenderData();
        Quad.Dispose();
    }

    private struct RenderData
    {
        public Mesh mesh;
        public RenderParams renderParams;
        public Matrix4x4 matrix;
    }

    private struct IndirectRenderData
    {
        public RenderParams renderParams;
        public GraphicsBuffer drawArgsBuffer;
        public GraphicsBuffer indexBuffer;
        public ComputeBuffer[] buffersToDispose;
    }
}
