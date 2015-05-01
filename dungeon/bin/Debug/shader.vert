#version 420

uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

in vec3 vPosition;
in vec3 vNormal;
in vec3 vColor;
out vec3 position;
out vec3 normal;
out vec3 color;

void main(void)
{
	position = vPosition;
	normal = vNormal;
	color = vColor;
	gl_Position = projectionMatrix * modelViewMatrix * vec4(vPosition, 1.0);
}