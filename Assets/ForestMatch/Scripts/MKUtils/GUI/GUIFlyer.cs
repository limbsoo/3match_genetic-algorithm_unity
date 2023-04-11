using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
    08.10.2020 - first
    25.11.2020 - check parent canvas
    26.01.2022 - random distance
 */
namespace Mkey
{
    public class GUIFlyer : MonoBehaviour
    {
        public Text text;
        [SerializeField]
        private float distanceMax = 300;
        [SerializeField]
        private float distanceMin = 100;
        [SerializeField]
        private float flyTime = 1.5f;
        [SerializeField]
        private EaseAnim ease = EaseAnim.EaseOutCubic;
        [SerializeField]
        private float delay = 0;
        [SerializeField]
        private bool destroy = true;
        [SerializeField]
        private UnityEvent StartEvent;
        [SerializeField]
        private UnityEvent EndEvent;

        private void Start()
        {
            Fly();
        }

        public void Fly(string flyerText)
        {
            if (text) text.text = flyerText;
            Fly();
        }

        public void Fly()
        {
            StartEvent?.Invoke();
            Vector3 pos = transform.localPosition;
            float distance = UnityEngine.Random.Range(distanceMin, distanceMax);
            SimpleTween.Value(gameObject, 0f, distance, flyTime).SetOnUpdate((float val) =>
            {
                if (this) transform.localPosition = pos + new Vector3(0, val);
            })
            .SetEase(ease)
            .SetDelay(delay)
            .AddCompleteCallBack(() =>
            {
                EndEvent?.Invoke();
                if(this && destroy) Destroy(gameObject);
            });
        }

        public GUIFlyer CreateFlyer(Canvas parentCanvas, string flyerText)
        {
            if (!parentCanvas) return null;
            GUIFlyer gF = Instantiate(this, parentCanvas.transform);
            if (gF && gF.text) gF.text.text = flyerText;
            return gF;
        }
    }
}
