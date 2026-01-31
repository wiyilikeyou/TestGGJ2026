using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    public class RaindropFX_URPPass : CustomPostProcessingPass<RaindropFX_URP> {
        public RaindropFX_URPPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader) { }

        protected override string RenderTag => "RaindropFX_URP";

        #region parameters
        public RaindropGenerator solver;
        public RenderTexture temp;
        public RenderTexture temp2;
        public RenderTexture tempD;
        public RenderTexture tempWip;
        public RenderTexture wipDelta;

        bool initFlag = false;
        Wiper wiper = null;
        bool cnt;
        #endregion

        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData) {
            ref var cameraData = ref renderingData.cameraData;
            var camera = cameraData.camera;

            // set shader properties here
            //Material.SetTexture("_RFX_BlurOpqaue", blurred);
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

            Vector2Int size = new Vector2Int(
                (int)Component.calcRainTextureSize.value.x,
                (int)Component.calcRainTextureSize.value.y
            );
            solver.UpdateProps(
               Component.fadeout_fadein_switch.value, Component.fastMode.value, Component.fadeSpeed.value,
               Component.forceRainTextureSize.value, size, Component.calcTimeStep.value, Component.refreshRate.value,
               Component.generateTrail.value, Component.maxStaticRaindropNumber.value, Component.maxDynamicRaindropNumber.value,
               Component.raindropSizeRange.value, Component.useWind.value, Component.windTurbulence.value,
               Component.windTurbScale.value, Component.wind.value, Component.gravity.value, Component.friction.value,
               Component.distortion.value, Component.useFog.value, Component.fogIntensity.value, Component.fogIteration.value,
               Component.fusion.value, Component._inBlack.value, Component._inWhite.value, Component._outWhite.value,
               Component._outBlack.value, Component.dropletBlur.value, Component._focalize.value, Component.blurIteration.value,
               Component.tintColor.value, Component.tintWeight.value, Component.fogTint.value, Component.radialWind.value,
               Component._rainMask_grayscale.value, Component.pixelization.value, Component.pixResolution.value,
               Component.downSampling.value, Component.foggingSpeed.value
            );
            solver.CalcRainTex();
            
            if (Component.wipeEffect.value && wiper != null) {
                if (cnt) {
                    //commandBuffer.Blit(wipDelta, tempD);
                    RaindropFX_Tools.KawaseBlur(
                        commandBuffer, wipDelta, tempD,
                        1, solver.kawase_blur_material, solver.calcTexSize
                    );
                } else {
                    solver.SetTimeMat(wiper.wipeTexture);
                    commandBuffer.Blit(tempD, wipDelta, solver.time_material);
                } cnt = !cnt;

                solver.wipe_material.SetTexture("_WipeTex", wipDelta);
                commandBuffer.Blit(solver.calcRainTex, tempWip, solver.wipe_material);
                wiper.GetWiped(this);
            }

            // apply blur effect to solver.calcRainTex
            //RaindropFX_Tools.KawaseBlur(
            //    commandBuffer, solver.calcRainTex, temp, Component.fusion.value, solver.kawase_blur_material, solver.calcTexSize
            //);
            RaindropFX_Tools.GaussianBlur(
                solver.calcRainTex, temp, Component.fusion.value, solver.blur_material
            );

            // apply color level to solver.calcRainTex
            solver.SetLevelMat();
            commandBuffer.Blit(temp, temp2, solver.level_material);

            // pixelize the raindrop
            solver.blend_material.SetFloat(
                "_PixelSize", solver.pixelization ? solver.pixResolution : -1
            );

            // convert height map to normal map and create screen blend effect
            solver.blend_material.SetInt("_IsEnableWip", Component.wipeEffect.value ? 1 : 0);
            solver.SetScreenBlendMat(ref temp2, ref temp, ref tempD);

            // output final result
            if (Component.dropletBlur.value) { // blur droplet
                var vsize = RaindropFX_Tools.GetViewSize();
                int tmpId1 = Shader.PropertyToID("tmpDropBlurRT1");
                int tmpId2 = Shader.PropertyToID("tmpDropBlurRT2");
                commandBuffer.GetTemporaryRT(tmpId1, vsize.x, vsize.y, 0);
                commandBuffer.GetTemporaryRT(tmpId2, vsize.x, vsize.y, 0);

                RenderTargetIdentifier tmpRT1, tmpRT2;
                tmpRT1 = new RenderTargetIdentifier(tmpId1);
                tmpRT2 = new RenderTargetIdentifier(tmpId2);

                commandBuffer.Blit(source, tmpRT1, solver.blend_material);
                RaindropFX_Tools.KawaseBlur(
                    commandBuffer, tmpRT1, tmpRT2,
                    Component.blurIteration.value,
                    solver.kawase_blur_material, vsize
                );

                commandBuffer.SetGlobalTexture("_BlurredMainTex", tmpRT2);
                solver.SetDropblurMat(ref temp, Component._rainMask_grayscale.value);
                commandBuffer.Blit(tmpRT1, dest, solver.dropblur_material);

                commandBuffer.ReleaseTemporaryRT(tmpId1);
                commandBuffer.ReleaseTemporaryRT(tmpId2);
            } else {
                commandBuffer.Blit(source, dest, solver.blend_material);
            }
        }

        protected override bool IsActive() {
            return Component.IsActive;
        }

        //---------------------------------
        // Initialize raindrop solver
        //---------------------------------
        private void InitSys() {
            solver.Init(
                (Texture2D)Component._raindropTex_alpha.value, new
                Vector2Int((int)Component.calcRainTextureSize.value.x,
                (int)Component.calcRainTextureSize.value.y)
            );
            RaindropFX_Tools.debugLog = false;
            RaindropFX_Tools.PrintLog("now calc tex size: " + "(" + solver.calcTexSize.x + ", " + solver.calcTexSize.y + ")");
        }

        //---------------------------------
        // Parameter security detection
        //---------------------------------
        private bool Constraints() {
            if (Component._raindropTex_alpha == null) {
                Debug.Log("raindrop tex is null!");
                return false;
            }
            if (solver == null) solver = new RaindropGenerator();
            if (solver.calcRainTex == null) initFlag = true;
            if (initFlag) { InitSys(); initFlag = false; }
            if (wiper == null) wiper = GameObject.FindObjectOfType(typeof(Wiper)) as Wiper;
            if (temp == null || temp.width != solver.calcTexSize.x)
                temp = new RenderTexture(solver.calcTexSize.x, solver.calcTexSize.y, 0);
            if (temp2 == null || temp2.width != solver.calcTexSize.x)
                temp2 = new RenderTexture(solver.calcTexSize.x, solver.calcTexSize.y, 0);
            if (tempD == null || tempD.width != solver.calcTexSize.x) {
                tempD = new RenderTexture(solver.calcTexSize.x, solver.calcTexSize.y, 0);
                //tempD.useMipMap = true; tempD.autoGenerateMips = true;
            }
            if (tempWip == null || tempWip.width != solver.calcTexSize.x)
                tempWip = new RenderTexture(solver.calcTexSize.x, solver.calcTexSize.y, 0);
            if (wipDelta == null || wipDelta.width != solver.calcTexSize.x)
                wipDelta = new RenderTexture(solver.calcTexSize.x, solver.calcTexSize.y, 0);

            if (Component.calcRainTextureSize.value.x < 0) Component.calcRainTextureSize.value = new Vector2(1, Component.calcRainTextureSize.value.y);
            if (Component.calcRainTextureSize.value.y < 0) Component.calcRainTextureSize.value = new Vector2(Component.calcRainTextureSize.value.x, 1);
            if (Component.maxDynamicRaindropNumber.value < 0) Component.maxDynamicRaindropNumber.value = 0;
            if (Component.calcTimeStep.value < 0.1f) Component.calcTimeStep.value = 0.1f;
            if (Component.raindropSizeRange.value.x <= 0.01f) Component.raindropSizeRange.value = new Vector2(0.01f, Component.raindropSizeRange.value.y);
            if (Component.raindropSizeRange.value.y < Component.raindropSizeRange.value.x)
                Component.raindropSizeRange.value = new Vector2(Component.raindropSizeRange.value.x, 0.01f);
            // Raindrop_STD.sFriction = Component.friction;
            //RaindropFX_Tools.debugLog = Component.debugLog.value;
            return true;
        }
    }
}