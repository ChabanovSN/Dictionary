using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            try
            {
                if (!File.Exists("dictionary.db"))
                {
                    connection = new SqliteConnection(cs);
                    connection.Open();
                    command = connection.CreateCommand();
                    CreateCommonTable();
                }
                else
                {


                    connection = new SqliteConnection(cs);
                    connection.Open();
                    command = connection.CreateCommand();
                    Console.WriteLine(connection.ServerVersion);
                }
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
            	$" where f.id=m.{idFrom} AND t.id =m.{idTo} AND f.word = '{word}' COLLATE NOCASE;";
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        translation = reader.GetString(0);
                        Console.WriteLine($" GetListTranslaters(), { translation} !");
                        list.Add(translation);
                    }
                }
            }catch(Exception e) {
                Console.WriteLine($"  GetListTranslaters {e.Message}");
            
            }

            return list;
           }

        static public Table GetTable(int id)
        {
            Table table = new Table();
            try
            {
               
                command.CommandText = $"select id, fromName, toName, fromTable, toTable, midTable from CommonTable where id = {id} COLLATE NOCASE;";                      //    command.Parameters.AddWithValue("$id",1);
                                                                                                                                                                          // command.Parameters.AddWithValue("$id", id);

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
              
            }catch(Exception e) {
                Console.WriteLine(e.Message);
               

            }
            return table;

        }





        public static void CreateCommonTable() {
            try
            {
                string CommonTable = "CREATE  TABLE IF NOT EXISTS CommonTable(id integer primary key autoincrement, " +
                      "fromName text," +
                   "toName text," +
                           "fromTable text," +
                   "toTable text," +
                   "midTable text)";
                command.CommandText = CommonTable;
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
     
        public static void CreateNewDictionary(string fromName, string toName, string fromTable, string toTable, string midTable)
        {
            try
            {
                int idPair = CheckTable(fromName, toName);
                if (idPair != 0) return;

                int idFromTable = CheckTable(fromName, null);

                if (idFromTable == 0)
                        CreateNewTable(fromTable);
                int idToTable = CheckTable(null, toName);
                if (idToTable == 0)
                        CreateNewTable(toTable);

                string idFrom = "id_" + fromTable;
                string idTo = "id_" + toTable;
                string createMid = $"CREATE  TABLE IF NOT EXISTS {midTable} (" +      
                                   $"{idFrom}  INTEGER," +
                                   $"{idTo}   INTEGER," +
                                   $"CONSTRAINT new_pk PRIMARY KEY ({idFrom}, {idTo})" +
                                                                             ");";

                command.CommandText = createMid;
                command.ExecuteNonQuery();
                string insertComTable = $"INSERT INTO CommonTable(fromName,toName,fromTable,toTable,midTable)" +
                                        $" VALUES('{fromName}','{toName}','{fromTable}','{toTable}','{midTable}')";
                command.CommandText = insertComTable;
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {

                Console.WriteLine(e.Message);
            }



        }

        static void CreateNewTable(string table){
            command.CommandText = $"BEGIN;" +
              $"CREATE  TABLE IF NOT EXISTS {table} (" +
              " id integer primary key autoincrement, word text UNIQUE);" +
              "COMMIT;";
          
            command.ExecuteNonQuery();

        }

        static int CheckTable(string fromName, string toName) {
            int id = 0;
            try
            {
                if(fromName !=null && toName ==null)
                command.CommandText = $"select id from CommonTable where fromName = '{fromName}' COLLATE NOCASE;";
                if (fromName == null && toName != null)
                    command.CommandText = $"select id from CommonTable where toName = '{toName}' COLLATE NOCASE;";                     //    command.Parameters.AddWithValue("$id",1);
                if (fromName != null && toName != null)
                    command.CommandText = $"select id from CommonTable where fromName = '{fromName}' AND toName = '{toName}'  COLLATE NOCASE;";                                                                                                               // command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       id = reader.GetInt32(0);                       
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);


            }
            return id;
        
        }


        public static bool Insert(string fromTable, string toTable, string midTable, string word, string translate)
        {
            try
            {
                int idFromTable = GetWordFromTable(fromTable, word);
            int idToTable = GetWordFromTable(toTable, translate);

            Console.WriteLine($"insert id, { idFromTable}   {idToTable}  !");
            Console.WriteLine($"insert, {word}   {translate}  !");
            string idFrom = "id_" + fromTable;
            string idTo = "id_" + toTable;
            command.CommandText = $"INSERT INTO {midTable}({idFrom},{idTo}) VALUES({idFromTable},{idToTable})";


                command.ExecuteNonQuery();
                return true;
            }
            catch {
                return false;
              }
        }

        public static bool Delete(string fromTable, string toTable, string midTable, string word, string translate)
        {
            try
            {
                int idFromTable = GetWordFromTable(fromTable, word);
            int idToTable = GetWordFromTable(toTable, translate);

            Console.WriteLine($"!!! { idFromTable}  {idToTable} ");

            string idFrom = "id_" + fromTable;
            string idTo = "id_" + toTable;
            command.CommandText = $"DELETE FROM {midTable} where {idFrom}='{idFromTable}' AND {idTo} = '{idToTable}';";


                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int InsertInFROMTable(string table, string word)
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
            command.CommandText = $"select id from {table} where word ='{word}' COLLATE NOCASE;";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    idFromTable = reader.GetInt32(0);

                    Console.WriteLine($" {table}, { idFromTable }   !");
                }
            }
            if (idFromTable == 0)
                return InsertInFROMTable(table, word);
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
