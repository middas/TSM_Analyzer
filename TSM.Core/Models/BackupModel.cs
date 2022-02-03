using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TSM.Core.Exceptions;

namespace TSM.Core.Models
{
    public class BackupModel
    {
        private const string TradeSkillData = "TradeSkillMasterDB";

        private readonly LuaModel backingLuaModel;

        public Character[] Characters { get; set; }

        public BackupModel(LuaModel luaModel)
        {
            backingLuaModel = luaModel;

            PopulateData();
        }

        private void PopulateData()
        {
            PopulateCharacterData();
            //c@Mugaelai - Korialstrasz@internalData@auctionSaleHints
            //r@Korialstrasz@internalData@csvExpired
            //r@Korialstrasz@internalData@csvSales
            //c@Mugaelai - Korialstrasz@internalData@auctionMessages
            //r@Korialstrasz@internalData@csvBuys
            //r@Korialstrasz@internalData@csvExpense
            //c@Mugaelai - Korialstrasz@internalData@auctionPrices
            //r@Korialstrasz@internalData@csvCancelled
        }

        private void PopulateCharacterData()
        {
            const string characterRexex = @"s@(?<name>[A-Za-z]+) - (?<faction>[A-Za-z]+) - (?<realm>[A-Za-z]+)@internalData@(?<type>[A-Za-z]+)";
            HashSet<Character> characters = new HashSet<Character>();
            LuaModel data = backingLuaModel[TradeSkillData];

            if (data == null) throw new InvalidBackupException("Not a valid backup file.");

            foreach (var characterLuaModel in data.Children.Where(x => x.Key.StartsWith("s@")))
            {
                Match match = Regex.Match(characterLuaModel.Key, characterRexex);

                if (match.Success)
                {
                    Character character = new(match.Groups["name"].Value, Enum.Parse<Faction>(match.Groups["faction"].Value), match.Groups["realm"].Value);
                    if (!characters.Add(character))
                    {
                        characters.TryGetValue(character, out character);
                    }

                    switch (match.Groups["type"].Value)
                    {
                        case "goldLogLastUpdate":
                            character.GoldLogLastUpdate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(characterLuaModel.Value));
                            break;
                        case "money":
                            character.Money = new Money(long.Parse(characterLuaModel.Value));
                            break;
                        case "mailQuantity":
                            character.MailItems = ParseItems(characterLuaModel.Children);
                            break;
                        case "reagentBankQuantity":
                            character.ReagentItems = ParseItems(characterLuaModel.Children);
                            break;
                        case "auctionQuantity":
                            character.AuctionItems = ParseItems(characterLuaModel.Children);
                            break;
                        case "classKey":
                            character.Class = characterLuaModel.Value;
                            break;
                        case "bagQuantity":
                            character.BagItems = ParseItems(characterLuaModel.Children);
                            break;
                        case "bankQuantity":
                            character.BankItems = ParseItems(characterLuaModel.Children);
                            break;
                    }
                }
            }

            Characters = characters.ToArray();
        }

        private Dictionary<string, int> ParseItems(ImmutableList<LuaModel> children)
        {
            if (children == null) return new Dictionary<string, int>();

            return children.Select(c => new { c.Key, c.Value }).ToDictionary(x => x.Key, x => int.Parse(x.Value));
        }
    }
}
