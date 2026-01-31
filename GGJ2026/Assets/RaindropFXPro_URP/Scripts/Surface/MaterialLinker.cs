using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaindropFX;

namespace RaindropFX {
    [ExecuteInEditMode]
    public class MaterialLinker : MaterialLinkerBase {
        #region properties
        public string normalTex_PropName = "_BumpMap";
        public string fogMask_PropName = "_FogMaskMap";
        public string wipeMask_PropName = "_WipeMap";
        #endregion

        private void Update() {
            Solve();

            if (targetMat == null) return;
            targetMat.SetFloat("_IsUseWipe", wipeEffect ? 1.0f : 0.0f);
            if (wipeEffect && wipeDelta != null) {
                targetMat.SetTexture(wipeMask_PropName, wipeMask);
            }

            if (calcRainTex != null && fogMask != null) {
                targetMat.SetTexture(normalTex_PropName, calcRainTex);
                targetMat.SetTexture(fogMask_PropName, fogMask);
            }
        }

    }

}