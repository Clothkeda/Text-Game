using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using System.Text;

public class ExcelReader
{
    public struct ExcelData
    {
        public string speakerName;
        public string speakingContent;
        public string avatarImageFileName;
        public string vocalAudioFileName;
    }

    public static List<ExcelData> ReadExcel(string filePath)
    {
        List<ExcelData> excelData = new List<ExcelData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    while (reader.Read())
                    {
                        ExcelData data = new ExcelData();
                        data.speakerName = reader.IsDBNull(0) ? string.Empty : reader.GetValue(0).ToString();
                        data.speakingContent = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1).ToString();
                        data.avatarImageFileName = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2).ToString();
                        data.vocalAudioFileName = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3).ToString();
                        excelData.Add(data);
                    }
                }while (reader.NextResult());
            }
        }
        return excelData;
    }
}
