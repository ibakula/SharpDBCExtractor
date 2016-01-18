using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DBCExtractor
{
    class ItemDBCStorage : DBCStore<ItemEntry>
    {
        public override bool CloseAndSaveToFile(string newFileName)
        {
            if (File.Exists(newFileName))
                return false;

            using (BinaryWriter dbc = new BinaryWriter(File.Create(newFileName)))
            {
                dbc.Write(header.magic);
                dbc.Write(header.record_count);
                dbc.Write(header.field_count);
                dbc.Write(header.record_length);
                dbc.Write(header.string_block_length);
                for (uint i = 0; i < records.Length; ++i)
                {
                    dbc.Write(records[i].ID);
                    dbc.Write(records[i].Class);
                    dbc.Write(records[i].SubClass);
                    dbc.Write(records[i].SoundOverrideSubclass);
                    dbc.Write(records[i].Material);
                    dbc.Write(records[i].DisplayId);
                    dbc.Write(records[i].InventoryType);
                    dbc.Write(records[i].Sheath);
                }
                dbc.Write(string_block);
            }
            return true;
        }

        public override bool ItemExists(uint itemID, ref ItemEntry[] recordsN)
        {
            for (int i = 0; i < header.record_count; ++i)
                if (recordsN[i].ID == itemID)
                    return true;

            return false;
        }

        public override void insertItemsDataFromSQL(List<ItemEntry> itemsList)
        {
            ItemEntry[] recordsN = new ItemEntry[header.record_count + 1];
            if (records == null)
            {
                header.record_count += 1;
                header.record_length += (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(ItemEntry));
                header.field_count += 8;
                header.string_block_length += 1;
                records = new ItemEntry[recordsN.Length];
                string_block = new char[1] { '\0' };
            }
            else
            {
                Array.Copy(records, recordsN, records.Length);
            }
            foreach (ItemEntry item in itemsList)
            {
                if (!ItemExists(item.ID, ref recordsN))
                {
                    recordsN[recordsN.Length - 1] = item;
                    int size = recordsN.Length + 1;
                    Array.Resize<ItemEntry>(ref recordsN, size);
                }
            }
            records = new ItemEntry[recordsN.Length - 1];
            Array.Copy(recordsN, records, recordsN.Length - 1);
            header.record_count = (uint)records.Length;
        }

        public override void insertDataFromDBC(BinaryReader dbc)
        {
            for (uint i = 0; i < records.Length; ++i)
            {
                records[i].ID = dbc.ReadUInt32();
                records[i].Class = dbc.ReadUInt32();
                records[i].SubClass = dbc.ReadUInt32();
                records[i].SoundOverrideSubclass = dbc.ReadInt32();
                records[i].Material = dbc.ReadInt32();
                records[i].DisplayId = dbc.ReadUInt32();
                records[i].InventoryType = dbc.ReadUInt32();
                records[i].Sheath = dbc.ReadUInt32();
            }
            string_block = dbc.ReadChars(string_block.Length);
        }

        public ItemDBCStorage(string fileName)
        {
            _fileName = fileName;
            createHeader();
        }
    }

    abstract class DBCStore<T> : dbc<T>
    {
        protected string _fileName;

        public abstract bool CloseAndSaveToFile(string newFileName);

        public abstract bool ItemExists(uint itemID, ref ItemEntry[] recordsN);

        public abstract void insertItemsDataFromSQL(List<ItemEntry> itemsList);

        public abstract void insertDataFromDBC(BinaryReader dbc);

        protected void createHeader()
        {
            if (File.Exists(_fileName)) // In case we want to append.
            {
                using (BinaryReader _dbc = new BinaryReader(File.OpenRead(_fileName)))
                {
                    header = new dbc_header { magic = _dbc.ReadChars(4), record_count = _dbc.ReadUInt32(), field_count = _dbc.ReadUInt32(), record_length = _dbc.ReadUInt32(), string_block_length = _dbc.ReadUInt32() };
                    records = new T[header.record_count];
                    string_block = new char[header.string_block_length];
                    insertDataFromDBC(_dbc);
                }
            }
            else
            {
                header = new dbc_header { magic = new char[] { 'W', 'D', 'B', 'C' }, record_count = 0, field_count = 0, record_length = 0, string_block_length = 0 };
                records = null;
                string_block = null;
            }
        }
    }
}
