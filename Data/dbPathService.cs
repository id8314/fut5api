using System.IO;
using Microsoft.Data.Sqlite;

namespace fut5.Data
{
    public class dbPathService
    {
        public string databasePath = "";
        public dbPathService()
        {
            var db_file = Directory.GetFiles("/data", "fut5.db", SearchOption.AllDirectories);
            if (db_file.GetLength(0) > 0)  {
                this.databasePath = db_file[0];
            }
            else {
                var datadir = Path.GetFullPath("/data");
                var ff = Directory.GetFiles("/data", "*", SearchOption.TopDirectoryOnly);
                var files = "";
                foreach(var f in ff)
                    files = files + f + "\n";
                throw new System.Exception($"\nDatabase not found in {datadir}\n{files}");
            }
        }
    }
}