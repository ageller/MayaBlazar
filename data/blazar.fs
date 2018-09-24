
out vec4 FragColor; 

in vec3 LightDir, Normal, EyeVec;
in vec2 texCoord0;

uniform float uv_time;

uniform vec3 mixColor;

uniform vec3 uv_diffuseMtrl;
uniform vec3 uv_ambientMtrl;
uniform vec3 uv_emissiveMtrl;

uniform float uv_opacityMtrl;


void main(void)
{

	vec4 finalDiffuseColor = vec4(0.0);
	vec4 finalAmbientColor  = vec4(uv_ambientMtrl,1.0);
	vec4 finalSpecularColor = vec4(0.0);
	vec4 finalColor;


	vec3 N = normalize(Normal);
	vec3 L = normalize(LightDir);

	float shadow = 1.0;

	float lambertTerm = (dot(N,L))*shadow;

	if (lambertTerm > 0.0)
	{
		finalDiffuseColor = vec4(uv_diffuseMtrl,1);
		finalDiffuseColor *= lambertTerm;

	}

	finalColor.rgb = (finalAmbientColor + finalDiffuseColor + finalSpecularColor).rgb;

	finalColor.rgb += uv_emissiveMtrl;

	finalColor.a = uv_opacityMtrl;

	FragColor = finalColor;
	//FragColor = vec4(L, 1);

}
