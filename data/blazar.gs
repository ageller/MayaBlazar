layout(triangles) in;
layout(triangle_strip, max_vertices=3) out;
in vec3 lightDir[];
in vec3 normal[];
in vec3 eyeVec[];
out vec3 LightDir;
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
//uniform vec2 rainRange;



void main()
{

	for (int i=0;i<3;i++) {
		vec4 vertexPos = uv_modelViewProjectionInverseMatrix*(gl_in[i].gl_Position);
		gl_Position =  uv_modelViewProjectionMatrix *vertexPos;

		//LightDir=lightDir[i];
		//try a point light emanating from the center
		vec3 L = vec3(0,1,0);//gl_in[i].gl_Position.xyz;
		LightDir = normalize(-1*L);
		//LightDir = (uv_modelViewProjectionInverseMatrix*(vec4(0))).xyz;//-1.*normalize(gl_in[i].gl_Position.xyz);

		Normal=normal[i];
		EyeVec=eyeVec[i];
		EmitVertex();
	}
	EndPrimitive();
}
