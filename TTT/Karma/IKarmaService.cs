using TTT.API;
using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.Karma;

public interface IKarmaService : IKeyedStorage<IPlayer, int>,
  IKeyWritable<IPlayer, int>, ITerrorModule;