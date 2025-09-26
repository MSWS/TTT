using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Core;
using TTT.Game;

namespace TTT.CS2.API;

public interface IBodyTracker {
  public IDictionary<IBody, CRagdollProp> Bodies { get; }

  public IBody? ReverseLookup(CRagdollProp ragdoll) {
    return Bodies.FirstOrDefault(x => x.Value == ragdoll).Key;
  }

  public bool TryReverseLookup(CRagdollProp ragdoll,
    [MaybeNullWhen(false)] out IBody body) {
    body = ReverseLookup(ragdoll);
    return body != null;
  }

  public bool TryLookup(string id, out IBody? body) {
    body = Bodies.Keys.FirstOrDefault(x => x.Id == id);
    return body != null;
  }
}