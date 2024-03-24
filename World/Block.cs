
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Toryngine.World;

internal class Block
{
    public Vector3 position;
    public BlockType type;

    private Dictionary<Faces, FaceData> faces;

    public Dictionary<Faces, List<Vector2>> blockUV = new Dictionary<Faces, List<Vector2>>()
    {
        {Faces.FRONT, new List<Vector2>() },
        {Faces.BACK, new List<Vector2>() },
        {Faces.LEFT, new List<Vector2>() },
        {Faces.RIGHT, new List<Vector2>() },
        {Faces.TOP, new List<Vector2>() },
        {Faces.BOTTOM, new List<Vector2>() },
    };
    public Dictionary<Faces, List<Vector2>> GetUVsFromCoordinates(Dictionary<Faces, Vector2> coords) 
    {
        Dictionary<Faces, List<Vector2>> faceData = new Dictionary<Faces, List<Vector2>>();

        foreach(var faceCoord in coords)
        {
            faceData[faceCoord.Key] = new List<Vector2>()
            {
                new Vector2((faceCoord.Value.X+1f) / 16f, (faceCoord.Value.Y+1f) / 16f), // topright
                new Vector2(faceCoord.Value.X / 16f, (faceCoord.Value.Y+1f) / 16f), // topleft
                new Vector2(faceCoord.Value.X / 16f, faceCoord.Value.Y / 16f), // bottom left
                new Vector2((faceCoord.Value.X+1f) / 16f, faceCoord.Value.Y / 16f), // bottom right
            };
        }

        return faceData;
    }
    public Block(Vector3 position, BlockType blockType = BlockType.EMPTY) {
        this.type = blockType;
        this.position = position;

        if(blockType != BlockType.EMPTY)
        {
            blockUV = GetUVsFromCoordinates(TextureData.blockTypeUVCoord[blockType]);
        }

        faces = new Dictionary<Faces, FaceData>
        {
            {Faces.FRONT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.FRONT]),
                uv = blockUV[Faces.FRONT]
            } },
            {Faces.BACK, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BACK]),
                uv = blockUV[Faces.BACK]
            } },
            {Faces.LEFT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.LEFT]),
                uv = blockUV[Faces.LEFT]
            } },
            {Faces.RIGHT, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.RIGHT]),
                uv = blockUV[Faces.RIGHT]
            } },
            {Faces.TOP, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.TOP]),
                uv = blockUV[Faces.TOP]
            } },
            {Faces.BOTTOM, new FaceData{
                vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BOTTOM]),
                uv = blockUV[Faces.BOTTOM]
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