using System.Numerics;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Riten_Tutorial.src;

public class Program
{
    private static Glfw _glfw = Glfw.GetApi();
    private static GL _gl = GL.GetApi(_glfw.GetProcAddress);

    public static GL GL = _gl;

    public static ShadingMode ShadingMode = ShadingMode.Shaded;

    private static bool[] _keys = new bool[1024];
    private static bool[] _keysProcessed = new bool[1024];

    // configurações
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    // câmera
    private static Vector3 _cameraPos = new Vector3(10.0f, 15.0f, 30.0f);
    private static Vector3 _cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
    private static Vector3 _cameraUp = new Vector3(0.0f, 1.0f, 0.0f);

    private static bool _firstMouse = true;
    private static float _yaw   = -90.0f; // yaw is initialized to -90.0 degrees since a yaw of 0.0 results in a direction vector pointing to the right so we initially rotate a bit to the left.
    private static float _pitch =  -20.0f;
    private static float _lastX =  SCR_WIDTH / 2.0f;
    private static float _lastY =  SCR_HEIGHT / 2.0f;
    private static float _fov   =  45.0f;

    // tempo
    private static float _deltaTime = 0.0f; // tempo entre o quadro atual e o quadro anterior
    private static float _lastFrame = 0.0f;

    private static unsafe void Main(string[] args)
    {
        // glfw: inicializar e configurar
        // --------------------------------------------------
        _glfw.Init();
        _glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        _glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        _glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

        if (OperatingSystem.IsMacOS())
        {
            _glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
        }

        // criação da janela glfw
        // --------------------------------------------------
        WindowHandle* window = _glfw.CreateWindow(SCR_WIDTH, SCR_HEIGHT, "Riten Tutorial", null, null);

        if (window == null)
        {
            Console.WriteLine("Falha ao criar a janela GLFW");
            _glfw.Terminate();
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            // Obtém o tamanho da janela passado para glfwCreateWindow
            _glfw.GetWindowSize(window, out int pWidth, out int pHeight);

            // Obtém a resolução do monitor principal
            VideoMode* videoMode = _glfw.GetVideoMode(_glfw.GetPrimaryMonitor());

            // Centralizar a janela
            _glfw.SetWindowPos(
                window,
                (videoMode->Width - pWidth) / 2,
                (videoMode->Height - pHeight) / 2
            );
        }

        _glfw.MakeContextCurrent(window);
        _glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);
        _glfw.SetCursorPosCallback(window, CursorPosCallback);
        _glfw.SetScrollCallback(window, ScrollCallback);
        _glfw.SetKeyCallback(window, KeyCallback);

        // Ativar v-sync
        _glfw.SwapInterval(1);

        // instruir o GLFW a capturar o mouse
        _glfw.SetInputMode(window, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);

        // configurar estado global do OpenGL
        // --------------------------------------------------
        _gl.Enable(EnableCap.DepthTest);
        _gl.Enable(EnableCap.CullFace);

        // construir e compilar nosso programa de shader
        // --------------------------------------------------
        Shader shader = new Shader(
            "res/shaders/shader/vertex.glsl",
            "res/shaders/shader/fragment.glsl"
        );

        // carregar e criar uma textura
        // --------------------------------------------------
        uint texture;

        _gl.GenTextures(1, out texture);
        _gl.BindTexture(TextureTarget.Texture2D, texture); // todas as operações GL_TEXTURE_2D subsequentes agora afetam este objeto de textura

        // define os parâmetros de repetição da textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o modo de repetição da textura como GL_REPEAT (método padrão)
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // definir parâmetros de filtragem de textura
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        // carregar imagem, criar textura e gerar mipmaps
        int width, height;
        byte[] data;

