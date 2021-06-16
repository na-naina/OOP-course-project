﻿#shader vertex
#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTexCoords;

out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0);
    TexCoords = aTexCoords;
}

#shader fragment
#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

const float offset = 1.0 / 600.0;


uniform sampler2D screenTexture;

void main()
{
    FragColor = vec4(vec3(texture(screenTexture, TexCoords)), 1.0);
}