using HotelFood.Data;
using HotelFood.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public class SQLScriptExecutor
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelUser> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SQLScriptExecutor(AppDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                _logger= serviceScope.ServiceProvider.GetRequiredService<ILogger<HotelUser>>();
            }
        }
        public void ExecuteStartScripts()
        {
            try
            {
                _logger.LogWarning("Executing startup scripts");
                string path = Path.Combine(AppContext.BaseDirectory, "SQL");
                var files = new DirectoryInfo(path)
                       .GetFiles("*.sql");
                files.ToList().ForEach(f => ExecuteScriptfromFile(f.FullName));
            }
            catch(Exception ex)
            {
                _logger.LogError("Unhandled exception", ex);
            }
        }
        private bool ExecuteScriptfromFile(string filename)
        {
            try
            {
                _logger.LogWarning("Executing startup scripts from file {0}",filename);
                string sql = File.ReadAllText(filename);
                Queue<string> batches = new Queue<string>();
                GetBatches(sql, batches);
                bool res = true;
                while (batches.Count > 0)
                {
                    res &= ExecuteSQL(batches.Dequeue());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Unhandled exception", ex);
                return false;
            }
            return true;
        } 
        private bool ExecuteSQL(string sql)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                using (var command = connection.CreateCommand())
                {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                    command.CommandText = sql;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "script {0}", sql);
                return false;
            }
            return true;
        }
        static internal Queue<string> GetBatches(string sql, Queue<string> batches)
        {
            Queue<string> _res = batches ?? new Queue<string>();
            Regex regexGO = new Regex(@"^[\s\t]*[gG][oO][\s\t]*(\-\-.*?)*$",
                            RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matches_go = regexGO.Matches(sql);
            int position = 0;
            for (int i = 0; i < matches_go.Count; i++)
            {
                int len = matches_go[i].Index - position;
                _res.Enqueue(sql.Substring(position, len));
                position = matches_go[i].Index + matches_go[i].Value.Length;
            }
            if (position < sql.Length)
                _res.Enqueue(sql.Substring(position));
            return _res;
        }
    }
}
