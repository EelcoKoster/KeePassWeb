using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using KeePassWeb.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeePassWeb.Data
{
    public class KeePassApi : IDisposable
    {
        private PwDatabase _db;

        public KeePassApi(IConfiguration config)
        {
            var dbpath = config.GetValue<string>("KeePassFile"); 
            var masterpw = config.GetValue<string>("KeePassword");

            var ioConnInfo = new IOConnectionInfo { Path = dbpath };
            var compKey = new CompositeKey();
            compKey.AddUserKey(new KcpPassword(masterpw));

            if (!File.Exists(dbpath))
            {
                var newDb = new PwDatabase();
                newDb.New(ioConnInfo, compKey);
                newDb.Save(null);
                newDb.Close();
            }

            _db = new PwDatabase();
            _db.Open(ioConnInfo, compKey, null);
        }

        public KeePassEntry GetEntry(string id)
        {
            return GetEntries().FirstOrDefault(e => e.ID == id);
        }

        public string GetEntryPassword(string id)
        {
            return GetEntry(id).Password;
        }

        public List<KeePassEntry> GetEntries()
        {
            var kpdata = from entry in _db.RootGroup.GetEntries(true)
                         select new KeePassEntry
                         {
                             Group = entry.ParentGroup.Name,
                             Title = entry.Strings.ReadSafe("Title"),
                             Username = entry.Strings.ReadSafe("UserName"),
                             Password = entry.Strings.ReadSafe("Password"),
                             URL = entry.Strings.ReadSafe("URL"),
                             Notes = entry.Strings.ReadSafe("Notes"),
                             ID = MemUtil.ByteArrayToHexString(entry.Uuid.UuidBytes)
                         };
            return kpdata.ToList();
        }

        public void AddEntry(KeePassEntry entry)
        {
            var addEntry = new PwEntry(true, true);
            addEntry.Strings.Set("Title", new KeePassLib.Security.ProtectedString(true, entry.Title));
            addEntry.Strings.Set("UserName", new KeePassLib.Security.ProtectedString(true, entry.Username));
            addEntry.Strings.Set("Password", new KeePassLib.Security.ProtectedString(true, entry.Password));
            if (!string.IsNullOrEmpty(entry.URL)) addEntry.Strings.Set("URL", new KeePassLib.Security.ProtectedString(true, entry.URL));
            if (!string.IsNullOrEmpty(entry.Notes)) addEntry.Strings.Set("Notes", new KeePassLib.Security.ProtectedString(true, entry.Notes));
            var group = _db.RootGroup.FindCreateGroup(entry.Group, true);
            group.AddEntry(addEntry, true);
            _db.Save(null);
        }

        public void EditEntry(KeePassEntry entry)
        {
            if (entry.Password == "**********")
            {
                entry.Password = GetEntryPassword(entry.ID);
            }
            RemoveEntry(entry.ID);
            var updateEntry = new PwEntry(false, true);
            updateEntry.Uuid = new PwUuid(MemUtil.HexStringToByteArray(entry.ID));
            updateEntry.Strings.Set("Title", new KeePassLib.Security.ProtectedString(true, entry.Title));
            updateEntry.Strings.Set("UserName", new KeePassLib.Security.ProtectedString(true, entry.Username));
            updateEntry.Strings.Set("Password", new KeePassLib.Security.ProtectedString(true, entry.Password));
            if (!string.IsNullOrEmpty(entry.URL)) updateEntry.Strings.Set("URL", new KeePassLib.Security.ProtectedString(true, entry.URL));
            if (!string.IsNullOrEmpty(entry.Notes)) updateEntry.Strings.Set("Notes", new KeePassLib.Security.ProtectedString(true, entry.Notes));

            var group = _db.RootGroup.FindCreateGroup(entry.Group, true);
            group.AddEntry(updateEntry, true);
            _db.Save(null);
        }

        public void RemoveEntry(string id)
        {
            var uuid = new PwUuid(MemUtil.HexStringToByteArray(id));
            var entry = _db.RootGroup.FindEntry(uuid, true);
            var group = _db.RootGroup.FindGroup(entry.ParentGroup.Uuid, true);
            group.Entries.Remove(entry);
            _db.Save(null);
        }

        public void Dispose()
        {
            _db.Close();
        }

    }
}
