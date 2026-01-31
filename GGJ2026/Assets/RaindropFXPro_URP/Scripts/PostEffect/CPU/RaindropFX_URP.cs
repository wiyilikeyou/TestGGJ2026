using UnityEngine;
using UnityEngine.Rendering;

namespace RaindropFX {

    public class RaindropFX_URP : VolumeComponent {
        // basic properties -------------------------------------------------------------------------------
        [Header("Basic Settings")]
        public BoolParameter enable = new BoolParameter(false);

        [Tooltip("Waterdrops will fade in/out automatically if you disable/enable this.")]
        public BoolParameter fadeout_fadein_switch = new BoolParameter(false);

        [Tooltip("Waterdrops will fade in/out with higher frame rate but lower accuracy.")]
        public BoolParameter fastMode = new BoolParameter(false);

        [Tooltip("Control the speed of fadeout, the bigger, the faster.")]
        public ClampedFloatParameter fadeSpeed = new ClampedFloatParameter(0.02f, 0.01f, 1);

        [Tooltip("Frame interval of texture rendering.")]
        public ClampedIntParameter refreshRate = new ClampedIntParameter(1, 0, 10);

        [Tooltip("Please use droplet texture with alpha channel.")]
        public TextureParameter _raindropTex_alpha = new TextureParameter(null);

        [Tooltip("Specifies the size of the rendered texture.")]
        public BoolParameter forceRainTextureSize = new BoolParameter(true);

        [Tooltip("Specifies the size of the rendered texture.")]
        public Vector2Parameter calcRainTextureSize = new Vector2Parameter(new Vector2Int(800, 450));

        [Tooltip("If rain texture size is not forced, size = current screen resolution * downSampling.")]
        public ClampedFloatParameter downSampling = new ClampedFloatParameter(0.5f, 0.125f, 8.0f);

        // post properties --------------------------------------------------------------------------------
        [Header("Special Post Effects")]
        [Tooltip("Use a grayscale image to specify the screen area affected by water droplets, " +
            "with black color representing culling area.")]
        public TextureParameter _rainMask_grayscale = new TextureParameter(null);

        [Tooltip("Enable this if you want to pixelize the raindrops.")]
        public BoolParameter pixelization = new BoolParameter(false);

        [Tooltip("Pixelization the raindrop texture, set size of a pixel.")]
        public ClampedFloatParameter pixResolution = new ClampedFloatParameter(1, 1, 1024);

        // interactive properties ------------------------------------------------------------------------
        [Header("Interactive Settings")]
        [Tooltip("Allow you to wipe the raindrops via GameObject.")]
        public BoolParameter wipeEffect = new BoolParameter(false);

        [Tooltip("The speed of screen fog recovery after being wiped.")]
        public ClampedFloatParameter foggingSpeed = new ClampedFloatParameter(0.98f, 0, 1);

        // physical properties ----------------------------------------------------------------------------
        [Header("Physical Settings")]
        [Tooltip("Time step of physical computing.")]
        public ClampedFloatParameter calcTimeStep = new ClampedFloatParameter(0.1f, 0, 1);

        [Tooltip("Enable this if you want to use wind.")]
        public BoolParameter useWind = new BoolParameter(false);

        [Tooltip("Enable radial wind, mostly for driving simulation.")]
        public BoolParameter radialWind = new BoolParameter(false);

        [Tooltip("Enable wind turbulence.")]
        public ClampedFloatParameter windTurbulence = new ClampedFloatParameter(0.1f, 0, 1);

        [Tooltip("Adjust scale of wind turbulence.")]
        public ClampedFloatParameter windTurbScale = new ClampedFloatParameter(1.0f, 0.01f, 10);

        [Tooltip("Wind power adjustment.")]
        public Vector2Parameter wind = new Vector2Parameter(new Vector2(0.0f, 0.0f));

        [Tooltip("Gravity adjustment.")]
        public Vector2Parameter gravity = new Vector2Parameter(new Vector2(0.0f, -9.8f));

        [Tooltip("Friction adjustment.")]
        public FloatParameter friction = new FloatParameter(0.8f);

        // raindrop properties ------------------------------------------------------------------------
        [Header("Raindrop Settings")]
        [Tooltip("Dynamic water droplets produce a tail when they slide if you enable this.")]
        public BoolParameter generateTrail = new BoolParameter(true);

        [Tooltip("Max number of static raindrops.")]
        public ClampedIntParameter maxStaticRaindropNumber = new ClampedIntParameter(5000, 0, 10000);

        [Tooltip("Max number of dynamic raindrops.")]
        public ClampedIntParameter maxDynamicRaindropNumber = new ClampedIntParameter(10, 0, 1000);

        [Tooltip("Random droplet size range.")]
        public Vector2Parameter raindropSizeRange = new Vector2Parameter(new Vector2(0.1f, 0.25f));

        // rednergin properties ------------------------------------------------------------------------
        [Header("Rendering Settings")]
        [Tooltip("Tint color for droplets.")]
        public ColorParameter tintColor = new ColorParameter(Color.white);

        [Tooltip("The larger the value, the thicker the color.")]
        public ClampedIntParameter tintWeight = new ClampedIntParameter(1, 1, 10);

        [Tooltip("Fusion droplets nearby.")]
        public ClampedIntParameter fusion = new ClampedIntParameter(1, 0, 15);

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

        // Fog properties ---------------------------------------------------------------------------------
        [Header("Fog Settings")]
        [Tooltip("If you want to use screen fog, enable this.")]
        public BoolParameter useFog = new BoolParameter(false);

        [Tooltip("Tint color of fog.")]
        public ColorParameter fogTint = new ColorParameter(Color.white);

        [Tooltip("Screen fog effect intensity.")]
        public ClampedFloatParameter fogIntensity = new ClampedFloatParameter(0.5f, 0.01f, 1);

        [Tooltip("Controls the effect of water droplet wake on fog.")]
        public ClampedIntParameter fogIteration = new ClampedIntParameter(2, 1, 5);

        // DOF properties ---------------------------------------------------------------------------------
        [Header("Depth-of-Field Settings")]
        [Tooltip("Enable this if you want to blur waterdrops.")]
        public BoolParameter dropletBlur = new BoolParameter(false);

        [Tooltip("Adjust focal length.")]
        public ClampedFloatParameter _focalize = new ClampedFloatParameter(1.0f, 0f, 10f);

        [Tooltip("Adjust blur strength.")]
        public ClampedIntParameter blurIteration = new ClampedIntParameter(1, 0, 10);

        public bool IsActive => _raindropTex_alpha != null;
    }
    
}