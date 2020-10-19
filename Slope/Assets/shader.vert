#version 330 core
in layout(location = 0) vec3 vPosition;
in layout(location = 1) vec3 vNormal;
in layout(location = 2) vec2 vTextureCoordinate;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uPerspective;

out vec2 fTextureCoordinate;

void main()
{
    gl_Position = uPerspective * uView * uModel * vec4(vPosition, 1);
    fTextureCoordinate = vTextureCoordinate;
}