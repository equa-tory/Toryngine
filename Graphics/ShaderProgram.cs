using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Toryngine.Graphics;

internal class ShaderProgram
{
    public int ID;
    public ShaderProgram(string vertexShaderFilePath, string fragmentShaderFilePath){
        // create the shader program
        ID = GL.CreateProgram();

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderFilePath));
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderFilePath));
        GL.CompileShader(fragmentShader);

        GL.AttachShader(ID, vertexShader);
        GL.AttachShader(ID, fragmentShader);

        GL.LinkProgram(ID);

        // delete the shaders
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }
    
    public void Bind() => GL.UseProgram(ID);
    public void Unbind() => GL.UseProgram(0);
    public void Delete() => GL.DeleteShader(ID);

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