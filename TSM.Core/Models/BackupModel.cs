﻿using CsvHelper;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using TSM.Core.Exceptions;

namespace TSM.Core.Models
{
    public class BackupModel
    {
        private const string TradeSkillData = "TradeSkillMasterDB";

        private readonly LuaModel backingLuaModel;

        public BackupModel(LuaModel luaModel)
        {
            backingLuaModel = luaModel;

            PopulateData();
        }

        public ImmutableArray<AuctionBuyModel> AuctionBuys { get; private set; }

        public ImmutableArray<CancelledAuctionModel> CancelledAuctions { get; private set; }

        public ImmutableArray<Character> Characters { get; private set; }

        public ImmutableArray<CharacterSaleModel> CharacterSaleModels { get; private set; }

        public ImmutableArray<ExpiredAuctionModel> ExpiredAuctions { get; private set; }

        public ImmutableDictionary<string, string> Items { get; private set; }

        private static IEnumerable<T> ParseCsv<T>(LuaModel lm)
        {
            using MemoryStream memoryStream = new();
            using (StreamWriter streamWriter = new(memoryStream, leaveOpen: true))
            {
                streamWriter.Write(lm.Value.Replace("\\n", "\n"));
                streamWriter.Flush();
            }
            memoryStream.Position = 0;

            using TextReader textReader = new StreamReader(memoryStream);
            using CsvReader csvReader = new(textReader, CultureInfo.CurrentCulture);

            return csvReader.GetRecords<T>().ToArray();
        }

        private static Dictionary<string, int> ParseItems(IEnumerable<LuaModel> children)
        {
            if (children == null) return new Dictionary<string, int>();

            return children.Select(c => new { c.Key, c.Value }).ToDictionary(x => x.Key, x => int.Parse(x.Value));
        }

        private void PopulateAuctionBuys()
        {
            //r@Korialstrasz@internalData@csvBuys
            List<AuctionBuyModel> auctionBuyModels = new();
            LuaModel data = backingLuaModel[TradeSkillData];

            foreach (var lm in data.Children.Where(x => x.Key.EndsWith("csvBuys")))
            {
                auctionBuyModels.AddRange(ParseCsv<AuctionBuyModel>(lm));
            }

            AuctionBuys = auctionBuyModels.ToImmutableArray();
        }

        private void PopulateAuctionData()
        {
            //c@Mugaelai - Korialstrasz@internalData@auctionMessages
            //r@Korialstrasz@internalData@csvExpense
            //c@Mugaelai - Korialstrasz@internalData@auctionPrices
            PopulateCharacterSales();
            PopulateExpiredAuctions();
            PopulateAuctionBuys();
            PopulateCancelledAuctions();
        }

        private void PopulateCancelledAuctions()
        {
            //r@Korialstrasz@internalData@csvCancelled
            List<CancelledAuctionModel> cancelledAuctionModels = new();
            LuaModel data = backingLuaModel[TradeSkillData];

            foreach (var lm in data.Children.Where(x => x.Key.EndsWith("csvCancelled")))
            {
                cancelledAuctionModels.AddRange(ParseCsv<CancelledAuctionModel>(lm));
            }

            CancelledAuctions = cancelledAuctionModels.ToImmutableArray();
        }

        private void PopulateCharacterData()
        {
            const string characterRexex = @"s@(?<name>[A-Za-z]+) - (?<faction>[A-Za-z]+) - (?<realm>[A-Za-z]+)@internalData@(?<type>[A-Za-z]+)";
            HashSet<Character> characters = new();
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

            Characters = characters.ToImmutableArray();
        }

        private void PopulateCharacterSales()
        {
            //r@Korialstrasz@internalData@csvSales
            List<CharacterSaleModel> sharacterSaleModels = new();
            LuaModel data = backingLuaModel[TradeSkillData];

            foreach (var lm in data.Children.Where(x => x.Key.EndsWith("csvSales")))
            {
                sharacterSaleModels.AddRange(ParseCsv<CharacterSaleModel>(lm));
            }

            CharacterSaleModels = sharacterSaleModels.ToImmutableArray();
        }

        private void PopulateData()
        {
            PopulateCharacterData();
            PopulateAuctionData();
            PopulateKnownItems();
        }

        private void PopulateExpiredAuctions()
        {
            //r@Korialstrasz@internalData@csvExpired
            List<ExpiredAuctionModel> expiredAuctions = new();
            LuaModel data = backingLuaModel[TradeSkillData];

            foreach (var lm in data.Children.Where(x => x.Key.EndsWith("csvExpired")))
            {
                expiredAuctions.AddRange(ParseCsv<ExpiredAuctionModel>(lm));
            }

            ExpiredAuctions = expiredAuctions.ToImmutableArray();
        }

        private void PopulateKnownItems()
        {
            //c@Mugaelai - Korialstrasz@internalData@auctionSaleHints
            const char separator = '\u0001';

            Dictionary<string, string> items = new();
            LuaModel data = backingLuaModel[TradeSkillData];

            foreach (LuaModel lm in data.Children.Where(x => x.Key.EndsWith("auctionSaleHints")))
            {
                var character = Characters.Single(c => c.Name == lm.Key[2..(lm.Key.IndexOf(' '))]);
                foreach (var sale in lm.Children)
                {
                    string[] keySplit = sale.Key.Split(separator);
                    items[keySplit[1]] = keySplit[0];
                }
            }

            Items = items.ToImmutableDictionary();
        }
    }
}