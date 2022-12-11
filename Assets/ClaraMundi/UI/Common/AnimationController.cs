using UnityEngine;

namespace ClaraMundi
{
    public class AnimationController : MonoBehaviour
    {
        
        Animation Animation;

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
    }
}