#shader vertex
#version 330 core
layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 texCoord;

out vec2 v_TexCoords;

uniform vec3 offset;
uniform mat4 ViewProjection;

void main()
{
    gl_Position = ViewProjection * vec4(pos + offset, 1.0);
    v_TexCoords = texCoord;
}

#shader fragment
#version 330 core
out vec4 result;

in vec2 v_TexCoords;

uniform vec3 color;
uniform sampler2D Tex;

void main()
{
    
    result = texture(Tex, v_TexCoords);
} 