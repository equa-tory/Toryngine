
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Toryngine.Graphics;
using Toryngine.World;

internal class Chunk
{
    private List<Vector3> chunkVerts;
    private List<Vector2> chunkUVs;
    private List<uint> chunkIndices;

    const int SIZE = 16;
    const int HEIGHT = 32;
    public Vector3 position;

    private uint indexCount;

    VAO chunkVAO;
    VBO chunkVertexVBO;
    VBO chunkUVVBO;
    IBO chunkIBO;

    Texture texture;

    public Block[,,] chunkBlocks = new Block[SIZE,HEIGHT,SIZE];

    public Chunk(Vector3 position) 
    {
        this.position = position;

        chunkVerts = new List<Vector3>();
        chunkUVs = new List<Vector2>();
        chunkIndices = new List<uint>();

        float[,] heightMap = GenChunk();
        GenBlocks(heightMap);
        GenFaces(heightMap);
        BuildChunk();
    }

    public float[,] GenChunk() { // generate the data
        float[,] heightMap = new float[SIZE,SIZE];
        
        SimplexNoise.Noise.Seed = 123456;
        for(int x=0;x<SIZE;x++)
        {
            for(int z=0;z<SIZE;z++)
            {
                heightMap[x,z] = SimplexNoise.Noise.CalcPixel2D(x,z,0.01f);
            }
        }
        
        return heightMap;
    } 
    public void GenBlocks(float[,] heightMap) // generate thre appropriate block faces given the data
    {
        for(int x=0;x<SIZE;x++)
        {
            for(int z=0;z<SIZE;z++)
            {
                int columnHeight = (int)(heightMap[x,z]/10);
                for(int y=0;y<HEIGHT;y++)
                {
                    BlockType type = BlockType.EMPTY;
                    if(y<columnHeight - 1)
                    {
                        type = BlockType.DIRT;
                    }
                    if(y == columnHeight - 1)
                    {
                        type = BlockType.GRASS;
                    }

                    chunkBlocks[x,y,z] = new Block(new Vector3(x,y,z), type);

                }
            }
        }
    }
    public void GenFaces(float[,] heightMap)
    {
        for(int x=0;x<SIZE;x++)
        {
            for(int z=0;z<SIZE;z++)
            {
                for(int y=0;y<HEIGHT;y++)
                {
                    // left faces
                    // qualifications: block to left is empty, is not farthest left in chunk
                    int numFaces = 0;

                    if(chunkBlocks[x,y,z].type != BlockType.EMPTY)
                    {
                        if (x > 0)
                        {
                            if(chunkBlocks[x-1,y,z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.LEFT);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.LEFT);
                            numFaces++;
                        }
                        // right faces
                        // qualifications: block to right is empty, is not farthest right in chunk
                        if (x < SIZE-1)
                        {
                            if(chunkBlocks[x+1,y,z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.RIGHT);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.RIGHT);
                            numFaces++;
                        }
                        // top faces
                        // qualifications: block to above is empty, is farthest up in chunk
                        if (y < HEIGHT - 1)
                        {
                            if(chunkBlocks[x,y+1,z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.TOP);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.TOP);
                            numFaces++;
                        }
                        // bottom faces
                        // qualifications: block to below is empty, is farthest down in chunk
                        if (y > 0)
                        {
                            if(chunkBlocks[x,y-1,z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.BOTTOM);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.BOTTOM);
                            numFaces++;
                        }
                        // front faces
                        if (z < SIZE-1)
                        {
                            if(chunkBlocks[x,y,z+1].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.FRONT);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.FRONT);
                            numFaces++;
                        }
                        // back faces
                        if (z > 0)
                        {
                            if(chunkBlocks[x,y,z-1].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x,y,z], Faces.BACK);
                                numFaces++;
                            }
                        } else
                        {
                            IntegrateFace(chunkBlocks[x,y,z], Faces.BACK);
                            numFaces++;
                        }
                        

                        AddIndices(numFaces);
                    }
                }
            }
        }
    }
    public void IntegrateFace(Block block, Faces face)
    {
        var faceData = block.GetFace(face);
        chunkVerts.AddRange(faceData.vertices);
        chunkUVs.AddRange(faceData.uv);

    }

    public void AddIndices(int amountFaces)
    {
        for(int i=0;i<amountFaces;i++)
        {
            chunkIndices.Add(0 + indexCount);
            chunkIndices.Add(1 + indexCount);
            chunkIndices.Add(2 + indexCount);
            chunkIndices.Add(2 + indexCount);
            chunkIndices.Add(3 + indexCount);
            chunkIndices.Add(0 + indexCount);

            indexCount += 4;
        }
    }
    public void BuildChunk() { // take data and process it for rendering
        chunkVAO = new VAO();
        chunkVAO.Bind();

        chunkVertexVBO = new VBO(chunkVerts);
        chunkVertexVBO.Bind();
        chunkVAO.LinkToVAO(0, 3, chunkVertexVBO);

        chunkUVVBO = new VBO(chunkUVs);
        chunkUVVBO.Bind();
        chunkVAO.LinkToVAO(1, 2, chunkUVVBO);

        chunkIBO = new IBO(chunkIndices);

        texture = new Texture("atlas.png");
    }
    public void Render(ShaderProgram program) // drawing the chunk
    {
        program.Bind();
        chunkVAO.Bind();
        chunkIBO.Bind();
        texture.Bind();
        GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void Delete()
    {
        chunkVAO.Delete();
        chunkVertexVBO.Delete();
        chunkUVVBO.Delete();
        chunkIBO.Delete();
        texture.Delete();
    }
}