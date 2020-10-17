#version 330 core
in vec2 fTextureCoordinate;

out vec4 Color;

void main()
{
    if ((fTextureCoordinate.x - int(fTextureCoordinate.x)) < 0.2f ||
        (fTextureCoordinate.y - int(fTextureCoordinate.y)) < 0.2f)
    {
        Color = vec4(1, 1, 1, 1);
    }
    else
    {
        Color = vec4(0, 0, 0, 1);
    }
}