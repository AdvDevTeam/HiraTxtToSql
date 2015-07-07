using AdvDevTeam.HiraTxtToSql.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvDevTeam.HiraTxtToSql.Core.Business
{
    /// <summary>
    /// Provides methods and properties to query data
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// Connection string to accede the database
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// Creates database schema if it doesn't exist yet
        /// </summary>
        void CreateSchemaIfNotExists();
        /// <summary>
        /// Inserts hymns into database
        /// </summary>
        /// <param name="hymns">list of hymn to insert</param>
        /// <param name="clearExisting">flag if clearing all existing data is required</param>
        void InsertHymns(List<Hymn> hymns, bool clearExisting = true);
    }
}
