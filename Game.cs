using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StbImageSharp;

namespace Toryngine;

internal class Game : GameWindow
{

    float[] vertices = 
    {
        -0.5f, 0.5f, 0f, // top left - 0
        0.5f, 0.5f, 0f, // top rigsht - 1
        0.5f, -0.5f, 0f, // bottom right - 2
        -0.5f, -0.5f, 0f, // bottom left - 3
    };

    float[] texCoords = 
    {
        0f, 1f,
        1f, 1f,
        1f, 0f,
        0f, 0f
    };

    uint[] indices =
    {
        // top
        0, 1, 2,
        // bottom
        2, 3, 0
    };

    // OnRenderFrame Pipeline vars
    int vao;
    int vbo;
    int shaderProgram;
    int textureVBO;
    int ebo;
    int textureID;
    
    //CONSTANTS
    int width, height;

    public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        this.width = width;
        this.height = height;
        this.CenterWindow(new Vector2i(width, height));
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0,0, e.Width, e.Height);
        this.width = e.Width;
        this.height = e.Height;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // --- Vertices VBO ---
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length*sizeof(float), vertices, BufferUsageHint.StaticDraw);
        // GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        

        // bind the vao
        // GL.BindVertexArray(vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbinding vbo

        // --- Texture VBO ---

        textureVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length*sizeof(float), texCoords, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 1);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length*sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);



        // create the shader program
        shaderProgram = GL.CreateProgram();

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
        GL.CompileShader(fragmentShader);

        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);

        GL.LinkProgram(shaderProgram);

        // delete the shaders
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // --- TEXTURES ---
        textureID = GL.GenTexture();
        // activate texture in the unit
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, textureID);

        // texture params
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        // load image
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("./Textures/dirt.png"), ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
        // unbind the texture
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
    protected override void OnUnload()
    {
        base.OnUnload();

        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
        GL.DeleteTexture(textureID);
        GL.DeleteProgram(shaderProgram);
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        // GL.ClearColor(0.6f,0.3f,1f,1f);
        GL.ClearColor(0.2f,0.2f,0.2f,0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);


        // draw tringle
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        // GL.DrawArrays(PrimitiveType.Triangles, 0, 4);


        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
    }

    // Function to load a text file and return its contents as a string
    public static string LoadShaderSource(string filePath)
    {
        string shaderSource = "";

        try
        {
            using (StreamReader reader = new StreamReader("./Shaders/" + filePath))
            {
                shaderSource = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load shader source file: " + e.Message);
        }

        return shaderSource;
    }
}