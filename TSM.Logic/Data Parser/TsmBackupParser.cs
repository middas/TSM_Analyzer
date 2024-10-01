using SharpCompress.Archives;
using System.Collections.Immutable;
using TSM.Core.Models;
using TSM.Logic.Data_Parser.Exceptions;
using TSM.Logic.Extensions;

namespace TSM.Logic.Data_Parser
{
    public class TsmBackupParser
    {
        private const string DefaultLuaBackupFileName = "TradeSkillMaster.lua";

        private const string LuaFileExtension = ".lua";

        private const string ZipExtension = ".zip";

        public event Action<string> OnStatusUpdated;

        public static async Task<BackupModel> ParseBackup(FileInfo backupPath)
        {
            if (backupPath == null)
            {
                throw new ArgumentNullException(nameof(backupPath));
            }

            if (!backupPath.Exists)
            {
                throw new FileNotFoundException();
            }

            bool doCleanup = false;
            if (backupPath.Extension.Equals(ZipExtension, StringComparison.OrdinalIgnoreCase))
            {
                doCleanup = true;
                backupPath = await ExtractBackupZipContents(backupPath);
            }

            if (!backupPath.Extension.Equals(LuaFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidBackupException("Not a valid lua backup.");
            }

            LuaModel luaModel = await LuaParser.ParseLua(backupPath);

            if (doCleanup && backupPath.Directory != null)
            {
                Directory.Delete(backupPath.Directory.FullName, true);
            }

            BackupModel backupModel = new(luaModel);
            return backupModel;
        }

        public async Task<Tuple<BackupModel, ScanFile[]>> ParseBackups(DirectoryInfo backupDirectory, string[] backupsScanned)
        {
            HashSet<Character> characters = new();
            HashSet<AuctionBuyModel> auctionBuyModels = new();
            HashSet<CharacterSaleModel> characterSaleModels = new();
            HashSet<ExpiredAuctionModel> expiredAuctionModels = new();
            HashSet<CancelledAuctionModel> cancelledAuctionModels = new();
            Dictionary<string, string> itemNames = new();
            List<ScanFile> scannedFiles = new();
            long warBankMoney = 0;
            bool warBankFound = false;

            foreach (FileInfo? backupFile in backupDirectory.GetFiles().OrderBy(f => f.CreationTime))
            {
                if (backupsScanned.Contains(backupFile.FullName))
                {
                    continue;
                }

                OnStatusUpdated?.Invoke($"Parsing {backupFile.Name}");
                BackupModel backup = await ParseBackup(backupFile);

                DateTimeOffset startTime = DateTimeOffset.Now;
                characters.UnionWith(backup.Characters);

                auctionBuyModels.UnionWith(backup.AuctionBuys);

                characterSaleModels.UnionWith(backup.CharacterSaleModels);

                expiredAuctionModels.UnionWith(backup.ExpiredAuctions);

                cancelledAuctionModels.UnionWith(backup.CancelledAuctions);

                scannedFiles.Add(new ScanFile(backupFile, startTime));

                _ = itemNames.MergeLeft(backup.Items);

                warBankMoney = backup.WarBankMoney;
                if (backup.WarBankFound)
                {
                    warBankFound = true;
                }
            }

            return new Tuple<BackupModel, ScanFile[]>(new BackupModel(auctionBuyModels, cancelledAuctionModels, characters,
                characterSaleModels, expiredAuctionModels, itemNames, warBankMoney, warBankFound), scannedFiles.ToArray());
        }

        private static async Task<FileInfo> ExtractBackupZipContents(FileInfo backupPath)
        {
            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                _ = Directory.CreateDirectory(tempDir);

                using FileStream fs = new(backupPath.FullName, FileMode.Open, FileAccess.Read);
                using IArchive zipArchive = ArchiveFactory.Open(fs);
                IArchiveEntry? entry = zipArchive.Entries.FirstOrDefault(e => e.Key.Equals(DefaultLuaBackupFileName, StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                {
                    throw new InvalidBackupException("Archive does not contain a valid lua backup.");
                }

                using FileStream writeStream = new(Path.Combine(tempDir, entry.Key), FileMode.Create, FileAccess.ReadWrite);
                using Stream entryStream = entry.OpenEntryStream();
                byte[] buffer = new byte[4096];
                int read;
                while ((read = await entryStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
                {
                    await writeStream.WriteAsync(buffer.AsMemory(0, read));
                }

                return new FileInfo(writeStream.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public struct ScanFile
        {
            public FileInfo FileName;

            public DateTimeOffset ScanTime;

            public ScanFile(FileInfo fileName, DateTimeOffset scanTime)
            {
                FileName = fileName;
                ScanTime = scanTime;
            }
        }
    }
}