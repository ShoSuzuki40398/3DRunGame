Shader "Custom/RimLighting"
{
	Properties{
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (0, 0, 1, 1)
		_Tickness("Tickness",Range(1.0,10.0)) = 1.5
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _BaseColor;
		fixed4 _RimColor;
		float _Tickness;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _BaseColor.rgb;
			float rim = 1 - saturate(dot(IN.viewDir,o.Normal));
			o.Emission = _RimColor * pow(rim, _Tickness);
		}
		ENDCG
	}
		FallBack "Diffuse"
}
