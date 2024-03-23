using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Toryngine.Graphics;
using Toryngine.World;

namespace Toryngine;

internal class Game : GameWindow
{
    Chunk chunk;
    ShaderProgram program;

    // camera
    Camera camera;

    // transfortmation variables
    float yRot = 0f;
    
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

        chunk = new Chunk(new Vector3(0,0,0));
        program = new ShaderProgram("Default.vert", "Default.frag");
        
        GL.Enable(EnableCap.DepthTest);

        GL.FrontFace(FrontFaceDirection.Cw);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);

        camera = new Camera(width, height, Vector3.Zero);
        CursorState = CursorState.Grabbed;
    }
    protected override void OnUnload()
    {
        base.OnUnload();
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        // GL.ClearColor(0.6f,0.3f,1f,1f);
        GL.ClearColor(0.2f,0.2f,0.2f,0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


        // transfromation matrices
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = camera.GetViewMatrix();
        Matrix4 projection = camera.GetProjectionMatrix();


        int modelLocation = GL.GetUniformLocation(program.ID, "model");
        int viewLocation = GL.GetUniformLocation(program.ID, "view");
        int projectionLocation = GL.GetUniformLocation(program.ID, "projection");

        GL.UniformMatrix4(modelLocation, true, ref model);
        GL.UniformMatrix4(viewLocation, true, ref view);
        GL.UniformMatrix4(projectionLocation, true, ref projection);

        chunk.Render(program);

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        MouseState mouse = MouseState;
        KeyboardState input = KeyboardState;

        base.OnUpdateFrame(args);
        camera.Update(input, mouse, args);
    }

    
}