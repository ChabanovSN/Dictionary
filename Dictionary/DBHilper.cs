using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Dictionary
{
    public static class DBHilper
    {
        static SqliteConnection connection;
        static SqliteCommand command;
        static string cs = "Data Source=dictionary.db";
        static DBHilper()
        {

            if (!File.Exists("dictionary.db"))
            {
                Console.WriteLine("Please, create DB and  tables ");
                return;
            }
            try
            {

                connection = new SqliteConnection(cs);
                connection.Open();
                command = connection.CreateCommand();
                Console.WriteLine(connection.ServerVersion);

            }
            catch(Exception e) {
                Console.WriteLine("Ошибка подключения "+e.Message);
            }
        }

        static public Dictionary<string, int> GetDictionary()
        {
            Dictionary<string, int> pairs = new Dictionary<string, int>();

            command.CommandText = $"select id, fromName, toName from CommonTable;";                      //    command.Parameters.AddWithValue("$id",1);

           
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                   int id = reader.GetInt32(0);
                   string  fromName = reader.GetString(1);
                   string toName = reader.GetString(2);
                    string midName = fromName + "-" + toName;
                    Console.WriteLine($"CommonTable, { id}   {fromName} {toName} {midName} !");
                    pairs[midName] = id;
                }
            }
            return pairs;
        }
        static public List<string> GetListTranslaters(string fromTable,string toTable,string midTable, string word) {
            List<string> list = new List<string>();
            string translation;
            string idFrom = "id_" + fromTable;
            string idTo = "id_" + toTable;
            command.CommandText = $"select t.word from {fromTable} as f,{toTable} as t,{midTable} as m" +
            	$" where f.id=m.{idFrom} AND t.id =m.{idTo} ;";                      //    command.Parameters.AddWithValue("$id",1);
          
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {

                     translation = reader.GetString(0);                   
                    Console.WriteLine($" GetListTranslaters(), { translation} !");
                    list.Add(translation);
                }
            }

            return list;
           }

        static public Table GetTable(int id)
        {
            Table table = new Table();
            command.CommandText = $"select id, fromName, toName, fromTable, toTable, midTable from CommonTable where id=$id;";                      //    command.Parameters.AddWithValue("$id",1);
            command.Parameters.AddWithValue("$id", id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    table.id = reader.GetInt32(0);
                    table.fromName = reader.GetString(1);
                    table.toName = reader.GetString(2);
                    table.fromTable = reader.GetString(3);
                    table.toTable = reader.GetString(4);
                    table.midTable = reader.GetString(5);
                    Console.WriteLine($"GetTable(int id), { table} !");

                }
            }
            return table;

        }

        static string createEnglishTable = "BEGIN;" +
            "CREATE  TABLE IF NOT EXISTS EnglishToRussian (" +
            " id integer primary key autoincrement, word text UNIQUE);" +
            "COMMIT;";
        static string creatRussianTable = "CREATE  TABLE IF NOT EXISTS RussianToEnglish (" +
            " id integer primary key autoincrement, word text UNIQUE)";
        static string createMidEnglishRussian = "CREATE  TABLE IF NOT EXISTS MidEnglishRussian (" +
          // "id integer primary key autoincrement," +
          "id_EnglishToRussian  INTEGER," +
          "id_RussianToEnglish  INTEGER," +
            "CONSTRAINT new_pk PRIMARY KEY (id_EnglishToRussian, id_RussianToEnglish)" +
          ");";
        static string insertEnglish = "INSERT INTO EnglishToRussian(word) VALUES('Help');";
        static string insertRussian = "INSERT INTO RussianToEnglish(word) VALUES('Спасение');";


        static string CommonTable = "CREATE  TABLE IF NOT EXISTS CommonTable(id integer primary key autoincrement, " +
              "fromName text," +
           "toName text," +
                   "fromTable text," +
           "toTable text," +
           "midTable text)";
        static string insertCom = "INSERT INTO CommonTable(fromName,toName,fromTable,toTable,midTable)" +
            " VALUES('English','Русский','EnglishToRussian','RussianToEnglish','MidEnglishRussian')";

      

        public static bool Insert(string fromTable, string toTable, string midTable, string word, string translate)
        {

            int idFromTable = GetWordFromTable(fromTable, word);
            int idToTable = GetWordFromTable(toTable, translate);

            Console.WriteLine($"result 2 table, { idFromTable}   {idToTable}  !");

            string idFrom = "id_" + fromTable;
            string idTo = "id_" + toTable;
            command.CommandText = $"INSERT INTO {midTable}({idFrom},{idTo}) VALUES({idFromTable},{idToTable})";

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch {
                return false;
              }
        }

        static int InsertInTable(string table, string word)
        {
            command.CommandText = $"insert into {table} (word) values('{word}');";
            if (command.ExecuteNonQuery() > 0)
                return GetWordFromTable(table, word);
            else return 0;
        }
        static int GetWordFromTable(string table, string word)
        {
            int idFromTable = 0;
            // SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"select id from {table} where word ='{word}' ;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    idFromTable = reader.GetInt32(0);

                    Console.WriteLine($" {table}, { idFromTable }   !");
                }
            }
            if (idFromTable == 0)
                return InsertInTable(table, word);
            return idFromTable;
        }

        static void mainF(string[] args)
        {
            // string cs = "Data Source=:memory:";
            //   string cs = "Data Source=/home/doka/database.db";
            string cs = "Data Source=dictionary.db";

            if (!File.Exists("dictionary.db"))
            {
                Console.WriteLine("Please, create DB and  tables ");
                return;
            }
            try
            {

                using (connection = new SqliteConnection(cs))
                {
                    connection.Open();
                    command = connection.CreateCommand();

                    Console.WriteLine(connection.ServerVersion);

                  //  Insert("English", "Русский", "Help", "Спасение");


                    //   var command = connection.CreateCommand();


                    //    command.CommandText = createEnglishTable;//"SELECT id FROM dht";
                    //    command.CommandText = creatRussianTable;//"SELECT id FROM dht";
                    //  command.CommandText = createMidEnglishRussian;//"SELECT id FROM dht";

                    // string eng = "Help";
                    // string rus = "Помощ";

                    // string getEngId = $"select id from  EnglishToRussian where word = '{eng}';";
                    // string getRusId = $"select id from  RussianToEnglish where word = '{rus}';";

                    //  string insertToMid = "insert into MidEnglishRussian (id_EnglishToRussian,id_RussianToEnglish) values (1,1);";

                    // string qw = " select E.word,R.word from MidEnglishRussian as M,EnglishToRussian as E, RussianToEnglish as R where M.id_EnglishToRussian=E.id and M.id_RussianToEnglish=R.id;";

                    //command.CommandText = createMidEnglishRussian;                      //    command.Parameters.AddWithValue("$id",1);
                    // int rezult=  command.ExecuteNonQuery();

                    //Console.WriteLine($"Hello, { rezult}!");
                    // using (var reader = command.ExecuteReader())
                    //{
                    //     while (reader.Read())
                    //     {
                    //         var name = reader.GetInt32(0);

                    //        Console.WriteLine($"Hello, { name}!");
                    //    }
                    //}
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
