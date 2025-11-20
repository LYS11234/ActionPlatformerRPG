using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GoogleSpreadSheetManager : MonoBehaviour
{
    // GameManager에 파싱해두기에 굳이 싱글톤 사용할 필요 없음
    [Tooltip("true면 구글 스프레드 시트를, false면 로컬 json 파일을 사용합니다.")]
    [SerializeField]
    private bool isAccessGoogleSheet;
    [Tooltip("구글 스프레드 시트 URL")]
    [SerializeField]
    private string googleSheetURL;
    [Tooltip("사용할 시트를 지정합니다. 공백없이, /로 구분합니다. 예: Sheet1/Sheet2")]
    [SerializeField]
    private string availableSheets;
    [Tooltip("파일 생성 경로를 지정합니다.")]
    [SerializeField]
    private string generateFolderPath;
    [Tooltip("외부에서 접근할 수 있는 Scriptable Object입니다.")]
    public ScriptableObject Scriptable { get; private set; }
    public T SO<T>() where T : ScriptableObject
    {
        if(Scriptable == null)
        {
            return null;
        }
        return Scriptable as T;
    }
    private string jsonPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetJson.json";
    private string soPath => $"Assets{generateFolderPath}/GoogleSheetSO.asset";
    private string[] availableSheetArray;
    private string json;
    private bool refreshTrigger;
    public async void FetchGoogleSheet()
    {
        if (isAccessGoogleSheet)
        {
            json = await LoadDataGoogleSheet(googleSheetURL);
        }
        else
        {

        }
        if (json != null)
        {
            return;
        }
        bool isJsonSaved = SaveFileOrSkip(jsonPath, json);
    }

    async Task<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] byteDatas = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(byteDatas);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Error: {e.HelpLink}\n{e.Message}\n{e.Source}");
                return null;
            }
        }
    }

    private bool SaveFileOrSkip(string path, string content)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (File.Exists(path) && File.ReadAllText(path).Equals(content))
        {
            return false;
        }
        File.WriteAllText(path, content);
        return true;

    }

    private bool IsExistAvailableSheets(string sheetName)
    {
        return Array.Exists(availableSheetArray, x => x == sheetName);
    }


    public void AddListOnList(string type)
    {
        if(Type.GetType(type) == null)
        {
            return;
        }
        if (Scriptable == null)
        {
            Scriptable = ScriptableObject.CreateInstance("GoogleSheetSO");
        }
        JObject jsonObject = JObject.Parse(json);
        
        try
        {
            foreach (var jObject in jsonObject)
            {


                switch (type)
                {
                    case "DialogueSO":
                        {

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        catch (Exception e)
        {

        }
    }
}
