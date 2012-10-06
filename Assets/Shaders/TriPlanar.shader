Shader "Custom/TriPlanar" {
	Properties {
		_XTexture ("X", 2D) = "white" {}
		_XBumpMap ("X Normals", 2D) = "bump" {}
		_YTexture ("Y", 2D) = "white" {}
		_YBumpMap ("Y Normals", 2D) = "bump" {}
		_ZTexture ("Z", 2D) = "white" {}
		_ZBumpMap ("Z Normals", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert
		
		float4 _XTexture_ST;
		float4 _YTexture_ST;
		float4 _ZTexture_ST;
		sampler2D _XTexture, _YTexture, _ZTexture;
		sampler2D _XBumpMap, _YBumpMap, _ZBumpMap;

		struct Input {
			float3 norm;
			float2 xv;
			float2 yv;
			float2 zv;
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.xv = TRANSFORM_TEX(v.vertex.zy, _XTexture);
			o.yv = TRANSFORM_TEX(v.vertex.zx, _YTexture);
			o.zv = TRANSFORM_TEX(v.vertex.xy, _YTexture);
			o.norm = v.normal;
			
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 xCol = tex2D(_XTexture, IN.xv);
			fixed4 yCol = tex2D(_YTexture, IN.yv);
			fixed4 zCol = tex2D(_ZTexture, IN.zv);

			//float  f = abs(IN.worldNormal.x) + abs(IN.worldNormal.y) + abs(IN.worldNormal.z);
			float3 norm = normalize(abs(IN.norm));
			 
			fixed4 output = xCol*norm.x*norm.x+yCol*norm.y*norm.y+zCol*norm.z*norm.z;
			//fixed4 output = xCol*norm.x+yCol*norm.y+zCol*norm.z;
			
			o.Albedo = output.rgb;
			o.Alpha = output.a;
			o.Normal = 
					UnpackNormal(tex2D(_XBumpMap, IN.xv))*norm.x*norm.x+
					UnpackNormal(tex2D(_YBumpMap, IN.yv))*norm.y*norm.y+
					UnpackNormal(tex2D(_ZBumpMap, IN.zv))*norm.z*norm.z;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}