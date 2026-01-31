using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RaindropFX {

    [Serializable]
    public class RaindropFX_GPU : VolumeComponent {
        public BoolParameter enable = new BoolParameter(false);

        [Tooltip("Waterdrops will fade in/out automatically if you disable/enable this.")]
        public BoolParameter fadeout_fadein_switch = new BoolParameter(false);

        [Tooltip("Noise texture is used to specify where raindrops should be generated.")]
        public TextureParameter _noiseTex = new TextureParameter(null);

        [Tooltip("Frame interval of texture rendering.")]
        public IntParameter refreshRate = new IntParameter(10);

        [Tooltip("Raindrop texture resolution = screen resolution / down sampling.")]
        public ClampedIntParameter DownSampling = new ClampedIntParameter(1, 0, 4);

        [Tooltip("Spawn rate of static raindrops.")]
        public ClampedFloatParameter StaticSpawnRate = new ClampedFloatParameter(5.0f, 0, 24);

        [Tooltip("Spawn rate of dynamic raindrops.")]
        public ClampedFloatParameter DynamicSpawnRate = new ClampedFloatParameter(2.0f, 0f, 6f);

        [Tooltip("Size of static raindrops.")]
        public ClampedFloatParameter staticRaindropSize = new ClampedFloatParameter(1.0f, 0, 4);

        [Tooltip("Size of dynamic raindrops.")]
        public ClampedFloatParameter dynamicRaindropSize = new ClampedFloatParameter(1.0f, 0, 4);

        [Tooltip("Lifetime of dynamic raindrops.")]
        public ClampedFloatParameter dynamicLifetime = new ClampedFloatParameter(1.0f, 0, 1);

        [Tooltip("Lifetime of static raindrops.")]
        public ClampedFloatParameter staticLifetime = new ClampedFloatParameter(1.0f, 0, 1);

        [Tooltip("Sharpen edge of raindrops.")]
        public ClampedFloatParameter sharpenEdge = new ClampedFloatParameter(1.0f, 0, 1);

        [Tooltip("Fusion small droplets.")]
        public ClampedIntParameter fusion = new ClampedIntParameter(2, 0, 15);

        [Tooltip("Gravity adjustment.")]
        public Vector2Parameter gravity = new Vector2Parameter(new Vector2(0, -9.8f));

        [Tooltip("Wind power adjustment.")]
        public Vector2Parameter wind = new Vector2Parameter(Vector2.zero);

        [Tooltip("Adjust scale of wind turbulence.")]
        public ClampedFloatParameter windTurbulence = new ClampedFloatParameter(1.0f, 0f, 10f);

        [Tooltip("Screen blend effect intensity.")]
        public ClampedFloatParameter distortion = new ClampedFloatParameter(0.6f, 0, 10);

        [Tooltip("Color level parameter.")]
        public ClampedFloatParameter _inBlack = new ClampedFloatParameter(55.0f, 0f, 255f);

        [Tooltip("Color level parameter.")]
        public ClampedFloatParameter _inWhite = new ClampedFloatParameter(180.0f, 0f, 255f);

        [Tooltip("Color level parameter.")]
        public ClampedFloatParameter _outWhite = new ClampedFloatParameter(160.0f, 0f, 255f);

        [Tooltip("Color level parameter.")]
        public ClampedFloatParameter _outBlack = new ClampedFloatParameter(5.0f, 0f, 255f);

        [Tooltip("Debug raindrop texture.")]
        public BoolParameter debug = new BoolParameter(false);

        public bool IsActive => _noiseTex != null;
    }

}