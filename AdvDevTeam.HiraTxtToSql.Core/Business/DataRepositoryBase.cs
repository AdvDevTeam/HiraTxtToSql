using AdvDevTeam.HiraTxtToSql.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    /// <summary>
    /// Base-class for data repository classes
    /// </summary>
    public abstract class DataRepositoryBase : IDataRepository
    {
        protected readonly IProgressLogger _progressLogger;

        public virtual string ConnectionString { get; set; }

        public DataRepositoryBase(string connectionString, IProgressLogger progressLogger)
        {
            this.ConnectionString = connectionString;
            this._progressLogger = progressLogger;
        }
        public abstract void CreateSchemaIfNotExists();

        protected abstract IDbConnection GetConnection();
        protected virtual void OpenConnection()
        {
            IDbConnection connection = this.GetConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }
        protected virtual void CloseConnection()
        {
            IDbConnection connection = this.GetConnection();
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }
        protected abstract IDbCommand CreateCommand(string query, CommandType commandType, params IDbDataParameter[] parameters);
        protected virtual void ExecuteNonQuery(string query, CommandType commandType, params IDbDataParameter[] parameters)
        {
            IDbCommand command = this.CreateCommand(query, commandType, parameters);
            command.ExecuteNonQuery();
        }
        protected abstract IDbDataParameter CreateParameter(string name, object value);
        public virtual void InsertHymns(List<Hymn> hymns, bool clearExisting = true)
        {
            string query = "";

            if (clearExisting)
            {
                this._progressLogger.WriteLine("Clearing existing hymns...");
                // deletes all existing hymns
                query = string.Format("DELETE FROM {0}", CommonConstants.DatabaseTables.TABLE_HYMN_NAME);
                this.ExecuteNonQuery(query, CommandType.Text);
                // deletes all existing stanzas
                query = string.Format("DELETE FROM {0}", CommonConstants.DatabaseTables.TABLE_STANZA_NAME);
                this.ExecuteNonQuery(query, CommandType.Text);
                this._progressLogger.WriteLine("Existing hymns cleared.");
            }
            this._progressLogger.WriteLine("Inserting new hymns...");
            Hymn hymn = null;
            Stanza stanza = null;
            string hymnQuery = string.Format("INSERT INTO {0} ({1}, {2}) VALUES "
                                            , CommonConstants.DatabaseTables.TABLE_HYMN_NAME
                                            , CommonConstants.DatabaseTables.FIELD_HYMN_NUMBER_NAME
                                            , CommonConstants.DatabaseTables.FIELD_HYMN_TITLE_NAME);
            string stanzaQuery = string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES "
                                                , CommonConstants.DatabaseTables.TABLE_STANZA_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_HYMN_NUMBER_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_NUMBER_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_CONTENT_NAME);
            List<IDbDataParameter> hymnParameters = new List<IDbDataParameter>();
            List<IDbDataParameter> stanzaParameters = new List<IDbDataParameter>();
            string hymnNumberName = "";
            string hymnTitleName = "";
            string stanzaHymnNumberName = "";
            string stanzaNumberName = "";
            string stanzaContentName = "";
            for (int i = 0; i <= hymns.Count - 1; i++ )
            {
                hymn = hymns[i];
                
                hymnNumberName = string.Format("n{0}", i);
                hymnTitleName = string.Format("t{0}", i);
                hymnQuery += string.Format("(@{0}, @{1}),", hymnNumberName, hymnTitleName);
                hymnParameters.Add(this.CreateParameter(hymnNumberName, hymn.Number));
                hymnParameters.Add(this.CreateParameter(hymnTitleName, hymn.Title));

                if (hymnParameters.Count >= 500)
                {
                    // to avoid reaching the maximum variables count, we split the query into multiple chunks
                    this._progressLogger.Write("=>");
                    hymnQuery = this.RemoveLastCharsIfExisting(hymnQuery, ",");
                    this.ExecuteNonQuery(hymnQuery, CommandType.Text, hymnParameters.ToArray());
                    hymnQuery = string.Format("INSERT INTO {0} ({1}, {2}) VALUES "
                                            , CommonConstants.DatabaseTables.TABLE_HYMN_NAME
                                            , CommonConstants.DatabaseTables.FIELD_HYMN_NUMBER_NAME
                                            , CommonConstants.DatabaseTables.FIELD_HYMN_TITLE_NAME);
                    hymnParameters.Clear();
                }
                
                // inserts stanzas
                for (int j = 0; j <= hymn.Stanzas.Count - 1; j++ )
                {
                    stanza = hymn.Stanzas[j];

                    stanzaHymnNumberName = string.Format("shn{0}{1}", i, j);
                    stanzaNumberName = string.Format("sn{0}{1}", i, j);
                    stanzaContentName = string.Format("sc{0}{1}", i, j);
                    stanzaQuery += string.Format("(@{0}, @{1}, @{2}),", stanzaHymnNumberName, stanzaNumberName, stanzaContentName);
                    stanzaParameters.Add(this.CreateParameter(stanzaHymnNumberName, hymn.Number));
                    stanzaParameters.Add(this.CreateParameter(stanzaNumberName, stanza.Number));
                    stanzaParameters.Add(this.CreateParameter(stanzaContentName, stanza.Content));

                    if (stanzaParameters.Count >= 500)
                    {
                        // to avoid reaching the maximum variables count, we split the query into multiple chunks
                        this._progressLogger.Write("=>");
                        stanzaQuery = this.RemoveLastCharsIfExisting(stanzaQuery, ",");
                        this.ExecuteNonQuery(stanzaQuery, CommandType.Text, stanzaParameters.ToArray());
                        stanzaQuery = string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES "
                                                , CommonConstants.DatabaseTables.TABLE_STANZA_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_HYMN_NUMBER_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_NUMBER_NAME
                                                , CommonConstants.DatabaseTables.FIELD_STANZA_CONTENT_NAME);
                        stanzaParameters.Clear();
                    }
                }
            }
            if (hymnParameters.Count > 0)
            {
                this._progressLogger.Write("=>");
                hymnQuery = this.RemoveLastCharsIfExisting(hymnQuery, ",");
                this.ExecuteNonQuery(hymnQuery, CommandType.Text, hymnParameters.ToArray());
            }
            if (stanzaParameters.Count > 0)
            {
                this._progressLogger.Write("=>");
                stanzaQuery = this.RemoveLastCharsIfExisting(stanzaQuery, ",");
                this.ExecuteNonQuery(stanzaQuery, CommandType.Text, stanzaParameters.ToArray());
            }
            this._progressLogger.WriteLine("");
            this._progressLogger.WriteLine("New hymns inserted successfully.");
        }

        private string RemoveLastCharsIfExisting(string value, string charsToRemove)
        {
            if (value.EndsWith(charsToRemove))
                value = value.Substring(0, value.Length - charsToRemove.Length);
            return value;
        }
    }
}
