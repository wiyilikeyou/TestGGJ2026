using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    [RequireComponent(typeof(Camera))]
    public class Wiper : MonoBehaviour {
        #region public params
        public Volume postVolumn;
        public int cullLayer = 30;
        public List<GameObject> wipers;
        #endregion

        #region private params
        [HideInInspector]
        public RenderTexture wipeTexture;
        private Camera cam;
        private Dictionary<GameObject, int> originalRenderLayers;
        private RaindropFX_URP RFX;
        #endregion

        void Start() {
            cam = this.GetComponent<Camera>();
            originalRenderLayers = new Dictionary<GameObject, int>();
            postVolumn.sharedProfile.TryGet<RaindropFX_URP>(out RFX);

            wipeTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0);
            wipeTexture.autoGenerateMips = false;
            wipeTexture.name = "RFX_WipeTex";
        }

        public void GetWiped(RaindropFX_URPPass target) {
            StartCoroutine(GetWipedI(target));
        }

        private IEnumerator GetWipedI(RaindropFX_URPPass target) {
            yield return (new WaitForEndOfFrame());
            RaindropFX_Tools.RenderTextureToTexture2D(
                target.tempWip, 
                target.solver.calcRainTex
            );
        }

        private void Update() {
            if (postVolumn == null || wipers.Count < 1) return;

            bool state = postVolumn.enabled;
            var cullMask = cam.cullingMask;
            var clearFlag = cam.clearFlags;
            var bgcol = cam.backgroundColor;

            postVolumn.enabled = false;
            CollectRenderLayers();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.cullingMask = 1 << cullLayer;
            cam.targetTexture = wipeTexture;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.Render();
            cam.backgroundColor = bgcol;
            cam.targetTexture = null;
            cam.cullingMask = cullMask;
            cam.clearFlags = clearFlag;
            RestoreRenderLayers();
            postVolumn.enabled = state;
        }

        void CollectRenderLayers() {
            originalRenderLayers.Clear();
            foreach (GameObject r in wipers) {
                if (!originalRenderLayers.ContainsKey(r)) {
                    originalRenderLayers.Add(r, r.gameObject.layer);
                    r.gameObject.layer = cullLayer;
                }
            }
        }

        void RestoreRenderLayers() {
            foreach (GameObject r in wipers) {
                int originalLayer = 0;
                originalRenderLayers.TryGetValue(r, out originalLayer);
                r.gameObject.layer = originalLayer;
            }
        }
    }
}