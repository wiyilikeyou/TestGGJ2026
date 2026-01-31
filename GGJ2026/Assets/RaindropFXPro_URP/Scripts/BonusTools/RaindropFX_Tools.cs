using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RaindropFX {
    public static class RaindropFX_Tools {

        public static bool debugLog = true;

        public static void PrintLog(string message) {
            if (debugLog) Debug.Log("RaindropFX info: " + message);
        }

        public static float PerlinNoiseSampler(Vector2 position, float scale) {
            float value = Mathf.PerlinNoise(position.x * scale, position.y * scale) * 2.0f - 1.0f;
            return value;
        }

        public static void RenderTextureToTexture2D(RenderTexture rTex, Texture2D dest) {
            RenderTexture act = RenderTexture.active; RenderTexture.active = rTex;
            dest.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            dest.Apply(); RenderTexture.active = act;
        }

        public static Vector2Int GetViewSize() {
#if UNITY_EDITOR
            return new Vector2Int(
                (int)UnityEditor.Handles.GetMainGameViewSize().x,
                (int)UnityEditor.Handles.GetMainGameViewSize().y
            );
#else
            return new Vector2Int(Screen.width, Screen.height);
#endif
        }

        public static Vector2Int GetDownSize(float downSamplingRate) {
            var size = GetViewSize();
            return new Vector2Int(
                (int)(downSamplingRate * size.x),
                (int)(downSamplingRate * size.y)
            );
        }

        public static Vector2 RotateAround(Vector2 targetPoint, Vector2 rotCenter, float theta) {
            float cx = rotCenter.x, cy = rotCenter.y;
            float px = targetPoint.x, py = targetPoint.y;

            float s = Mathf.Sin(theta);
            float c = Mathf.Cos(theta);
            px -= cx; py -= cy;
            
            float xnew = px * c + py * s;
            float ynew = -px * s + py * c;
            px = xnew + cx;
            py = ynew + cy;

            return new Vector2(px, py);
        }

        //----------------
        // Kawase blur
        //----------------
        public static void KawaseBlur(
            CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dst,
            int nIterations, Material blurMaterial, Vector2Int calcTexSize
        ) {
            int tmpId1 = Shader.PropertyToID("tmpKawaseBlurRT1");
            int tmpId2 = Shader.PropertyToID("tmpKawaseBlurRT2");
            cmd.GetTemporaryRT(
                tmpId1, calcTexSize.x, calcTexSize.y,
                0, FilterMode.Bilinear, RenderTextureFormat.ARGB32
            );
            cmd.GetTemporaryRT(
                tmpId2, calcTexSize.x, calcTexSize.y,
                0, FilterMode.Bilinear, RenderTextureFormat.ARGB32
            );
            RenderTargetIdentifier tmpRT1, tmpRT2;
            tmpRT1 = new RenderTargetIdentifier(tmpId1);
            tmpRT2 = new RenderTargetIdentifier(tmpId2);

            if (nIterations > 0) {
                // first pass
                cmd.SetGlobalFloat("_offset", 1.5f);
                cmd.Blit(src, tmpRT1, blurMaterial);

                for (var i = 1; i < nIterations - 1; i++) {
                    cmd.SetGlobalFloat("_offset", 0.5f + i);
                    cmd.Blit(tmpRT1, tmpRT2, blurMaterial);

                    // do blur
                    var rttmp = tmpRT1;
                    tmpRT1 = tmpRT2;
                    tmpRT2 = rttmp;
                }

                // final pass
                cmd.SetGlobalFloat("_offset", 0.5f + nIterations - 1f);
                cmd.Blit(tmpRT1, dst, blurMaterial);
            } else {
                cmd.Blit(src, dst);
            }

            cmd.ReleaseTemporaryRT(tmpId1);
            cmd.ReleaseTemporaryRT(tmpId2);
        }

        //----------------
        // Gaussian blur
        //----------------
        public static void GaussianBlur(Texture src, RenderTexture dst, int nIterations, Material gaussianMat) {
            var tmp0 = RenderTexture.GetTemporary(src.width, src.height, 0);
            var tmp1 = RenderTexture.GetTemporary(src.width, src.height, 0);
            var iters = Mathf.Clamp(nIterations, 0, 15);
            Graphics.Blit(src, tmp0);
            for (var i = 0; i < iters; i++) {
                for (var pass = 1; pass < 3; pass++) {
                    tmp1.DiscardContents();
                    tmp0.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(tmp0, tmp1, gaussianMat, pass);
                    var tmpSwap = tmp0;
                    tmp0 = tmp1;
                    tmp1 = tmpSwap;
                }
            }
            Graphics.Blit(tmp0, dst);
            RenderTexture.ReleaseTemporary(tmp0);
            RenderTexture.ReleaseTemporary(tmp1);
        }

        public static Texture2D RenderTexToTex2D(RenderTexture input) {
            int width = input.width, height = input.height;
            Texture2D tex2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture temp = RenderTexture.active;
            RenderTexture.active = input;
            tex2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex2D.Apply();
            RenderTexture.active = temp;

            return tex2D;
        }
    }
    
}