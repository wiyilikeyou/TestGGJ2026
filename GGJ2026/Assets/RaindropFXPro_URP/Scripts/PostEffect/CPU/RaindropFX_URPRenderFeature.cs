using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    public class RaindropFX_URPRenderFeature : ScriptableRendererFeature {
        [System.Serializable]
        public class RFXSettings {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Shader blendShader;
        }

        public RFXSettings settings = new RFXSettings();
        private RaindropFX_URPPass rainPass;

        public override void Create() {
            this.name = "RaindropFX_URP";
            rainPass = new RaindropFX_URPPass(settings.renderPassEvent, settings.blendShader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            rainPass.Setup(renderer);
            renderer.EnqueuePass(rainPass);
        }
    }
}