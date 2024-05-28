using System;

namespace ClaraMundi
{
  public class PartyHandle
  {
    public Party Party;
    public event Action OnChange;

    public void Trigger()
    {
      OnChange?.Invoke();
    }
  }
}