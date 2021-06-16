#shader vertex
#version 330 core
layout(location = 0) in vec3  i_pos;
layout(location = 1) in vec4  i_Color;
layout(location = 2) in vec2  i_TexCoord;
layout(location = 3) in float i_TexIndex;

uniform vec3 offset;
uniform mat4 u_ViewProj;

out vec4 v_Color;
out vec2 v_TexCoord;
out float v_TexIndex;

void main()
{
    v_Color = i_Color;
    v_TexCoord = i_TexCoord;
    v_TexIndex = i_TexIndex;
    gl_Position = u_ViewProj * vec4(i_pos + offset, 1.0);
}

#shader fragment
#version 330 core
out vec4 result;

in vec4 v_Color;
in vec2 v_TexCoord;
in float v_TexIndex;

uniform sampler2D u_Textures[32];

void main()
{
    int index = int(v_TexIndex);
    result = texture(u_Textures[index], v_TexCoord) * v_Color;
}