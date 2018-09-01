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
	// float ave_z=0;
	// float top=rainRange.y;
	// float bottom =rainRange.x;
	// for (int i=0;i<3;i++) {
	// 	ave_z+=(uv_modelViewProjectionInverseMatrix*(gl_in[i].gl_Position)).z;
	// }
	// ave_z/=3;
 //    float shift = (bottom-top)*fract(0.3*uv_simulationtimeSeconds);
	// if (ave_z+shift < bottom) {
	// 	shift += (top-bottom);
	// }
	for (int i=0;i<3;i++) {
		LightDir=lightDir[i];
		Normal=normal[i];
		EyeVec=eyeVec[i];
		vec4 vertexPos = uv_modelViewProjectionInverseMatrix*(gl_in[i].gl_Position);
		// vertexPos.z+=shift;
		gl_Position =  uv_modelViewProjectionMatrix *vertexPos;
		EmitVertex();
	}
	EndPrimitive();
}
