using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace Stats;

public class StatsCommand(IServiceProvider provider) : ICommand {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  public void Dispose() { }
  public void Start() { }

  public string Id => "stats";

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    var response = await client.GetAsync("user/summary/" + executor.Id);

    if (!response.IsSuccessStatusCode) return CommandResult.ERROR;

    var body = await response.Content.ReadAsStringAsync();

    var statsInfo = JsonSerializer.Deserialize<StatsResponse>(body);

    if (statsInfo == null) return CommandResult.ERROR;

    info.ReplySync(
      $" {ChatColors.Grey}Stats for {ChatColors.Default}{statsInfo.name}");
    info.ReplySync(
      $" {ChatColors.DarkRed}K{ChatColors.Red}D{ChatColors.LightRed}R{ChatColors.Grey}: {ChatColors.Yellow}{statsInfo.kdr:F2}");
    info.ReplySync($" {ChatColors.Blue}Good Kills: {ChatColors.LightYellow}"
      + statsInfo.good_kills);
    info.ReplySync(
      $" {ChatColors.Red}Bad Kills: {ChatColors.LightYellow}{statsInfo.bad_kills}");
    if (statsInfo.rounds_played != null)
      printRoleDictionary(info, $" {ChatColors.Default}Rounds Played",
        statsInfo.rounds_played);
    if (statsInfo.rounds_won != null)
      printRoleDictionary(info, $" {ChatColors.Green}Rounds Won",
        statsInfo.rounds_won);
    if (statsInfo.kills_as != null)
      printRoleDictionary(info, $" {ChatColors.Red}Kills As",
        statsInfo.kills_as);
    if (statsInfo.deaths_as != null)
      printRoleDictionary(info, $" {ChatColors.Grey}Deaths As",
        statsInfo.deaths_as);
    info.ReplySync(
      $" {ChatColors.Silver}Bodies Found: {ChatColors.LightYellow}{statsInfo.bodies_found}");

    return CommandResult.SUCCESS;
  }

  private void
    printRoleDictionary(ICommandInfo info, string title,
      Dictionary<string, int> dict) {
    info.ReplySync(title
      + $"{ChatColors.Grey}: {ChatColors.Lime}Innocent {ChatColors.Default}- "
      + ChatColors.Yellow + dict.GetValueOrDefault("innocent", 0)
      + $"{ChatColors.Default}, {ChatColors.Red}Traitor {ChatColors.Default}- "
      + ChatColors.Yellow + dict.GetValueOrDefault("traitor", 0)
      + $"{ChatColors.Grey}, {ChatColors.LightBlue}Detective {ChatColors.Default}- "
      + ChatColors.Yellow + dict.GetValueOrDefault("detective", 0));
  }

  private record StatsResponse {
    public required string steam_id { get; init; }
    public required string name { get; init; }
    public float kdr { get; init; }
    public int good_kills { get; init; }
    public int bad_kills { get; init; }
    public int bodies_found { get; init; }

    public Dictionary<string, int>? rounds_played { get; init; }
    public Dictionary<string, int>? kills_as { get; init; }
    public Dictionary<string, int>? deaths_as { get; init; }
    public Dictionary<string, int>? rounds_won { get; init; }
  }
}