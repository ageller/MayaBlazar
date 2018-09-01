
out vec4 FragColor; 

in vec3 LightDir, Normal, EyeVec;
in vec2 texCoord0;

uniform float uv_time;

uniform vec3 mixColor;

#ifdef UV_DIFFUSE_TEXTURE
uniform sampler2D uv_diffuseTexture;
uniform float uv_diffuseTexStrength;
uniform int uv_diffuseTexOp;
#endif

#ifdef UV_AMBIENT_TEXTURE
uniform sampler2D uv_ambientTexture;
uniform float uv_ambientTexStrength;
uniform int uv_ambientTexOp;
#endif

#ifdef  UV_SPECULAR_TEXTURE
uniform sampler2D uv_specularTexture;
uniform float uv_specularTexStrength;
uniform int uv_specularTexOp;
#endif

#ifdef UV_NORMAL_TEXTURE
uniform sampler2D uv_normalTexture;
#endif


#ifdef UV_EMISSIVE_TEXTURE
uniform sampler2D uv_emissiveTexture;
uniform float uv_emissiveTexStrength;
#endif

#ifdef UV_LIGHTMAP_TEXTURE
uniform sampler2D uv_lightmapTexture;
uniform float uv_lightmapTexStrength;
#endif

#ifdef UV_OPACITY_TEXTURE
uniform sampler2D uv_opacityTexture;
uniform float uv_opacityTexStrength;
#endif


#ifdef UV_REFLECTION_TEXTURE
uniform sampler2D uv_reflectionTexture;
uniform float uv_reflectionTexStrength;
uniform int uv_reflectionTexOp;
#endif

uniform vec3 uv_diffuseMtrl;
uniform vec3 uv_ambientMtrl;
uniform vec3 uv_emissiveMtrl;


#ifdef UV_SPECULAR_MATERIAL
uniform vec3 uv_specularMtrl;
uniform float uv_shininessMtrl;
#endif

uniform float uv_opacityMtrl;

#ifdef UV_REFLECTION_TEXTURE
vec2 SphereMap(in vec3 ecPosition3, in vec3 normal)
{
    float m;
    vec3 r, u;
    u = normalize(ecPosition3);
    r = reflect(u, normal);
    m = 2.0 * sqrt(r.x * r.x + r.y * r.y + (r.z + 1.0) * (r.z + 1.0));
    return vec2(r.x / m + 0.5, r.y / m + 0.5);
}
#endif

#ifdef UV_SHADOWS
uniform sampler2D uv_shadowMap;
uniform float uv_shadowBias;     // depth offset to shadow compare
uniform vec2  uv_shadowOffset;   // half a shadowmap pixel offset

in vec3 shadowCoord;     // shadow map uv and depth in z
float sampleShadow( vec2 coord )
{
    if ( shadowCoord.z > (texture2D(uv_shadowMap, coord).x + uv_shadowBias ) )
    {
        return 1.0;
    }
    return 0.0;
}

#ifndef UV_SHADOWS_LOW
#define UV_PCF_SHADOW
#endif

// Returns a float of the amount of shadow (0-1) for this fragment
float eval_shadow()
{
#ifdef UV_PCF_SHADOW
    return (sampleShadow( shadowCoord.xy + uv_shadowOffset * vec2(0.0,0.0)) +
            sampleShadow(shadowCoord.xy + uv_shadowOffset * vec2(2.0,2.0)) +
            sampleShadow(shadowCoord.xy + uv_shadowOffset * vec2(2.0,-2.0)) +
            sampleShadow(shadowCoord.xy + uv_shadowOffset * vec2(-2.0,-2.0)) )*0.25;
#else
    return sampleShadow(shadowCoord.xy + uv_shadowOffset);
#endif
}
#endif

vec4 applyOperation(vec4 T1, vec4 T2, int op)
{

    vec4 T = vec4(0,0,0,0);

    if (op == 0)
        T = T1 * T2;
    else if (op == 1)
        T = T1 + T2;

    else if (op == 2)
        T = T1 - T2;

    else if (op == 3)
    {
        /*
          if (T2.r != 0.0 && T2.g != 0.0 && T2.b != 0.0)
          {

          T.r = T1.r / T2.r;
          T.g = T1.g / T2.g;
          T.b = T1.b / T2.b;

          }
        */
    }

    else if (op == 4)
        T = (T1 + T2) - (T1*T2);
    else
        T = T1 + (T2 - 0.5);


    return T;
}

void main(void)
{

    vec4 finalDiffuseColor = vec4(0.0);
    vec4 finalAmbientColor  = vec4(uv_ambientMtrl,1.0);
    vec4 finalSpecularColor = vec4(0.0);
    vec4 finalColor;


#ifdef UV_AMBIENT_TEXTURE
    finalAmbientColor = uv_ambientTexStrength * applyOperation(finalAmbientColor,texture2D(uv_ambientTexture,texCoord0.st),uv_ambientTexOp);
#endif


#ifdef UV_NORMAL_TEXTURE
    vec3 bump = normalize(texture2D(uv_normalTexture,texCoord0.st).rgb * 2.0 - 1.0);
    vec3 N = bump;
#else
    vec3 N = normalize(Normal);
#endif

    vec3 L = normalize(LightDir);

#ifdef UV_SHADOWS
    float shadow = 1.0 - eval_shadow();
#else
    float shadow = 1.0;
#endif
    float lambertTerm = (dot(N,L))*shadow;
    float specular;


    if (lambertTerm > 0.0)
    {
        finalDiffuseColor = vec4(uv_diffuseMtrl,1);

#ifdef UV_DIFFUSE_TEXTURE
        finalDiffuseColor = uv_diffuseTexStrength * applyOperation(finalDiffuseColor,texture2D(uv_diffuseTexture,texCoord0.st),uv_diffuseTexOp);

#endif

        finalDiffuseColor *= lambertTerm;

#ifdef UV_SPECULAR_MATERIAL
        vec3 E = normalize(EyeVec);
        vec3 R = reflect(-L,N);
        specular = pow( max(dot(R,E), 0.0) , uv_shininessMtrl );



        finalSpecularColor = vec4(uv_specularMtrl,1);

#ifdef  UV_SPECULAR_TEXTURE
        finalSpecularColor = uv_specularTexStrength *                   applyOperation(finalSpecularColor,texture2D					(uv_specularTexture,texCoord0.st),uv_specularTexOp);
#endif
        finalSpecularColor *= specular;
#endif
    }

    finalColor.rgb = (finalAmbientColor + finalDiffuseColor + finalSpecularColor).rgb;



#ifdef UV_EMISSIVE_TEXTURE
    finalColor.rgb += uv_emissiveTexStrength * texture2D(uv_emissiveTexture, texCoord0.st).rgb;
#else
    finalColor.rgb += uv_emissiveMtrl;
#endif

#ifdef UV_LIGHTMAP_TEXTURE
    finalColor.rgb *= texture2D(uv_lightmapTexture, texCoord0.st).rgb * uv_lightmapTexStrength;
#endif

#ifdef UV_REFLECTION_TEXTURE
    finalColor = uv_reflectionTexStrength * applyOperation(finalColor,texture2D(uv_reflectionTexture,SphereMap(-EyeVec,N)),uv_reflectionTexOp);
#endif


#ifdef UV_OPACITY_TEXTURE
    finalColor.a = uv_opacityTexStrength * texture2D(uv_opacityTexture, texCoord0.st).UV_OPACITY_TEXTURE_REGISTER_MASK;
#else
    finalColor.a = uv_opacityMtrl;
#endif

    FragColor = finalColor;
    //FragColor = vec4(L, 1);

}
