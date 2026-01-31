using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    public class RaindropFX_GPUPass : CustomPostProcessingPass<RaindropFX_GPU> {
        public RaindropFX_GPUPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader) { }

        protected override string RenderTag => "RaindropFX_GPU";

        #region parameters
        Material SIM_Mat;
        Material BLEND_Mat;
        Material BLUR_Mat;

        int clock = 0;
        bool initFlag = false;
        bool swapBuffer = true;
        bool forceBakeFlag = true;

        RenderTexture[] _simBuffer = null;
        RenderTexture result = null;
        #endregion

        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData) {
            ref var cameraData = ref renderingData.cameraData;
            var camera = cameraData.camera;
        }

        protected override void Render(
            CommandBuffer commandBuffer, ref RenderingData renderingData,
            RenderTargetIdentifier source, RenderTargetIdentifier dest
        ) {
            //ref var cameraData = ref renderingData.cameraData;
            if (!Component.enable.value) {
                commandBuffer.Blit(source, dest); return;
            }
            if (!Constraints()) {
                commandBuffer.Blit(source, dest); return;
            }

            if (Component._noiseTex.value != null) {
                Constraints();

                if (clock++ >= Component.refreshRate.value) {
                    clock = 0; SetSimMat();
                    commandBuffer.Blit(
                        _simBuffer[swapBuffer ? 1 : 0], 
                        _simBuffer[swapBuffer ? 0 : 1], SIM_Mat
                    );
                    swapBuffer = !swapBuffer;
                    if (Component.debug.value) {
                        commandBuffer.Blit(_simBuffer[swapBuffer ? 0 : 1], dest);
                        return;
                    }
                }

                RaindropFX_Tools.KawaseBlur(
                    commandBuffer, _simBuffer[swapBuffer ? 0 : 1], result,
                    Component.fusion.value, BLUR_Mat,
                    new Vector2Int(_simBuffer[0].width, _simBuffer[0].height)
                );

                //RaindropFX_Tools.GaussianBlur(
                //    _simBuffer[swapBuffer ? 0 : 1], result, 
                //    Component.fusion.value, BLUR_Mat
                //);

                // pixelize the raindrop
                BLEND_Mat.SetFloat("_PixelSize", -1);

                // convert height map to normal map and create screen blend effect
                SetBlendMat(ref result, ref result, ref result);

                // output final result
                commandBuffer.Blit(source, dest, BLEND_Mat);

                //RenderTexture.ReleaseTemporary(result);
            } else {
                Debug.Log("noise tex is null!");
                commandBuffer.Blit(source, dest);
            }
        }

        protected override bool IsActive() {
            return Component.IsActive;
        }

        //---------------------------------
        // Setup materials
        //---------------------------------
        private void SetSimMat() {
            SIM_Mat.SetTexture("_NoiseTex", Component._noiseTex.value);
            SIM_Mat.SetFloat("_staDens", Component.StaticSpawnRate.value);
            SIM_Mat.SetFloat("_dynDens", Component.DynamicSpawnRate.value);
            SIM_Mat.SetFloat("_TurbScale", Component.windTurbulence.value);
            SIM_Mat.SetInt("_fadeout", Component.fadeout_fadein_switch.value ? 0 : 1);
            SIM_Mat.SetVector("_Force_RaindropSize", new Vector4(
                Component.wind.value.x + Component.gravity.value.x,
                Component.wind.value.y + Component.gravity.value.y,
                Component.staticRaindropSize.value,
                Component.dynamicRaindropSize.value
            ));
            SIM_Mat.SetVector("_Lifetime", new Vector4(
                Component.dynamicLifetime.value,
                Component.staticLifetime.value
            ));
            SIM_Mat.SetVector("_MainTex_TexelSize", new Vector4(
                _simBuffer[0].width, _simBuffer[0].height,
                1.0f / _simBuffer[0].width, 1.0f / _simBuffer[0].height
            ));
        }

        private void SetBlendMat(ref RenderTexture heightMap, ref RenderTexture wetMap, ref RenderTexture wipeMap) {
            BLEND_Mat.SetFloat("_Distortion", Component.distortion.value);
            BLEND_Mat.SetTexture("_HeightMap", heightMap);

            BLEND_Mat.SetInt("_IsUseFog", 0);
            BLEND_Mat.SetInt("_IsUseWipe", 0);

            BLEND_Mat.SetFloat("_inBlack", Component._inBlack.value);
            BLEND_Mat.SetFloat("_inWhite", Component._inWhite.value);
            BLEND_Mat.SetFloat("_outWhite", Component._outWhite.value);
            BLEND_Mat.SetFloat("_outBlack", Component._outBlack.value);
            BLEND_Mat.SetFloat("_cutEdge", Component.sharpenEdge.value);
        }

        //---------------------------------
        // Initialize raindrop solver
        //---------------------------------
        private void InitSys() {
            RaindropFX_Tools.debugLog = false;

            Vector2Int size = new Vector2Int(
                Screen.width / (Component.DownSampling.value + 1),
                Screen.height / (Component.DownSampling.value + 1)
            );

            //Debug.Log("[RaindropFX GPU] CalcTexSize: " + size);

            _simBuffer = new RenderTexture[2];
            _simBuffer[0] = new RenderTexture(size.x, size.y, 0);
            _simBuffer[1] = new RenderTexture(size.x, size.y, 0);
            result = new RenderTexture(size.x, size.y, 0);

            if (Shader.Find("Custom/RaindropFX/GPUCore") != null)
                SIM_Mat = new Material(
                    Shader.Find("Custom/RaindropFX/GPUCore")
                );
            if (Shader.Find("Custom/RaindropFX/ScreenBlendEffect_GPU") != null)
                BLEND_Mat = new Material(
                    Shader.Find("Custom/RaindropFX/ScreenBlendEffect_GPU")
                );
            if (Shader.Find("Hidden/Custom/KawaseBlur") != null)
                BLUR_Mat = new Material(
                    Shader.Find("Hidden/Custom/KawaseBlur")
                    //Shader.Find("Hidden/Custom/GaussianBlur_GPU")
                );
        }

        //---------------------------------
        // Parameter security detection
        //---------------------------------
        private bool Constraints() {
            if (_simBuffer == null) InitSys();
            if (_simBuffer[0] == null) InitSys();
            if (_simBuffer[1] == null) InitSys();
            return true;
        }
    }
}