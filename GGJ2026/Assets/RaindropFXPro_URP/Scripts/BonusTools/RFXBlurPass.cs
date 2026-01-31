using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RFXBlurPass : ScriptableRendererFeature {
    [System.Serializable]
    public class RFXBlurPassSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        public Material blurMaterial = null;

        [Range(0, 10)]
        public int blurIteration = 1;

        [Range(1, 6)]
        public int downSampling = 1;

        [HideInInspector]
        public bool copyToFramebuffer;
        public string blurName = "_RFX_BlurOpaque";
        public string opaqueName = "_RFX_Opaque";
    }

    public RFXBlurPassSettings settings = new RFXBlurPassSettings();

    class CustomRenderPass : ScriptableRenderPass {
        public Material blurMaterial;
        public int passes;
        public int downSampling;
        public bool copyToFramebuffer;
        public string blurName;
        public string opaqueName;
        string profilerTag;

        int blurredID, opaqueID;
        RenderTargetIdentifier blurredRT;
        RenderTargetIdentifier opaqueRT;

        private RenderTargetIdentifier source { get; set; }

        public void Setup(ScriptableRenderer renderer) {
            this.source = renderer.cameraColorTarget;
        }

        public CustomRenderPass(string profilerTag) {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            var width = cameraTextureDescriptor.width / downSampling;
            var height = cameraTextureDescriptor.height / downSampling;

            blurredID = Shader.PropertyToID("tmpBlurRT1");
            opaqueID = Shader.PropertyToID("opaqueRT1");
            cmd.GetTemporaryRT(
                blurredID, width, height, 
                0, FilterMode.Bilinear, RenderTextureFormat.ARGB32
            );
            cmd.GetTemporaryRT(
                opaqueID, cameraTextureDescriptor.width, cameraTextureDescriptor.height, 
                0, FilterMode.Bilinear, RenderTextureFormat.ARGB32
            );
            blurredRT = new RenderTargetIdentifier(blurredID);
            opaqueRT = new RenderTargetIdentifier(opaqueID);
            ConfigureTarget(blurredRT);
            ConfigureTarget(opaqueID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            var discriptor = renderingData.cameraData.cameraTargetDescriptor;
            Vector2Int size = new Vector2Int(
                discriptor.width / downSampling,
                discriptor.height / downSampling
            );

            RaindropFX.RaindropFX_Tools.KawaseBlur(
                cmd, source, blurredRT, passes, 
                blurMaterial, size
            );
            cmd.SetGlobalTexture(blurName, blurredRT);
            cmd.Blit(source, opaqueRT);
            cmd.SetGlobalTexture(opaqueName, opaqueRT);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(blurredID);
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create() {
        this.name = "RaindropFX_BlurPass";
        scriptablePass = new CustomRenderPass("RFXBlurPass");
        scriptablePass.blurMaterial = settings.blurMaterial;
        scriptablePass.passes = settings.blurIteration;
        scriptablePass.downSampling = settings.downSampling;
        scriptablePass.copyToFramebuffer = settings.copyToFramebuffer;
        scriptablePass.blurName = settings.blurName;
        scriptablePass.opaqueName = settings.opaqueName;

        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        scriptablePass.Setup(renderer);
        renderer.EnqueuePass(scriptablePass);
    }
}

