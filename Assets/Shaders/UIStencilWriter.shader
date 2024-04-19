Shader "Custom/UIStencilWriter"
{
    Properties
    {
        _Stencil ("Stencil", Float) = 1
        _StencilOp ("Stencil Operation", Float) = 0 // Default value for stencil operation
        _StencilComp ("Stencil Comparison", Float) = 8 // Default value for stencil comparison
        _StencilReadMask ("Stencil Read Mask", Float) = 255 // Default value for stencil read mask
        // Add other properties as needed
    }

    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Stencil
        {
            Ref [_Stencil] // Set the stencil reference value
            Comp [_StencilComp] // Set the stencil comparison function
            Pass [_StencilOp] // Set the stencil operation
            ReadMask [_StencilReadMask] // Set the stencil read mask
        }
        CGPROGRAM
        #pragma surface surf Lambert

        #include "UnityCG.cginc"

        struct Input
        {
            float2 uv_MainTex;
            // Add other input attributes if needed
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Output white color with full opacity
            o.Albedo = fixed3(1, 1, 1); // White color
            o.Alpha = 1.0; // Full opacity
        }
        ENDCG
    }
}
