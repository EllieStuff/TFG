Shader "Unlit/VertexAnimPhong"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "" {}
		_alpha("Alpha", Range(0, 1)) = 1
		 _objectColor("Main color",Color) = (0,0,0,1)
		 _ambientInt("Ambient int", Range(0,1)) = 0.25
		 _ambientColor("Ambient Color", Color) = (0,0,0,1)
		 _materialQ("Material Q", Range(0, 1)) = 1

		 _diffuseInt("Diffuse int", Range(0,1)) = 1
		_scecularExp("Specular exponent",Float) = 2.0

		_pointLightPos("Point light Pos",Vector) = (0,0,0,1)
		_pointLightColor("Point light Color",Color) = (0,0,0,1)
		_pointLightIntensity("Point light Intensity",Float) = 1

		_directionalLightDir("Directional light Dir",Vector) = (0,1,0,1)
		_directionalLightColor("Directional light Color",Color) = (0,0,0,1)
		_directionalLightIntensity("Directional light Intensity",Float) = 1

		_speed("Speed", Float) = 5
		_frequency("Frequency", Float) = 2
		_range("Range", Float) = 0.02
		_ScrollXSpeed("Texture Scroll X", Range(0,10)) = 0
		_ScrollYSpeed("Texture Scroll Y", Range(0,10)) = 0

	}
		SubShader
		 {
			 Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
			 LOD 100

			 Pass
			 {
				 HLSLPROGRAM
				 #pragma vertex vert
				 #pragma fragment frag
				 #pragma multi_compile __ POINT_LIGHT_ON 
				 #pragma multi_compile __ DIRECTIONAL_LIGHT_ON
				 #include "UnityCG.cginc"

				 struct appdata
				 {
					 float4 vertex : POSITION;
					 float2 uv : TEXCOORD0;
					 float3 normal : NORMAL;
				 };

				 struct v2f
				 {
					 float2 uv : TEXCOORD0;
					 float4 vertex : SV_POSITION;
					 float3 worldNormal : TEXCOORD1;
					 float3 wPos : TEXCOORD2;
				 };


				 float _speed;
				 float _frequency;
				 float _range;
				 float _ScrollXSpeed;
				 float _ScrollYSpeed;

				 v2f vert(appdata v)
				 {
					 v2f o;
					 o.vertex = UnityObjectToClipPos(v.vertex);
					 o.uv = v.uv;
					 o.worldNormal = UnityObjectToWorldNormal(v.normal);
					 o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					 float timer = _Time.y * _speed;
					 float waver = _range * sin(timer + v.vertex.x * _frequency);
					 v.vertex.y = v.vertex.y + waver;
					 o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					 o.uv = v.uv;

					 return o;
				 }

				 fixed4 _objectColor;
				 float _alpha;

				 float _ambientInt;//How strong it is?
				 fixed4 _ambientColor;
				 float _diffuseInt;
				 float _scecularExp;

				 float4 _pointLightPos;
				 float4 _pointLightColor;
				 float _pointLightIntensity;

				 float4 _directionalLightDir;
				 float4 _directionalLightColor;
				 float _directionalLightIntensity;
				 float _materialQ;
				 Texture2D _MainTex;
				 SamplerState sampler_MainTex;

				 fixed4 frag(v2f i) : SV_Target
				 {
					float xScrollValue = _ScrollXSpeed * _Time;
					 float yScrollValue = _ScrollYSpeed * _Time;
					 fixed2 scrolledUV = i.uv;
					 scrolledUV += fixed2(xScrollValue, yScrollValue);

					 //3 phong model light components
					 //We assign color to the ambient term		
					 fixed4 ambientComp = _ambientColor * _ambientInt;//We calculate the ambient term based on intensity
					 fixed4 finalColor = ambientComp;

					 float3 viewVec;
					 float3 halfVec;
					 float3 difuseComp = float4(0, 0, 0, 1);
					 float3 specularComp = float4(0, 0, 0, 1);
					 float3 lightColor;
					 float3 lightDir;

					 //Directional light properties
					 lightColor = _directionalLightColor.xyz;
					 lightDir = normalize(_directionalLightDir);

					 //Diffuse componenet
					 difuseComp = lightColor * _diffuseInt * clamp(dot(lightDir, i.worldNormal),0,1);

					 //Specular component	
					 viewVec = normalize(_WorldSpaceCameraPos - i.wPos);

					 //Specular component
					 //phong
					 //float3 halfVec = reflect(-lightDir, i.worldNormal);
					 //fixed4 specularComp = lightColor * pow(clamp(dot(halfVec, viewVec),0,1), _scecularExp);

					 //blinnPhong
					 halfVec = normalize(viewVec + lightDir);
					 specularComp = lightColor * pow(max(dot(halfVec, i.worldNormal),0), _scecularExp);

					 //Sum
					 finalColor += clamp(float4(_directionalLightIntensity * (difuseComp + specularComp),1),0,1);

					 //Point light properties
					 lightColor = _pointLightColor.xyz;
					 lightDir = _pointLightPos - i.wPos;
					 float lightDist = length(lightDir);
					 lightDir = lightDir / lightDist;
					 //lightDir *= 4 * 3.14;

					 //Diffuse componenet
					 difuseComp = lightColor * _diffuseInt * clamp(dot(lightDir, i.worldNormal), 0, 1) / lightDist;

					 //Specular component	
					 viewVec = normalize(_WorldSpaceCameraPos - i.wPos);

					 //Specular component
					 //phong
					 //float3 halfVec = reflect(-lightDir, i.worldNormal);
					 //fixed4 specularComp = lightColor * pow(clamp(dot(halfVec, viewVec),0,1), _scecularExp);

					 //blinnPhong
					 halfVec = normalize(viewVec + lightDir);
					 specularComp = lightColor * pow(max(dot(halfVec, i.worldNormal), 0), _scecularExp) / lightDist;

					 //Sum
					 //finalColor += clamp(float4(_pointLightIntensity*(difuseComp + specularComp),1),0,1);

					 //float3 lightPoint = float3(0, 0, 0);
					 float alpha = dot(lightDir, i.worldNormal);
					 float alpha2 = alpha * alpha;
					 float PI = 3.14159265359;

					 float fresnel = _materialQ + (1 - _materialQ) * pow((1 - dot(lightDir, halfVec)), 5);
					 float geometry = dot(i.worldNormal, lightDir) * dot(i.worldNormal, viewVec);
					 float distribution = alpha2 / (PI * pow(pow(dot(i.worldNormal, halfVec), 2) * (alpha2 - 1) + 1, 2));
					 float BRDF = (fresnel * geometry * distribution) / (4 * dot(i.worldNormal, lightDir) * dot(i.worldNormal, viewVec));
					 float4 mainTexColor = _MainTex.Sample(sampler_MainTex, scrolledUV);
					 finalColor += clamp(float4(_pointLightIntensity * (difuseComp + BRDF) + mainTexColor, 1), 0, 1);
					 //finalColor += clamp(float4(_pointLightIntensity * (difuseComp + specularComp), 1), 0, 1);

					 finalColor.a = _alpha;

					 return finalColor * _objectColor;
				 }
				 ENDHLSL
			 }
		 }
}