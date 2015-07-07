using AdvDevTeam.HiraTxtToSql.Core.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.ConsoleApp
{
    class Program
    {
        private const string DEFAULT_ZIP_FILENAME = @"Resources\HiraTxt.zip";
        private const string APP_PATH = "{APP_PATH}";
        private const string DEFAULT_CONNECTION_STRING = "Data Source=" + APP_PATH + @"\hira.sqlite;Version=3;";

        private static string _zipFilename = "";
        private static string _connectionString = "";

        /// <summary>
        /// Entry-point of the app
        /// </summary>
        /// <param name="args">
        /// - first argument should be the zip path
        /// </param>
        static void Main(string[] args)
        {
            string currentFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // gets static parameters from exe arguments
            if (args != null && args.Length >= 1)
            {
                _zipFilename = args[0];
            }
            if (args != null && args.Length >= 2)
            {
                _connectionString = args[1];
            }

            // check if the zip filename is provided,
            // if it is not provided, then set it to default one
            // (there should be a zip provided in .\Resources\)
            if (string.IsNullOrWhiteSpace(_zipFilename) || !File.Exists(_zipFilename))
            {
                _zipFilename = Path.Combine(currentFolderPath, DEFAULT_ZIP_FILENAME);
                if (!File.Exists(_zipFilename))
                {
                    Console.WriteLine("Zip file that contains txt hymns is not provided or not found!");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(_connectionString))
                _connectionString = DEFAULT_CONNECTION_STRING;
            // replace {APP_PATH} by current path
            _connectionString = _connectionString.Replace(APP_PATH, currentFolderPath);

            IProgressLogger progressLogger = new ConsoleProgressLogger();
            IDataRepository dataRepository = new SQLiteDataRepository(_connectionString, progressLogger);
            dataRepository.CreateSchemaIfNotExists();
            TxtImporter importer = new TxtImporter(dataRepository, progressLogger) { InputArhiveFilename = _zipFilename };
            importer.Import();

            Console.WriteLine();
            Console.WriteLine("Type [Return] key to exit...");
            Console.ReadLine();
        }
    }
}
