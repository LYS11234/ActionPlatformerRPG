using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Device;

using Application = UnityEngine.Device.Application;
public class DataManager : MonoBehaviour
{
    private string path;
    private JsonSerializerOptions options = new JsonSerializerOptions();
    private const int chunkSize = 1024;

    private void Awake()
    {
        path = Application.persistentDataPath + "/";
    }

    public async Task SaveData<PlayerInfo>(PlayerInfo _player, string _fileName, IProgress<float> _progress)
    {
        path = Path.Combine(path, _fileName);
#if UNITY_EDITOR
        path = Application.dataPath + $"/{_fileName}";
#endif
        
        string _json = JsonUtility.ToJson(_player, true);
        options.WriteIndented = true;
        byte[] _bytes = Encoding.UTF8.GetBytes(_json);
        int _totalLength = _bytes.Length;
        int _currentWritten = 0;

        using (FileStream _stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, chunkSize, true))
        {
            while(_currentWritten < _totalLength)
            {
                int _bytesToWrite = Math.Min(chunkSize, _totalLength - _currentWritten);
                await _stream.WriteAsync(_bytes,_currentWritten, _bytesToWrite);

                _currentWritten += _bytesToWrite;

                if (_progress == null)
                {
                    continue;
                }

                float _percent = (float)_currentWritten / _totalLength;
                _progress.Report(_percent);

                await Task.Delay(1);
            }
            _progress?.Report(1.0f);
        }
    }

    public async Task<PlayerInfo> LoadData()
    {
        PlayerInfo _loadData = new PlayerInfo();
        return _loadData;
    }
}
