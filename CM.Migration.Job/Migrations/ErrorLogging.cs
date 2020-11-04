using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marten.Util;

namespace CM.Migration.Job.Migrations
{
    public class ErrorLogging<T> where T : TEntity
    {
        public Dictionary<T, List<string>> _errorLog;
        public Dictionary<string, Guid> _idConvertion;
        
        public string ConvertionLogFilePath { get; private set; }
        public string ErrorLogFilePath { get; private set; }
        
        public ErrorLogging(string name, string text)
        {
            _errorLog = new Dictionary<T, List<string>>();
            _idConvertion = new Dictionary<string, Guid>();
            CreateMigrationLog(name, text);
            CreateMigrationErrorLog(name, text);
        }

        public void WriteToConvertionLog(string oldId, Guid newId, string text)
        {
            using (StreamWriter sw = File.AppendText(ConvertionLogFilePath)) 
            {
                _idConvertion.Add(oldId, newId);
                sw.WriteLine(text);
            }
        }
        
        public void WriteToErrorLog(T entity, string msg)
        {
            if (entity.Id == null)
            {
                return;
            }
            
            var master = _errorLog.Where(x => x.Key.Id.Equals(entity.Id));

            if (master.Any())
            {
                _errorLog[entity].Add(msg);
            }
            else
            {
                _errorLog.Add(entity, new List<string>());
                _errorLog[entity].Add(msg);
            }
        }
        
        private void CreateMigrationLog(string Name, string Text)
        {
            string txtPath = Directory.GetCurrentDirectory() + $"/MigrationLogs/{Name}Migration.txt";
            if (!File.Exists(txtPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(txtPath)) 
                {
                    sw.WriteLine($"{Text} Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                //Clear previous migration log
                File.WriteAllText(txtPath, $"{Text} - " + DateTime.Now + "\n");
            }

            ConvertionLogFilePath = txtPath;
        }
        
        private void CreateMigrationErrorLog(string name, string text)
        {
            var migrationPath = Directory.GetCurrentDirectory() + $"/MigrationLogs/{name}MigrationErrorLog.txt";
            if (!File.Exists(migrationPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(migrationPath)) 
                {
                    sw.WriteLine($"{text} Error Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                //Clear previous migration log
                File.WriteAllText(migrationPath, $"{text} Error Log - " + DateTime.Now + "\n");
            }

            ErrorLogFilePath = migrationPath;
        }
        
        public void FlushErrorLogToFile(string lineText)
        {
            using (StreamWriter sw = File.AppendText(ErrorLogFilePath)) 
            {
                foreach (var err in _errorLog)
                {
                    var entity = err.Key;
                    var newId = Guid.Empty;
                    if (_idConvertion.Any(x => x.Key.Equals(entity.Id)))
                    {
                        newId = _idConvertion[entity.Id];
                    }

                    var type = entity.GetType();
                    if (type.Equals(typeof(UserMigration.User)))
                    {
                        var user = entity as UserMigration.User;
                        
                        sw.WriteLine($"User: Email:{user.email} - oldId:{user.Id} --> New Id:{newId}");
                    }
                    else if(type.Equals(typeof(TastingMigration.Tasting)))
                    {
                        var tasting = entity as TastingMigration.Tasting;
                        sw.WriteLine($"Tasting: OldId:{tasting.Id} - NewId:{newId} - userId: {tasting.userId} - brandId:{tasting.BrandId}"); //Make this champagne info 
                    }
                    foreach (var val in err.Value)
                    {
                        sw.WriteLine("\t--> " +val ); 
                    }
                    sw.WriteLine("\n");
                }
                
                sw.WriteLine("");
            }
        }
    }
}