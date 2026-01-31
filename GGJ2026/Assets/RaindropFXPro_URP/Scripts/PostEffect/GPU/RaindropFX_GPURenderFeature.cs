using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    public class RaindropFX_GPURenderFeature : ScriptableRendererFeature {
        [System.Serializable]
        public class RFXSettings {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Shader blendShader;
        }

        public RFXSettings settings = new RFXSettings();
        private RaindropFX_GPUPass rainPass;

        public override void Create() {
            this.name = "RaindropFX_GPU";
            rainPass = new RaindropFX_GPUPass(settings.renderPassEvent, settings.blendShader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            rainPass.Setup(renderer);
            renderer.EnqueuePass(rainPass);
        }
    }
}