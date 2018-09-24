out vec3 lightDir, normal, eyeVec;
out vec2 texCoord0;

#ifdef UV_NORMAL_TEXTURE
in vec3 uv_tangentAttrib;
in vec3 uv_bitangentAttrib;
#endif

#ifdef UV_DEFINE_SKELETAL_ANIMATION

uniform mat4 uv_boneMatrices[UV_BONE_MATRICES_SIZE];
in vec4 uv_weights;
in vec4 uv_boneIndices;

#endif

in vec3 uv_vertexAttrib;
in vec3 uv_normalAttrib;
in vec2 uv_texCoordAttrib0;

uniform vec4 uv_lightPos;
uniform float uv_time;
uniform bool uv_texFlipU;
uniform bool uv_texFlipV;

uniform mat4 uv_modelViewProjectionMatrix;

#ifdef UV_SHADOWS
uniform mat4 uv_shadowMatrix;
out vec3 shadowCoord;
#endif

uniform mat4 uv_object2SceneMatrix;
uniform mat4 uv_scene2ObjectMatrix;
uniform vec4 uv_cameraPos;

void main(void)
{

	mat4 normalMatrix = transpose( uv_scene2ObjectMatrix );
	
#ifdef UV_DEFINE_SKELETAL_ANIMATION

    vec4 newVertex = vec4(0,0,0,0);
    vec4 newNormal = vec4(0,0,0,0);
    int index;

    for (int i=0; i<4; i++)
    {
        index = int(uv_boneIndices[i]);
        newVertex += uv_boneMatrices[index] * vec4(uv_vertexAttrib,1.0) * uv_weights[i];
        newNormal += uv_boneMatrices[index] * vec4(uv_normalAttrib,0.0) * uv_weights[i];
    }

    normal = (normalMatrix * vec4(newNormal.xyz,0.0)).xyz;
    gl_Position = uv_modelViewProjectionMatrix* vec4(newVertex.xyz ,1.0);

#else
    normal = (normalMatrix * vec4(uv_normalAttrib,0.0)).xyz;
    gl_Position = uv_modelViewProjectionMatrix* vec4(uv_vertexAttrib ,1.0);
#endif

#ifdef UV_SHADOWS
    vec4 shadow_pos = (uv_shadowMatrix * vec4(uv_vertexAttrib,1.0));
    shadowCoord.xy = (shadow_pos.xy + vec2(1.0,1.0)) * vec2(0.5,0.5);
    shadowCoord.z = ( (shadow_pos.z/shadow_pos.w) + 1.0)*0.5;
#endif

    vec3 vVertex = vec3(uv_object2SceneMatrix* vec4(uv_vertexAttrib,1.0));
    vec3 tmpVec = normalize( (uv_object2SceneMatrix* uv_lightPos).xyz );

#ifndef UV_NORMAL_TEXTURE

    lightDir = tmpVec;
    eyeVec = uv_cameraPos.xyz-vVertex;

#else

    vec3 n = normalize( (normalMatrix * vec4(uv_normalAttrib,0.0)).xyz );


#ifdef UV_MESH_HAS_TANGENTS
    vec3 t = normalize( (normalMatrix  * vec4(uv_tangentAttrib,0.0)).xyz);
#else
    vec3 t = cross(n,vec3(0,1,0) );
#endif

#ifdef UV_MESH_HAS_BINORMALS
    vec3 b = normalize( (normalMatrix  * vec4(uv_bitangentAttrib,0.0)).xyz);
#else
    vec3 b = cross(n,t);
#endif

    mat3 tangentMatrix = transpose(mat3(t, b, n));

    lightDir = tangentMatrix * tmpVec;
    eyeVec = tangentMatrix * (uv_cameraPos.xyz-vVertex);

#endif

    if (uv_texFlipU)
        texCoord0.s  = 1 - uv_texCoordAttrib0.s;
    else
        texCoord0.s  = uv_texCoordAttrib0.s;

    if (uv_texFlipV)
        texCoord0.t  = 1 - uv_texCoordAttrib0.t;
    else
        texCoord0.t  = uv_texCoordAttrib0.t;
}