using OpenCvSharp;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace ImageBank
{
    public class OrbsFile
    {
        private readonly string _filename;
        private readonly object _lock = new object();
        private readonly int _length;
        private readonly ConcurrentDictionary<int, Mat> _cache = new ConcurrentDictionary<int, Mat>();
        private readonly FileStream fs = null;

        public OrbsFile(string filename, int length)
        {
            Debug.Assert(!string.IsNullOrEmpty(filename), $"filename is null");
            Debug.Assert(length > 0, $"{length}: length <= 0");

            _filename = filename;
            _length = length;
            fs = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        ~OrbsFile()
        {
            if (fs != null)
            {
                fs.Dispose();
            }
        }

        public int Set(int slot, Mat mat)
        {
            Debug.Assert(slot >= 0 && slot <= 99999, $"{slot}: slot < 0 || slot > 99999");
            Debug.Assert(mat != null, $"mat is null");

            byte[] array;
            lock (_lock)
            {
                if (_cache.ContainsKey(slot))
                {
                    _cache[slot] = mat;
                }
                else
                {
                    _cache.TryAdd(slot, mat);
                }

                mat.GetArray<byte>(out array);
                var dstoffset = slot * _length;
                lock (_lock)
                {
                    fs.Seek(dstoffset, SeekOrigin.Begin);
                    fs.Write(array, 0, array.Length);
                    fs.Flush();
                }
            }

            return array.Length;
        }

        public Mat Get(int slot, int length)
        {
            Debug.Assert(slot >= 0 && slot <= 99999, $"{slot}: slot < 0 || slot > 99999");
            Debug.Assert(length <= _length, $"{length},{_length}: array.Length > _length");

            Mat mat;
            lock (_lock)
            {
                if (_cache.TryGetValue(slot, out mat))
                {
                    return mat;
                }

                var array = new byte[length];
                var srcoffset = slot * _length;
                lock (_lock)
                {
                    fs.Seek(srcoffset, SeekOrigin.Begin);
                    fs.Read(array, 0, array.Length);
                }

                mat = new Mat(array.Length / 32, 32, MatType.CV_8UC1);
                mat.SetArray(array);
                _cache.TryAdd(slot, mat);
            }

            return mat;
        }

        public void Delete(int slot)
        {
            lock (_lock)
            {
                if (_cache.ContainsKey(slot))
                {
                    _cache.TryRemove(slot, out var _);
                }
            }
        }
    }
}