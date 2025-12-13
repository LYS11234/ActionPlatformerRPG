using UnityEngine;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using ExcelDataReader;

namespace Excel
{
    
    public class ExcelToScriptable : MonoBehaviour
    {
        [SerializeField]
        private string[] excelFilePaths;
#if UNITY_EDITOR
        [ContextMenu("FetchExcelToScriptableObject")]
        private async void FetchExcel()
        {
            for (int i = 0; i < excelFilePaths.Length; i++)
            {
                CheckFiles(excelFilePaths[i]);
            }
        }

        private async void CheckFiles(string _path)
        {
            using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var _reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var _result = _reader.AsDataSet();
                    GetTables( _result );
                }
            }
        }

        private void GetTables(DataSet dataSet)
        {
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                Type _soType = Type.GetType(dataSet.DataSetName + "SO");
                Type _dataType = Type.GetType(dataSet.DataSetName + "Data");
                ScriptableObject _soInstance = ScriptableObject.CreateInstance(_soType);

                CreateScriptables(dataSet.Tables[i]);
            }
        }

        private void CreateScriptables(DataTable dataTable)
        {

        }
#endif
    }
}
