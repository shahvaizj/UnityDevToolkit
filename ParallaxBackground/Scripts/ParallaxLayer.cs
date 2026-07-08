using UnityEngine;

namespace ShahvaizJ.ParallaxBackground
{
    /// <summary>
    /// Moves this layer relative to a driving camera to create a 2D parallax effect. Attach one
    /// instance to each background layer (sky, mountains, foreground, etc.) and give each a
    /// different <see cref="_parallaxFactor"/> — smaller values feel further away. Optionally
    /// wraps the layer horizontally/vertically for seamless infinite scrolling when the attached
    /// <see cref="SpriteRenderer"/> uses a tiled sprite.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Parallax Layer")]
    public class ParallaxLayer : MonoBehaviour
    {
        private enum Axis
        {
            Horizontal,
            Vertical
        }

        [Header("Camera")]
        [Tooltip("Camera that drives the parallax effect. Defaults to Camera.main if left unassigned.")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Parallax")]
        [Tooltip("How strongly this layer moves relative to the camera per axis. 0 = fixed in place (reads as infinitely far away), 1 = moves exactly with the camera (no parallax).")]
        [SerializeField] private Vector2 _parallaxFactor = new Vector2(0.5f, 0.5f);

        [Header("Infinite Scroll")]
        [Tooltip("Seamlessly repeats this layer horizontally as the camera moves. Requires a SpriteRenderer with a sprite wide enough to tile without a visible seam.")]
        [SerializeField] private bool _infiniteHorizontal = false;

        [Tooltip("Seamlessly repeats this layer vertically as the camera moves. Requires a SpriteRenderer with a sprite tall enough to tile without a visible seam.")]
        [SerializeField] private bool _infiniteVertical = false;

        /// <summary>How strongly this layer moves relative to the camera per axis.</summary>
        public Vector2 ParallaxFactor
        {
            get => _parallaxFactor;
            set => _parallaxFactor = value;
        }

        private SpriteRenderer _spriteRenderer;
        private Vector3 _startPosition;
        private Vector3 _cameraStartPosition;
        private float _textureUnitSizeX;
        private float _textureUnitSizeY;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            ResetLayer();
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
                return;

            Vector3 distanceMoved = _cameraTransform.position - _cameraStartPosition;
            Vector3 parallaxOffset = new Vector3(distanceMoved.x * _parallaxFactor.x, distanceMoved.y * _parallaxFactor.y, 0f);
            Vector3 newPosition = _startPosition + parallaxOffset;

            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            if (_infiniteHorizontal && _textureUnitSizeX > 0f)
                WrapAxis(Axis.Horizontal);

            if (_infiniteVertical && _textureUnitSizeY > 0f)
                WrapAxis(Axis.Vertical);
        }

        /// <summary>
        /// Shifts the tracked start position by one texture unit once the camera has moved a full
        /// tile away from this layer, keeping the wrap invisible as long as the sprite tiles cleanly.
        /// </summary>
        private void WrapAxis(Axis axis)
        {
            if (axis == Axis.Horizontal)
            {
                float offset = _cameraTransform.position.x - transform.position.x;
                if (Mathf.Abs(offset) >= _textureUnitSizeX)
                {
                    float shift = _textureUnitSizeX * Mathf.Sign(offset);
                    _startPosition.x += shift;
                    transform.position += new Vector3(shift, 0f, 0f);
                }
            }
            else
            {
                float offset = _cameraTransform.position.y - transform.position.y;
                if (Mathf.Abs(offset) >= _textureUnitSizeY)
                {
                    float shift = _textureUnitSizeY * Mathf.Sign(offset);
                    _startPosition.y += shift;
                    transform.position += new Vector3(0f, shift, 0f);
                }
            }
        }

        /// <summary>
        /// Reassigns the driving camera at runtime and re-baselines the layer against its current
        /// position, so switching cameras mid-game doesn't cause a visible jump.
        /// </summary>
        public void SetCamera(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
            ResetLayer();
        }

        /// <summary>
        /// Re-baselines this layer's start position and the camera's start position to their
        /// current values, and recomputes tile size for infinite scroll. Call after manually
        /// moving the layer or camera (e.g. on a level transition) to avoid a visible snap.
        /// </summary>
        public void ResetLayer()
        {
            _startPosition = transform.position;

            if (_cameraTransform != null)
                _cameraStartPosition = _cameraTransform.position;

            _textureUnitSizeX = 0f;
            _textureUnitSizeY = 0f;

            if ((_infiniteHorizontal || _infiniteVertical) && _spriteRenderer != null && _spriteRenderer.sprite != null)
            {
                Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;
                _textureUnitSizeX = spriteSize.x * transform.lossyScale.x;
                _textureUnitSizeY = spriteSize.y * transform.lossyScale.y;
            }
        }
    }
}
