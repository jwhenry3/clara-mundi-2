using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class UIAnimator : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnClick;
        Animation Animation;

        public bool isActivator;
        public bool isVisibility;
        public bool ToggleGameObject;

        public bool IsHidden()
        {
            if (ToggleGameObject)
            {
                return !gameObject.activeInHierarchy;
            }
            return isVisibility && Animation.clip != Animation.GetClip("Show");
        }

        public bool IsActivated()
        {
            return isActivator && Animation.clip == Animation.GetClip("Activate");
        }

        private void Awake()
        {
            Animation = GetComponent<Animation>();
        }

        public void Play(string anim)
        {
            Animation.clip = Animation.GetClip(anim);
            Animation.Play();
        }

        public void Stop()
        {
            Animation.Stop();
        }

        public void Show()
        {
            if (!IsHidden()) return;
            if (ToggleGameObject)
                gameObject.SetActive(true);
            else
                Play("Show");
        }

        public void Activate()
        {
            if (!IsActivated())
                Play("Activate");
        }

        public void Deactivate()
        {
            if (IsActivated())
                Play("Deactivate");
        }

        public void Hide()
        {
            if (IsHidden()) return;
            if (ToggleGameObject)
                gameObject.SetActive(false);
            else
                Play("Hide");
        }

        public void Toggle()
        {
            if (isActivator)
            {
                if (IsActivated())
                    Deactivate();
                else
                    Activate();
                return;
            }
            if (IsHidden())
                Show();
            else
                Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}