        using (FileStream stream = File.OpenRead("res/textures/terrain2.png"))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.Default);

            width = image.Width;
            height = image.Height;
            data = image.Data;
        }

        if (data != null)
        {
            fixed (byte* ptr = data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        else
        {
            Console.WriteLine("Falha ao carregar a textura");
        }

        BlockList blockList = new BlockList();
        blockList.Awake();

        // Chunk chunk = new Chunk();
        // chunk.Start();

        PlayerController playerController = new PlayerController();

        // loop de renderização
        // --------------------------------------------------
        while (!_glfw.WindowShouldClose(window))
        {
            // lógica de tempo por quadro
            // --------------------------------------------------
            float currentFrame = (float)_glfw.GetTime();
            _deltaTime = currentFrame - _lastFrame;
            _lastFrame = currentFrame;

            // input
            // --------------------------------------------------
            ProcessInput(window);

            // render
            // --------------------------------------------------
            _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
                fieldOfView:       MathHelper.DegreesToRadians(_fov), 
                aspectRatio:       (float)SCR_WIDTH / (float)SCR_HEIGHT, 
                nearPlaneDistance: 0.03f, 
                farPlaneDistance:  1000.0f
            );
            shader.SetMat4("projection", projection);

            Matrix4x4 view = Matrix4x4.CreateLookAt(
                cameraPosition: _cameraPos, 
                cameraTarget:   _cameraPos + _cameraFront, 
                cameraUpVector: _cameraUp
            );
            shader.SetMat4("view", view);

            Matrix4x4 model = Matrix4x4.Identity;
            shader.SetMat4("model", model);

            // vincular textura
            _gl.BindTexture(TextureTarget.Texture2D, texture);

            // chunk.Draw(shader);

            playerController.Position = _cameraPos;
            playerController.Update(shader);

            Array.Copy(_keys, _keysProcessed, _keys.Length);

            // glfw: troca os buffers e processa eventos de E/S (teclas pressionadas/liberadas, movimento do mouse, etc.)
            // --------------------------------------------------
            _glfw.SwapBuffers(window);
            _glfw.PollEvents();
        }

        // opcional: desalocar todos os recursos assim que não forem mais necessários:
        // --------------------------------------------------
        shader.Dispose();
        // chunk.Dispose();

        // glfw: encerra, liberando todos os recursos do GLFW alocados anteriormente.
        // --------------------------------------------------
        _glfw.Terminate();
        return;
    }

    // processar toda a entrada: consultar a GLFW para saber se teclas relevantes foram pressionadas ou liberadas neste quadro e reagir de acordo
    // --------------------------------------------------
    private static unsafe void ProcessInput(WindowHandle* window)
    {
        if (GetKeyDown(Keys.Escape))
        {
            _glfw.SetWindowShouldClose(window, true);
        }

        if (GetKey(Keys.F3))
        {
            if (GetKeyDown(Keys.W))
            {
                switch (ShadingMode)
                {
                    case ShadingMode.Shaded:
                        ShadingMode = ShadingMode.Shaded_Wireframe;
                        break;
                    case ShadingMode.Shaded_Wireframe:
                        ShadingMode = ShadingMode.Wireframe;
                        break;
                    case ShadingMode.Wireframe:
                        ShadingMode = ShadingMode.Shaded;
                        break;
                }
            }
        }
        else
        {
            float cameraSpeed = 21.58f * _deltaTime;

            Vector3 front = Vector3.Normalize(new Vector3(_cameraFront.X, 0.0f, _cameraFront.Z));
            Vector3 right = Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp));
            Vector3 up    = _cameraUp;

            if (GetKey(Keys.W))
            {
                _cameraPos += cameraSpeed * front;
            }
            if (GetKey(Keys.S))
            {
                _cameraPos -= cameraSpeed * front;
            }
            if (GetKey(Keys.A))
            {
                _cameraPos -= cameraSpeed * right;
            }
            if (GetKey(Keys.D))
            {
                _cameraPos += cameraSpeed * right;
            }
            if (GetKey(Keys.Space))
            {
                _cameraPos += cameraSpeed * up;
            }
            if (GetKey(Keys.ShiftLeft))
            {
                _cameraPos -= cameraSpeed * up;
            }

            if (GetKeyDown(Keys.R))
            {
                _fov = 60.0f;
            }
        }
    }

    // glfw: sempre que o tamanho da janela é alterado (pelo SO ou por redimensionamento do usuário), esta função de callback é executada
    // --------------------------------------------------
    private static unsafe void FramebufferSizeCallback(WindowHandle* window, int width, int height)
    {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que a largura e
        // a altura serão significativamente maiores do que as especificadas em telas Retina.
        _gl.Viewport(0, 0, (uint)width, (uint)height);
    }

    // glfw: sempre que o mouse se move, este callback é chamado
    // --------------------------------------------------
    private static unsafe void CursorPosCallback(WindowHandle* window, double x, double y)
    {
        float xpos = (float)x;
        float ypos = (float)y;

        if (_firstMouse)
        {
            _lastX = xpos;
            _lastY = ypos;

            _firstMouse = false;
        }

        float xoffset = xpos - _lastX;
        float yoffset = _lastY - ypos; // invertido, já que as coordenadas y vão de baixo para cima
        _lastX = xpos;
        _lastY = ypos;

        float sensitivity = 0.1f; // altere este valor conforme sua preferência
        xoffset *= sensitivity;
        yoffset *= sensitivity;

        _yaw += xoffset;
        _pitch += yoffset;

        // certifique-se de que a tela não seja invertida quando o pitch estiver fora dos limites
        _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);

        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
        front.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

        _cameraFront = Vector3.Normalize(front);
    }

    // glfw: sempre que a roda de rolagem do mouse é girada, este callback é chamado
    // --------------------------------------------------
    private static unsafe void ScrollCallback(WindowHandle* window, double offsetX, double offsetY)
    {
        _fov -= (float)offsetY;

        _fov = Math.Clamp(_fov, 1.0f, 180.0f);
    }

    private static unsafe void KeyCallback(WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods)
    {
        if (action == InputAction.Press)
        {
            _keys[(int)key] = true;
        }
        else if (action == InputAction.Release)
        {
            _keys[(int)key] = false;
        }
    }

    public static bool GetKey(Keys key)
    {
        return _keys[(int)key];
    }

    public static bool GetKeyDown(Keys key)
    {
        return _keys[(int)key] && !_keysProcessed[(int)key];
    }

    public static bool GetKeyUp(Keys key)
    {
        return !_keys[(int)key] && _keysProcessed[(int)key];
    }
}
