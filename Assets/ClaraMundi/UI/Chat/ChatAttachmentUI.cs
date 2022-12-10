using UnityEngine;
using TMPro;

namespace ClaraMundi
{
    public class ChatAttachmentUI : MonoBehaviour
    {

        public string Key;
        public TextMeshProUGUI Text;

        public void Remove()
        {
            ChatWindowUI.Instance.RemoveAttachment(Key);
        }

        public void SetValue(string key, string value)
        {
            Key = key;
            Text.text = value;
        }
    }
}