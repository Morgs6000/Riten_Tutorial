#version 330 core
out vec4 FragColor;

in vec4 Color;
in vec2 TexCoord;

uniform vec4 uColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);

uniform bool useWireframe = false;
uniform bool useColor = false;
uniform bool useTexture = false;

uniform sampler2D texture0;

void processWireframe();
void processColor();
void processTexture();

vec4 result = uColor;

void main()
{
    processWireframe();
    processColor();
    processTexture();

    FragColor = result;
}

void processWireframe()
{
    if (useWireframe)
    {
        result = vec4(0.0f, 0.0f, 0.0f, 1.0f);
    }
}

void processColor()
{
    if (useColor)
    {
        result *= Color;
    }
}

void processTexture()
{
    if (useTexture)
    {
        result *= texture(texture0, TexCoord);
    }
}
