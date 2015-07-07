using AdvDevTeam.HiraTxtToSql.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    public class SQLiteDataRepository : DataRepositoryBase
    {
        private string _fileName = string.Empty;
        private SQLiteConnection _dbConnection = null;

        public SQLiteDataRepository(string connectionString, IProgressLogger progressLogger)
            : base(connectionString, progressLogger)
        {
            SQLiteConnectionStringBuilder cnBuilder = new SQLiteConnectionStringBuilder(connectionString);
            this._fileName = cnBuilder.DataSource;
        }

        public override void CreateSchemaIfNotExists()
        {
            if (!File.Exists(this._fileName))
                SQLiteConnection.CreateFile(this._fileName);
            this.OpenConnection();
            string query = "";

            query = string.Format("DROP TABLE IF EXISTS {0}", CommonConstants.DatabaseTables.TABLE_HYMN_NAME);
            this.ExecuteNonQuery(query, CommandType.Text);
            query = string.Format("DROP TABLE IF EXISTS {0}", CommonConstants.DatabaseTables.TABLE_STANZA_NAME);
            this.ExecuteNonQuery(query, CommandType.Text);
            query = string.Format("CREATE TABLE {0} ({1} INTEGER, {2} TEXT)"
                                        , CommonConstants.DatabaseTables.TABLE_HYMN_NAME
                                        , CommonConstants.DatabaseTables.FIELD_HYMN_NUMBER_NAME
                                        , CommonConstants.DatabaseTables.FIELD_HYMN_TITLE_NAME);
            this.ExecuteNonQuery(query, CommandType.Text);
            query = string.Format("CREATE TABLE {0} ({1} INTEGER, {2} INTEGER, {3} TEXT)"
                                        , CommonConstants.DatabaseTables.TABLE_STANZA_NAME
                                        , CommonConstants.DatabaseTables.FIELD_STANZA_HYMN_NUMBER_NAME
                                        , CommonConstants.DatabaseTables.FIELD_STANZA_NUMBER_NAME
                                        , CommonConstants.DatabaseTables.FIELD_STANZA_CONTENT_NAME
                                        );
            this.ExecuteNonQuery(query, CommandType.Text);
        }

        protected override IDbConnection GetConnection()
        {
            if (this._dbConnection == null)
                this._dbConnection = new SQLiteConnection(ConnectionString);
            return this._dbConnection;
        }

        protected override IDbCommand CreateCommand(string query, CommandType commandType, params IDbDataParameter[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand(query, this.GetConnection() as SQLiteConnection);
            if (parameters != null && parameters.Any())
                command.Parameters.AddRange(parameters);
            return command;
        }

        protected override IDbDataParameter CreateParameter(string name, object value)
        {
            name = "$" + name;
            SQLiteParameter parameter = new SQLiteParameter(name, value);
            return parameter;
        }

        protected override void ExecuteNonQuery(string query, CommandType commandType, params IDbDataParameter[] parameters)
        {
            // in sqlite, we use '$' instead of '@'
            // yes, I know, it's really dirty to do like I do here :p
            query = query.Replace("@", "$");
            base.ExecuteNonQuery(query, commandType, parameters);
        }
    }
}
