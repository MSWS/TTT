﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class SuppressedRound(IServiceProvider provider)
  : AbstractSpecialRound(provider), IPluginModule {
  public override string Name => "Suppressed";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_SUPPRESSED;
  public override SpecialRoundConfig Config => config;
  private BasePlugin? plugin;

  private SuppressedRoundConfig config
    => Provider.GetService<IStorage<SuppressedRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SuppressedRoundConfig();

  public void Start(BasePlugin? newPlugin) { plugin ??= newPlugin; }

  public override void ApplyRoundEffects() {
    plugin?.HookUserMessage(452, onWeaponSound);
  }

  private static readonly HashSet<uint> silencedWeapons = new() {
    1,  // deagle
    2,  // dual berettas
    3,  // five seven
    30, // tec9
    32, // p2000
    36, // p250
    4,  // glock
    61, // usp-s
    63, // cz75 auto
    64  // r8 revolver
  };

  private HookResult onWeaponSound(UserMessage msg) {
    var defIndex = msg.ReadUInt("item_def_index");

    if (!silencedWeapons.Contains(defIndex)) return HookResult.Continue;

    msg.Recipients.Clear();
    return HookResult.Handled;
  }

  public override void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    plugin?.UnhookUserMessage(452, onWeaponSound);
  }
}