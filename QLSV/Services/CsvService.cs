using System.Globalization;
using System.Reflection;
using System.Text;

namespace StudentManagement.Services
{
    public class CsvService : ICsvService
    {
        private readonly string _dataPath;

        public CsvService(IWebHostEnvironment env)
        {
            _dataPath = Path.Combine(env.ContentRootPath, "Data");
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public async Task<List<T>> ReadAsync<T>(string fileName) where T : class, new()
        {
            var filePath = Path.Combine(_dataPath, fileName);
            var result = new List<T>();

            if (!File.Exists(filePath))
            {
                return result;
            }

            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            if (lines.Length == 0)
            {
                return result;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = SplitCsvLine(lines[i]);
                var obj = new T();

                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    var prop = properties.FirstOrDefault(p =>
                        p.Name.Equals(headers[j].Trim(), StringComparison.OrdinalIgnoreCase));

                    if (prop != null && prop.CanWrite)
                    {
                        try
                        {
                            var value = values[j].Trim().Trim('"');
                            if (!string.IsNullOrEmpty(value))
                            {
                                var convertedValue = Convert.ChangeType(value,
                                    Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType,
                                    CultureInfo.InvariantCulture);
                                prop.SetValue(obj, convertedValue);
                            }
                        }
                        catch { }
                    }
                }
                result.Add(obj);
            }

            return result;
        }

        public async Task WriteAsync<T>(string fileName, List<T> data) where T : class
        {
            var filePath = Path.Combine(_dataPath, fileName);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item)?.ToString() ?? "";
                    return value.Contains(',') || value.Contains('"')
                        ? $"\"{value.Replace("\"", "\"\"")}\""
                        : value;
                });
                csv.AppendLine(string.Join(",", values));
            }

            await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        }

        private string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(line[i]);
                }
            }
            result.Add(current.ToString());

            return result.ToArray();
        }
    }
}