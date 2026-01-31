#if UNITY_EDITOR
using UnityEditor.Rendering;
using UnityEngine;
using UnityEditor;
using RaindropFX;

[VolumeComponentEditor(typeof(RaindropFX_GPU))]
public class RaindropFX_GPUEditor : VolumeComponentEditor {
    SerializedDataParameter enable;
    SerializedDataParameter refreshRate;
    SerializedDataParameter fadeout_fadein_switch;

    SerializedDataParameter _inBlack;
    SerializedDataParameter _inWhite;
    SerializedDataParameter _outWhite;
    SerializedDataParameter _outBlack;

    SerializedDataParameter wind;
    SerializedDataParameter windTurbulence;
    SerializedDataParameter distortion;

    SerializedDataParameter fusion;
    SerializedDataParameter gravity;
    SerializedDataParameter downSampling;

    SerializedDataParameter noiseTex;
    SerializedDataParameter _raindropTex_alpha;
    SerializedDataParameter _rainMask_grayscale;

    SerializedDataParameter sharpenEdge;
    SerializedDataParameter staticLifetime;
    SerializedDataParameter dynamicLifetime;
    SerializedDataParameter staticSizeRange;
    SerializedDataParameter dynamicSizeRange;
    SerializedDataParameter staticSpawnRate;
    SerializedDataParameter dynamicSpawnRate;

    //public override bool hasAdvancedMode => false;

    public override void OnEnable() {
        base.OnEnable();
        var o = new PropertyFetcher<RaindropFX_GPU>(serializedObject);

        enable = Unpack(o.Find(x => x.enable));
        refreshRate = Unpack(o.Find(x => x.refreshRate));
        fadeout_fadein_switch = Unpack(o.Find(x => x.fadeout_fadein_switch));

        _inBlack = Unpack(o.Find(x => x._inBlack));
        _inWhite = Unpack(o.Find(x => x._inWhite));
        _outWhite = Unpack(o.Find(x => x._outWhite));
        _outBlack = Unpack(o.Find(x => x._outBlack));

        wind = Unpack(o.Find(x => x.wind));
        windTurbulence = Unpack(o.Find(x => x.windTurbulence));
        distortion = Unpack(o.Find(x => x.distortion));

        fusion = Unpack(o.Find(x => x.fusion));
        gravity = Unpack(o.Find(x => x.gravity));
        downSampling = Unpack(o.Find(x => x.DownSampling));

        noiseTex = Unpack(o.Find(x => x._noiseTex));

        sharpenEdge = Unpack(o.Find(x => x.sharpenEdge));
        staticLifetime = Unpack(o.Find(x => x.staticLifetime));
        dynamicLifetime = Unpack(o.Find(x => x.dynamicLifetime));
        staticSizeRange = Unpack(o.Find(x => x.staticRaindropSize));
        dynamicSizeRange = Unpack(o.Find(x => x.dynamicRaindropSize));
        staticSpawnRate = Unpack(o.Find(x => x.StaticSpawnRate));
        dynamicSpawnRate = Unpack(o.Find(x => x.DynamicSpawnRate));
    }

    public override void OnInspectorGUI() {
        GUILayout.Space(10.0f);
        GUILayout.Label("[ Basic Settings ]");
        PropertyField(enable);
        PropertyField(fadeout_fadein_switch);
        PropertyField(refreshRate);
        PropertyField(downSampling);

        GUILayout.Space(10.0f);
        GUILayout.Label("[ Texture Settings ]");
        PropertyField(noiseTex);

        GUILayout.Space(10.0f);
        GUILayout.Label("[ Raindrop Settings ]");
        PropertyField(staticLifetime);
        PropertyField(dynamicLifetime);
        PropertyField(staticSizeRange);
        PropertyField(dynamicSizeRange);
        PropertyField(staticSpawnRate);
        PropertyField(dynamicSpawnRate);

        GUILayout.Space(10.0f);
        GUILayout.Label("[ Physics Settings ]");
        PropertyField(wind);
        PropertyField(windTurbulence);
        PropertyField(gravity);

        GUILayout.Space(10.0f);
        GUILayout.Label("[ Rendering Settings ]");
        PropertyField(fusion);
        PropertyField(distortion);
        PropertyField(sharpenEdge);
        PropertyField(_inBlack);
        PropertyField(_inWhite);
        PropertyField(_outWhite);
        PropertyField(_outBlack);
    }
}
#endif
