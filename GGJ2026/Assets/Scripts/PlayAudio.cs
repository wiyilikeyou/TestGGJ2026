using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ib_Core;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
   [SerializeField]private int audioIndex = 0;
   [SerializeField]private bool loop = false;
   [SerializeField]private float volume = 1.0f;
   [SerializeField] private float delay = 0;
   private void OnEnable()
   {
      Ib_Async.DelayDoSomething(delay,PlayAudioClip,this.GetCancellationTokenOnDestroy());
   }

   private void PlayAudioClip()
   {
      if (audioIndex >= 0 && audioIndex < AudioManager.Instance.audioClips.Count)
      {
         Ib_Log.Info($"Play Audio Clip {audioIndex}");
         AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[audioIndex],0,volume);
      }
   }
}
