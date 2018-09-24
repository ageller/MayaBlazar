out vec3 lightDir, normal, eyeVec;
out vec2 texCoord0;

in vec3 uv_vertexAttrib;
in vec3 uv_normalAttrib;
in vec2 uv_texCoordAttrib0;

uniform vec4 uv_lightPos;

uniform mat4 uv_modelViewProjectionMatrix;

uniform mat4 uv_object2SceneMatrix;
uniform mat4 uv_scene2ObjectMatrix;
uniform vec4 uv_cameraPos;

void main(void)
{

	mat4 normalMatrix = transpose( uv_scene2ObjectMatrix );
	
	normal = (normalMatrix * vec4(uv_normalAttrib,0.0)).xyz;
	vec4 vertex = uv_modelViewProjectionMatrix* vec4(uv_vertexAttrib ,1.0);
	gl_Position = vertex;

	vec3 vVertex = vec3(uv_object2SceneMatrix* vec4(uv_vertexAttrib,1.0));
	vec3 tmpVec = normalize( (uv_object2SceneMatrix* uv_lightPos).xyz );

	//lightDir = tmpVec;
	lightDir = -1.*vVertex.xyz;
	eyeVec = uv_cameraPos.xyz-vVertex;

	texCoord0.s  = uv_texCoordAttrib0.s;
	texCoord0.t  = uv_texCoordAttrib0.t;
}
