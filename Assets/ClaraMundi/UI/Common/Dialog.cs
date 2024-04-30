using UnityEngine;
using UnityEngine.Events;
namespace ClaraMundi
{
  public class Dialog : MonoBehaviour
  {

    public string Context;
    public UnityEvent<Dialog> OnClose;
    public UnityEvent<Dialog> OnConfirm;
    public UnityEvent<Dialog> OnCancel;

    public void Confirm()
    {
      OnConfirm?.Invoke(this);
    }
    public void Close()
    {
      gameObject.SetActive(false);
    }
    public void Cancel()
    {
      OnCancel?.Invoke(this);
    }

    void OnDisable()
    {
      OnClose?.Invoke(this);
    }
  }
}