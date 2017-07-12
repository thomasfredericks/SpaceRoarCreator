Shader "Custom/VertexColor" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float4 vertexColor : COLOR;
		};

		void vert(inout appdata_full v, out Input o) {
			o.vertexColor = v.color;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			o.Albedo = IN.vertexColor.xyz;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0.0;
			o.Smoothness = 0.0;
			o.Alpha = 0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
