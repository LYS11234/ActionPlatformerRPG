using System.IO;
using System.Text.Json;
public class DataManager
{
    private string path;
    private JsonSerializerOptions options = new JsonSerializerOptions();

    public async void SaveData(PlayerInfo _player)
    {
        options.WriteIndented = true;

        using (FileStream _stream = File.Create(path))
        {
            await System.Text.Json.JsonSerializer.SerializeAsync(_stream, _player, options);
        }
    }

    public async void LoadData()
    {

    }
}
