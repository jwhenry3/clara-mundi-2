using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{

    public enum AlertType
    {
        System,
        Error,
        Success,
        Warning,
        Info,
    }
    public class AlertMessage
    {
        public AlertType Type = AlertType.Info;
        // Expire after 5 seconds
        public float Expiry = 5;
        public string Message = "";
    }

    public class AlertController : NetworkBehaviour
    {

        // Only the current player should receive alert messages from the server
        // this will reduce traffic over the net while reporting only to the owner
        // Using SyncVar will allow us to covertly send RPC data to the client and retain
        // the last value sent
        [SyncVar(ReadPermissions = ReadPermission.OwnerOnly, OnChange = "OnAlertMessage")]
        public AlertMessage CurrentMessage;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;
            if (AlertManager.Instance == null) return;
            // testing welcome alert
            AlertManager.Instance.AddMessage(new AlertMessage
            {
                Type = AlertType.System,
                Message = "Welcome to Clara Mundi!"
            });
        }
        public void SendAlert(AlertMessage message)
        {
            if (!IsServer)
            {
                if (AlertManager.Instance == null) return;
                // allow client dispatching of alerts to self
                AlertManager.Instance.AddMessage(message);
                return;
            }
            CurrentMessage = message;
        }
        public void SendAlert(AlertType type, string message, float expiry = 5)
        {
            SendAlert(new AlertMessage
            {
                Type = type,
                Message = message,
                Expiry = expiry
            });
        }

        private void OnAlertMessage(AlertMessage oldMessage, AlertMessage newMessage, bool asServer)
        {
            // Don't double up alerts when running in host mode
            if (asServer) return;
            if (AlertManager.Instance == null) return;
            // send the message to the UI area if accessible
            AlertManager.Instance.AddMessage(newMessage);
        }
    }
}