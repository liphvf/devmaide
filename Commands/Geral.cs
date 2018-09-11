using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text;
using Npgsql;

namespace DevMaid.Commands
{
    public static class Geral
    {
        public static void CsvToClass(string inputFile = @"./arquivo.csv")
        {
            if (!File.Exists(inputFile))
            {
                throw new System.ArgumentException("Can`t find the input file");
            }

            string[] lines = File.ReadAllLines(inputFile);

            /*
             * select column_name, data_type, is_nullable from information_schema.columns where table_name = '';
             */
            foreach (string line in lines)
            {
                // Console.WriteLine(line);
                var quebrando = line.Split(",");
                if (quebrando.Length <= 0 || string.IsNullOrEmpty(line) || quebrando[0].Contains("column_name"))
                {
                    continue;
                }

                var tipos = new Dictionary<string, string>
                {
                    { "bigint" , "long"  },
                    { "binary" , "byte[]"  },
                    { "bit" , "bool"  },
                    { "char" , "string"  },
                    { "date" , "DateTime"  },
                    { "datetime" , "DateTime"  },
                    { "datetime2" , "DateTime"  },
                    { "datetimeoffset" , "DateTimeOffset"  },
                    { "decimal" , "decimal"  },
                    { "float" , "float"  },
                    { "image" , "byte[]"  },
                    { "int" , "int"  },
                    { "money" , "decimal"  },
                    { "nchar" , "char"  },
                    { "ntext" , "string"  },
                    { "numeric" , "decimal"  },
                    { "nvarchar" , "string"  },
                    { "real" , "double"  },
                    { "smalldatetime" , "DateTime"  },
                    { "smallint" , "short"  },
                    { "smallmoney" , "decimal"  },
                    { "text" , "string"  },
                    { "time" , "TimeSpan"  },
                    { "timestamp" , "DateTime"  },
                    { "tinyint" , "byte"  },
                    { "uniqueidentifier" , "Guid"  },
                    { "\"character varying\"", "string" },
                    { "character", "string" }
                };

                var nulo = quebrando[2] == "YES" ? "?" : "";
                var tabelainfo = new { coluna = quebrando[0], tipo = tipos.GetValueOrDefault(quebrando[1].Trim()), Nulo = nulo };
                // quebrando[2] = quebrando[2].Replace("'","''");

                var strbuild = new StringBuilder();

                strbuild.Append($"[Column(\"{tabelainfo.coluna}\")]");
                strbuild.Append("\n");
                strbuild.Append($"public {tabelainfo.tipo}");
                if (tabelainfo.tipo != "string")
                {
                    strbuild.Append($"{tabelainfo.Nulo}");
                }
                strbuild.Append($" {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tabelainfo.coluna)} " + "{ get; set; }");
                strbuild.Append("\n");


                using (System.IO.StreamWriter file = new StreamWriter(@"./saida.class", true))
                {
                    // Console.WriteLine(template);
                    file.WriteLine(strbuild.ToString());
                    //file.WriteLine("\n");
                }
            }
        }

        public static dynamic RunQuery(string sqlQuery)
        {
            var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    cmd.ExecuteNonQuery();
                }

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
            }

            return new ExpandoObject();
        }
        // Restore database: pg_restore -U <username> -d <dbname> -1 <filename>.dump
    }
}