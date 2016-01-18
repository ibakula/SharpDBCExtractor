using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DBCExtractor
{
    class Database
    {
        private MySqlConnection _connection;
        public List<ItemEntry> ItemsList;
        public Database(string details)
        {
            _connection = new MySqlConnection(details);
            ItemsList = new List<ItemEntry>();
        }
        public void SelectItems()
        {
            using (_connection)
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT entry, class, subclass, SoundOverrideSubclass, Material, displayid, InventoryType, sheath FROM item_template ORDER BY entry ASC", _connection))
                {
                    _connection.Open();
                    MySqlDataReader _data = cmd.ExecuteReader();
                    while (_data.HasRows)
                    {
                        while (_data.Read())
                        {
                            ItemEntry ie = new ItemEntry();
                            ie.ID = _data.GetUInt32("entry");
                            ie.Class = _data.GetUInt32("class");
                            ie.SubClass = _data.GetUInt32("subclass");
                            ie.SoundOverrideSubclass = _data.GetInt32("SoundOverrideSubclass");
                            ie.Material = _data.GetInt32("Material");
                            ie.DisplayId = _data.GetUInt32("displayid");
                            ie.InventoryType = _data.GetUInt32("InventoryType");
                            ie.Sheath = _data.GetUInt32("sheath");
                            ItemsList.Add(ie);
                        }
                        _data.NextResult();
                    }
                }
            }
        }
    }
}
