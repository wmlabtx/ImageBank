using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using OpenCvSharp;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private readonly ConcurrentDictionary<string, Img> _imgList = new ConcurrentDictionary<string, Img>();

        private readonly BFMatcher _bfMatcher;

        private readonly object _sqlLock = new object();
        private readonly SqlConnection _sqlConnection;

        private readonly Queue<double> _findTimes = new Queue<double>();
        private double _avgTimes;

        public ImgMdf()
        {
            _bfMatcher = new BFMatcher(NormTypes.Hamming, true);

            var connectionString =
                $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppConsts.FileDatabase};Integrated Security=True;Connect Timeout=30";
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }
    }
}