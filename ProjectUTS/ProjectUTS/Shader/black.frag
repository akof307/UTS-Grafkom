#version 330

out vec4 outputColor;

uniform vec4 ourColor;

void main() {
    outputColor = vec4(ourColor);
}