using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
    public class Outline : MonoBehaviour
    {
        private GameObject Parent;
        private ProceduralImage img;
        public bool Always;
        private void Awake()
        {
            Parent = transform.parent.gameObject;
            img = GetComponent<ProceduralImage>();
        }

        private void Update()
        {
            img.enabled = Always || EventSystem.current.currentSelectedGameObject == Parent;
        }
    }
}