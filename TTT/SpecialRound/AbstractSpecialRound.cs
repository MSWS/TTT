using TTT.API;
using TTT.API.Game;
using TTT.Locale;

namespace SpecialRound;

public abstract class AbstractSpecialRound : ITerrorModule {
  public void Dispose() { }
  public void Start() { }
  
  public abstract IMsg Name { get; }
  public abstract IMsg Description { get; }

  public abstract void ApplyRoundEffects();
}