using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Collections.ObjectModel;
using Npgsql;

// https://www.dotnetperls.com/serialize-list
// https://www.daveoncsharp.com/2009/07/xml-serialization-of-collections/


namespace Lab2Solution
{ 
    /// <summary>
    /// This is the database class, currently a FlatFileDatabase
    /// </summary>
    public class RelationalDatabase : IDatabase
    {

        String connectionString;
        ObservableCollection<Entry> entries = new ObservableCollection<Entry>();


        /// <summary>
        /// Here or thereabouts initialize a connectionString that will be used in all the SQL calls
        /// </summary>
        public RelationalDatabase()
        {
            connectionString = InitializeConnectionString();
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
        }


        /// <summary>
        /// Adds an entry to the database
        /// </summary>
        /// <param name="entry">the entry to add</param>
        public void AddEntry(Entry entry)
        {
           
            entry.Id = entries.Count + 1;
            entries.Add(entry);

            using var con = new NpgsqlConnection(connectionString);
            con.Open();

            String sql = $"INSERT INTO entries VALUES ('{entry.Id}', '{entry.Clue}', '{entry.Answer}', '{entry.Difficulty}', '{entry.Date}');";

            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            con.Close();
           
        }


        /// <summary>
        /// Finds a specific entry
        /// </summary>
        /// <param name="id">id to find</param>
        /// <returns>the Entry (if available)</returns>
        public Entry FindEntry(int id)
        {
            foreach (Entry entry in entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes an entry 
        /// </summary>
        /// 
        /// <param name="entry">An entry, which is presumed to exist</param>
        public bool DeleteEntry(Entry entry)
        {
            
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            String sql = $"DELETE FROM entries where id = '{entry.Id}';";

            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var result = entries.Remove(entry);
            con.Close();

            return true;
        }

        /// <summary>
        /// Edits an entry
        /// </summary>
        /// <param name="replacementEntry"></param>
        /// <returns>true if editing was successful</returns>
        public bool EditEntry(Entry replacementEntry)
        {
            foreach (Entry entry in entries) // iterate through entries until we find the Entry in question
            {
                if (entry.Id == replacementEntry.Id) // found it
                {
                    entry.Answer = replacementEntry.Answer;
                    entry.Clue = replacementEntry.Clue;
                    entry.Difficulty = replacementEntry.Difficulty;
                    entry.Date = replacementEntry.Date;

                   
                    using var con = new NpgsqlConnection(connectionString);
                    con.Open();
                    String sql = $"UPDATE entries SET id = '{replacementEntry.Id}', clue = '{replacementEntry.Clue}', answer = '{replacementEntry.Answer}', difficulty = '{replacementEntry.Difficulty}', date = '{replacementEntry.Date}' where id = '{entry.Id}';";

                    using var cmd = new NpgsqlCommand(sql, con);
                    using NpgsqlDataReader reader = cmd.ExecuteReader();
                    con.Close();

                    return true;

                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves all the entries ordered by clue
        /// </summary>
        /// <returns>all of the entries</returns>
        public ObservableCollection<Entry> SortByClue()
        {
            return GetEntries("SELECT * FROM \"entries\" ORDER BY clue ASC limit 10;");

        }

        /// <summary>
        /// Retrieves all the entries ordered by answer
        /// </summary>
        /// <returns>all of the entries</returns>
        public ObservableCollection<Entry> SortByAnswer()
        {
            return GetEntries("SELECT * FROM \"entries\" ORDER BY answer ASC limit 10;");
        }

        /// <summary>
        /// Retrieves all the entries
        /// </summary>
        /// <returns>all of the entries</returns>
        public ObservableCollection<Entry> GetEntries()
        {
            return GetEntries("SELECT * FROM \"entries\" limit 10;");
        }

        /// <summary>
        /// Retrieves all the entries based on a specific sql
        /// </summary>
        /// <returns>all of the entries</returns>
        private ObservableCollection<Entry> GetEntries(String sql)
        {
            while (entries.Count > 0)
            {
                entries.RemoveAt(0);
            }

            using var con = new NpgsqlConnection(connectionString);
            con.Open();

            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Columns are clue, answer, difficulty, date, id in that order ...
            // Show all data
            while (reader.Read())
            {
                for (int colNum = 0; colNum < reader.FieldCount; colNum++)
                {
                    Console.Write(reader.GetName(colNum) + "=" + reader[colNum] + " ");
                }
                Console.Write("\n");
                entries.Add(new Entry(reader[1] as String, reader[2] as String, (int)reader[3], reader[4] as String, (int)reader[0]));
            }

            con.Close();



            return entries;
        }

        /// <summary>
        /// Creates the connection string to be utilized throughout the program
        /// 
        /// </summary>
        public String InitializeConnectionString()
        {
            var bitHost = "db.bit.io";
            var bitApiKey = "v2_3ugGU_ByXzDCRTXu38i2Ms287p5ZD"; // from the "Password" field of the "Connect" menu

            var bitUser = "ahmadd60";
            var bitDbName = "ahmadd60/Lab3DB";

            return connectionString = $"Host={bitHost};Username={bitUser};Password={bitApiKey};Database={bitDbName}";
        }
    }
}