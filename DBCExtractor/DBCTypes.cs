namespace DBCExtractor
{
    struct ItemEntry
    {
        public uint ID;                                             // 0
        public uint Class;                                          // 1
        public uint SubClass;                                       // 2 some items have strange subclasses
        public int SoundOverrideSubclass;                          // 3
        public int Material;                                       // 4
        public uint DisplayId;                                      // 5
        public uint InventoryType;                                  // 6
        public uint Sheath;                                         // 7
    }
}
