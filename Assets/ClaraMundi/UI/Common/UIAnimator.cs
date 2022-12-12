using UnityEngine;

namespace ClaraMundi
{
    public class UIAnimator : MonoBehaviour
    {
        
        Animation Animation;

        public bool IsHidden()
        {
            return Animation.clip != Animation.GetClip("Show");
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
            if (IsHidden())
                Play("Show");
        }

        public void Hide()
        {
            if (!IsHidden())
                Play("Hide");
        }

        public void Toggle()
        {
            if (IsHidden())
                Show();
            else
                Hide();
        }
    }
}