using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaindropFX;

namespace RaindropFX {
    public class CachePool_URP : MonoBehaviour {

        public int counter = 0;

        List<Raindrop_URP> raindrops = new List<Raindrop_URP>();

        public void Init() {
            counter = 0;
            raindrops.Clear();
        }

        public void Recycle(Raindrop_URP raindrop) {
            raindrops.Add(raindrop);
            counter = raindrops.Count;
        }

        public Raindrop_URP GetRaindrop() {
            if (counter > 0) {
                Raindrop_URP temp = raindrops[0];
                raindrops.RemoveAt(0);
                counter = raindrops.Count;
                return temp;
            } else return new Raindrop_URP();
        }

    }
}
