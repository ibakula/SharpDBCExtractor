using System;
using System.Text;

namespace DBCExtractor
{
    class Program
    {
        static void Main()
        {
            DBCStore<ItemEntry> store = null;
            string answer = " ";
            while (!(answer.Contains("y") || answer.Contains("n") ? true : false))
            {
                Console.WriteLine("Would you like to append/update data to an existing Item.dbc file? (y/n)");
                answer = Console.ReadLine();
            }

            try
            {
                if (answer == "y")
                {
                    Console.WriteLine("DBC file must be in the same directory as program.");
                    Console.WriteLine("Press any key when ready..");
                    Console.ReadKey();
                    store = new ItemDBCStorage("item.dbc");
                }
                else store = new ItemDBCStorage("");
                string connection = "";
                string[,] MySQLDetails = new string[4, 2] { { "Enter MySQL Host", "Server=" }, { "Enter MySQL Username", "UserId=" }, { "Enter MySQL Password", "Password=" }, { "Enter MySQL Database name", "Database=" } };
                for (short i = 0; i < 4; ++i)
                {
                    Console.WriteLine(MySQLDetails[i,0]);
                    string arg = Console.ReadLine();
                    connection += MySQLDetails[i,1] + arg + ";";
                }
                Database db = new Database(connection);
                connection = null;
                MySQLDetails = null;
                db.SelectItems();
                store.insertItemsDataFromSQL(db.ItemsList);
                Console.Write("What would you like to name the new modified file?");
                while(!store.CloseAndSaveToFile(Console.ReadLine()))
                {
                    Console.WriteLine("File exists, please use an unused name.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has occured: " + e.Message);
            }
            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}
