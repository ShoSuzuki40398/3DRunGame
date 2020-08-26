Shader "Custom/BlockNoise"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BlockNum("BlockNum",int) = 8
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		int _BlockNum;

		struct Input
		{
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float random(fixed2 p)
		{
			return frac(sin( dot( p , fixed2(12.9898,78.233))) * 43758.5453);
		}

		float noise(fixed2 st)
		{
			fixed2 p = floor(st);
			return random(p);
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			float c = noise(IN.uv_MainTex*_BlockNum);
			o.Albedo = fixed4(c, c, c, 1);
		}
		ENDCG
	}
		FallBack "Diffuse"
}
