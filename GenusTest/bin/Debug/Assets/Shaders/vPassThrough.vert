#version 330

uniform mat4 uModel;
uniform mat4 uWorld;
uniform mat4 uProjection;

in vec3 vPosition;
in vec2 vTexCoords;

out vec2 oTexCoords;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel * uWorld * uProjection;
	oTexCoords = vTexCoords;
}