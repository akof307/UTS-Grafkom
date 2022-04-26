#version 330

out vec4 outputColor;

in vec4 vertexColor;

uniform vec4 ourColor;
void main(){
	outputColor = vec4(0.99,0.61,0.26,1.0);
}