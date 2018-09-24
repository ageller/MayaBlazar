
out vec4 FragColor; 

in vec3 LightDir1, LightDir2, Normal, EyeVec;
in vec2 texCoord0;

in vec4 vPos;

uniform float uv_time;
uniform int uv_simulationtimeDays;
uniform float uv_simulationtimeSeconds;

uniform vec3 uv_diffuseMtrl;
uniform vec3 uv_ambientMtrl;
uniform vec3 uv_emissiveMtrl;

uniform float uv_opacityMtrl;

uniform sampler2D cmap;

const float PI = 3.141592653589793;

void main(void)
{
	float dist = length(vPos.xz)/10.; 


	vec3 cm = texture(cmap ,vec2(clamp(dist,0.,1),0.5)).rgb;

	vec4 finalDiffuseColor1 = vec4(0.0);
	vec4 finalDiffuseColor2 = vec4(0.0);
	vec4 finalAmbientColor  = vec4(cm,1.0)*0.5;
	vec4 finalSpecularColor = vec4(0.0);
	vec4 finalColor;


	vec3 N = normalize(Normal);
	vec3 L1 = normalize(LightDir1);
	vec3 L2 = normalize(LightDir2);

	float shadow = 1.0;

	float lambertTerm1 = (dot(N,L1))*shadow;
	float lambertTerm2 = (dot(N,L2))*shadow*0.02;

	//point light from center
	if (lambertTerm1 > 0.0)
	{
		finalDiffuseColor1 = vec4(uv_diffuseMtrl,1);
		finalDiffuseColor1 *= lambertTerm1;
	}
	//additional diffuse light from other direction
	if (lambertTerm2 > 0.0)
	{
		finalDiffuseColor2 = vec4(uv_diffuseMtrl,1);
		finalDiffuseColor2 *= lambertTerm2;
	}
	finalColor.rgb = (finalAmbientColor + finalDiffuseColor1 + finalDiffuseColor2 + finalSpecularColor).rgb;

	finalColor.rgb += uv_emissiveMtrl;

	finalColor.a = uv_opacityMtrl;

	FragColor = finalColor;

	//attempt to simulate keplerian motion
	
	//define the time 
	//THIS IS NOT WORKING VERY WELL HERE BECAUE OF LIMITTED VERTICES.  I SHOULD PROBABLY CREATE A FLAT BILLBOARD AND DRAW ON TOP OF THIS WITH THE KEPLERIAN MOTION
	float dayfract = uv_simulationtimeSeconds/(24.0*3600.0);
	float days = uv_simulationtimeDays + dayfract;
	float years = 1970. + days/365.2425;

	float angle = atan(vPos.z, vPos.x) + PI;
	float amin = mod(days/pow(dist*10., 2./3.), 2.*PI) ;
	float amax = amin + 0.2;
	if (angle >= amin && angle <= amax){
		FragColor = vec4(0,0,0,1);
	}

}
