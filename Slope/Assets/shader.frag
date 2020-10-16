#version 330 core
in vec3 fTextureCoordinate;

out vec4 Color;

void main()
{
    Color = vec4(fTextureCoordinate, 1);
}