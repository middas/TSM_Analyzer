using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM.Core.Models;
using TSM.Logic.Data_Parser.Exceptions;
using TSM.Logic.Data_Parser.Models;

namespace TSM.Logic.Data_Parser
{
    public class TsmBackupParser
    {
        private const string ZipExtension = ".zip";
        private const string LuaFileExtension = ".lua";
        private const string DefaultLuaBackupFileName = "TradeSkillMaster.lua";

        public async Task<BackupModel> ParseBackup(FileInfo backupPath)
        {
            if (backupPath == null) throw new ArgumentNullException(nameof(backupPath));
            if (!backupPath.Exists) throw new FileNotFoundException();

            bool doCleanup = false;
            if (backupPath.Extension.Equals(ZipExtension, StringComparison.OrdinalIgnoreCase))
            {
                doCleanup = true;
                backupPath = await ExtractBackupZipContents(backupPath);
            }

            if (!backupPath.Extension.Equals(LuaFileExtension, StringComparison.OrdinalIgnoreCase)) throw new InvalidBackupException("Not a valid lua backup.");

            LuaModel luaModel = await LuaParser.ParseLua(backupPath);

            if (doCleanup && backupPath.Directory != null)
            {
                Directory.Delete(backupPath.Directory.FullName, true);
            }

            BackupModel backupModel = new();
            return backupModel;
        }

        private async Task<FileInfo> ExtractBackupZipContents(FileInfo backupPath)
        {
            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                using FileStream fs = new(backupPath.FullName, FileMode.Open, FileAccess.Read);
                using var zipArchive = ArchiveFactory.Open(fs);
                var entry = zipArchive.Entries.FirstOrDefault(e => e.Key.Equals(DefaultLuaBackupFileName, StringComparison.OrdinalIgnoreCase));

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
    }
}
