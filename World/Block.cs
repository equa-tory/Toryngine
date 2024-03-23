
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Toryngine.World;

internal class Block
{
    public Vector3 position;

    private Dictionary<Faces, FaceData> faces;

    List<Vector2> dirtUv = new List<Vector2>
    {
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(0f, 0f),
    };

    public Block(Vector3 position) {
        this.position = position;

        faces = new Dictionary<Faces, FaceData>
        {
            {Faces.FRONT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.FRONT]),
                uv = dirtUv
            } },
            {Faces.BACK, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BACK]),
                uv = dirtUv
            } },
            {Faces.LEFT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.LEFT]),
                uv = dirtUv
            } },
            {Faces.RIGHT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.RIGHT]),
                uv = dirtUv
            } },
            {Faces.TOP, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.TOP]),
                uv = dirtUv
            } },
            {Faces.BOTTOM, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BOTTOM]),
                uv = dirtUv
            } },
            
        };
    }
    
    public List<Vector3> AddTransformedVertices(List<Vector3> vertices) {
        List<Vector3> transformedVertices = new List<Vector3>();
        foreach(var vert in vertices){
            transformedVertices.Add(vert + position);
        }
        return transformedVertices;
    } 
    public FaceData GetFace(Faces face) {
        return faces[face];
    } 
}