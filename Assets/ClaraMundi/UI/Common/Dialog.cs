using UnityEngine;
using UnityEngine.Events;
namespace ClaraMundi
{
  public class Dialog : MonoBehaviour
  {

    public string Context;
    public UnityEvent<Dialog> OnOpen = new();
    public UnityEvent<Dialog> OnClose = new();
    public UnityEvent<Dialog> OnConfirm = new();
    public UnityEvent<Dialog> OnCancel = new();

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

    void OnEnable()
    {
      OnOpen?.Invoke(this);
    }
    void OnDisable()
    {
      OnClose?.Invoke(this);
    }
  }
}