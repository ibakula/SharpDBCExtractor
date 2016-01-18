namespace DBCExtractor
{
    struct dbc_header 
    {
        public char[] magic;
        public uint record_count;
        public uint field_count;
        public uint record_length;
        public uint string_block_length;
    }

    abstract class dbc<T>
    {
        protected dbc_header header;
        protected T[] records;
        protected char[] string_block;
    }
}
