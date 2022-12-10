using UnityEngine;
using UnityEngine.EventSystems;
namespace ClaraMundi
{
    public class ContextMenuTrigger : MonoBehaviour, IPointerClickHandler
    {
        public ContextMenus ContextMenus;
        public string ContextMenuName;

        public Vector2 MenuOffset;

        public void OnPointerClick(PointerEventData eventData)
        {
            var position1 = transform.position;
            Vector2 position = new Vector2(
                position1.x + MenuOffset.x,
                position1.y + MenuOffset.y
            );
            if (ContextMenus != null)
            {
                ContextMenus.ShowMenu(ContextMenuName, position);
            }
        }
    }
}