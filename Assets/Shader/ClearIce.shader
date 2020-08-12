Shader "Custom/ClearIce"
{
	Properties{
		_BaseColor("Base Color", Color) = (1,1,1,1)
		_Tickness("Tickness",Range(1.0,10.0)) = 1.5
	}

		SubShader
	{
		Tags { "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _BaseColor;
		float _Tickness;

        struct Input
        {
            float3 worldNormal;
			float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			o.Albedo = _BaseColor.rgb;
			o.Alpha = 1;
			// ビューベクトルとモデルの法線ベクトルで内積をとると、中央から輪郭の重みがとれる
			float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
            o.Alpha = alpha * _Tickness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
