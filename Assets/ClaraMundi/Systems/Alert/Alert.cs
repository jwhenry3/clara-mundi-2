using UnityEngine;
using TMPro;

namespace ClaraMundi
{
    public class Alert : MonoBehaviour
    {
        public TextMeshProUGUI MessageText;
        [HideInInspector]
        public AlertMessage Message;
        float timeElapsed;

        public Color SuccessColor;
        public Color ErrorColor;
        public Color WarningColor;
        public Color InfoColor;
        public Color SystemColor;


        void Update()
        {
            if (Message == null) return; 
            
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= Message.Expiry)
                FadeOut();
        }

        void FadeOut()
        {
            Destroy(gameObject);
        }

        public void SetMessage(AlertMessage message)
        {
            // the message text color does not change after created, so run once
            MessageText.color = (message.Type switch
            {
                AlertType.Success => SuccessColor,
                AlertType.Error => ErrorColor,
                AlertType.Warning => WarningColor,
                AlertType.Info => InfoColor,
                AlertType.System => SystemColor,
                _ => InfoColor
            });
            MessageText.text = message.Message;
            Message = message;
        }
    }
}