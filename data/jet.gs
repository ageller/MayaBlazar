layout(triangles) in;
layout(triangle_strip, max_vertices=3) out;
in vec3 lightDir1[];
in vec3 lightDir2[];
in vec3 normal[];
in vec3 eyeVec[];
out vec3 LightDir1;
out vec3 LightDir2;
out vec3 Normal;
out vec3 EyeVec;


uniform mat4 uv_modelViewProjectionMatrix;
uniform mat4 uv_modelViewProjectionInverseMatrix;
uniform mat4 uv_modelViewMatrix;
uniform mat4 uv_projectionMatrix;
uniform mat4 uv_projectionInverseMatrix;
uniform mat4 uv_modelViewInverseMatrix;
uniform vec4 uv_cameraPos;
uniform mat4 uv_scene2ObjectMatrix;

uniform int uv_simulationtimeDays;
uniform float uv_simulationtimeSeconds;
uniform float uv_fade;

out vec4 vPos;
out vec2 texcoord;

void main()
{

	//take first position to get distance (wanted average of triangle, but couldn't make that work for some reason), and normalized by eye
	vPos =  uv_modelViewProjectionInverseMatrix*(gl_in[0].gl_Position);

	for (int i=0;i<3;i++) {
		vec4 vertexPos = uv_modelViewProjectionInverseMatrix*(gl_in[i].gl_Position);
		gl_Position =  uv_modelViewProjectionMatrix *vertexPos;
		texcoord = vertexPos.xy;
		
		LightDir1 = lightDir1[i];
		LightDir2 = lightDir2[i];

		Normal = normal[i];
		EyeVec = eyeVec[i];
		EmitVertex();
	}


	EndPrimitive();


}
