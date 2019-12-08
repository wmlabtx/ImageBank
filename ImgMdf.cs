using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private readonly object _sqlLock = new object();
        private readonly SqlConnection _sqlConnection;

        private readonly ConcurrentDictionary<string, Img> _imgList = new ConcurrentDictionary<string, Img>();       
        private readonly Queue<double> _findTimes = new Queue<double>();
        private double _avgTimes;

        public ImgMdf()
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Connection Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }
    }
}