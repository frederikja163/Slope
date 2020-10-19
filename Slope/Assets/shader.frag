#version 330 core
in vec2 fTextureCoordinate;

out vec4 Color;

void main()
{
    float x = abs(fTextureCoordinate.x - int(fTextureCoordinate.x));
    float y = abs(fTextureCoordinate.y - int(fTextureCoordinate.y));
    if (x < 0.1f || x > 0.9f ||
        y < 0.1f || y > 0.9f)
    {
        Color = vec4(0, 1, 0.2f, 1);
    }
    else
    {
        Color = vec4(0, 0, 0, 1);
    }
}