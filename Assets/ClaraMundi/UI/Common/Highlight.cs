using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Highlighter;


        public void OnPointerEnter(PointerEventData eventData)
        {
            Highlighter.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Highlighter.SetActive(false);
        }
    }
}