#version 330

uniform mat4 uModel;
uniform mat4 uWorld;
uniform mat4 uProjection;

uniform vec2 uFlipUV;

in vec3 vPosition;
in vec2 vTexCoords;

out vec2 oTexCoords;

void main()
{
    vec3 position = vPosition;
    if (uFlipUV.x == 1.0)
	{ 
	    position.x = 1.0 - position.x;
    }
	if (uFlipUV.y == 1.0) 
	{ 
	    position.y = 1.0 - position.y;
	}
	gl_Position = vec4(position, 1) * uModel * uWorld;
	gl_Position = gl_Position * uProjection;

	oTexCoords = vTexCoords;
}