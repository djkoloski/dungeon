#version 420

precision highp float;

in vec3 position;
in vec3 normal;
in vec3 color;
out vec4 fragColor;

void main(void)
{
	vec3 lightPos = vec3(30, 40, 50);
	float lambert = clamp(dot(normalize(lightPos - position), normalize(normal)), 0.0, 1.0);
	fragColor = vec4((lambert + 0.2) * color, 1.0);
}