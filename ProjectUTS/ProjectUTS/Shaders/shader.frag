#version 330

out vec4 outputColor;

in vec4 vertexColor;

uniform vec4 ourColor;

void main(){
	//outputColor = vec4(0.3, 0.5, 0.6, 1.0); 
	outputColor = ourColor;
}