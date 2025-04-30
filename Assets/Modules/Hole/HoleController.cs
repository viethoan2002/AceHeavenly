using System;
using UnityEngine;

namespace Modules.Ball
{
    public class HoleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;

        public void PlayEffect()
        {
            effect.Play();
        }
    }
}
