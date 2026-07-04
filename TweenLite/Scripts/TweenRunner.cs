using System.Collections.Generic;
using UnityEngine;

namespace ShahvaizJ.TweenLite
{
    // Hidden singleton MonoBehaviour that ticks every active Tween once per frame. Created lazily
    // on first use so nothing needs to be added to a scene by hand.
    internal sealed class TweenRunner : MonoBehaviour
    {
        private static TweenRunner _instance;

        private readonly List<Tween> _tweens = new List<Tween>();

        internal static TweenRunner Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var runnerObject = new GameObject("[TweenLite]")
                {
                    hideFlags = HideFlags.HideInHierarchy
                };
                _instance = runnerObject.AddComponent<TweenRunner>();

                if (Application.isPlaying)
                    DontDestroyOnLoad(runnerObject);

                return _instance;
            }
        }

        internal void Register(Tween tween)
        {
            _tweens.Add(tween);
        }

        internal void KillAll()
        {
            for (int i = _tweens.Count - 1; i >= 0; i--)
                _tweens[i].Kill();

            _tweens.Clear();
        }

        private void Update()
        {
            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                Tween tween = _tweens[i];
                float deltaTime = tween.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                tween.Tick(deltaTime);

                if (!tween.IsActive)
                    _tweens.RemoveAt(i);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
