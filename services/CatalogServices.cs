using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace DateManager.services
{
    internal static class CatalogServices
    {
        // 카탈로그 파일을 읽어서 FrameData 객체 리스트로 반환하는 메서드
        public static List<FrameData> LoadCatalog(string catalogPath)
        {
            var frames = new List<FrameData>();

            foreach (var line in File.ReadLines(catalogPath))// 각 줄을 읽어서 JSON으로 파싱
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                using var document = JsonDocument.Parse(line);// JSON 문서를 파싱하여 루트 요소를 가져옴
                var root = document.RootElement;// 필요한 속성들을 추출하여 FrameData 객체를 생성하고 리스트에 추가

                frames.Add(new FrameData
                {
                    Index = GetInt32(root, "_index"),
                    SessionId = GetString(root, "_session_id"),
                    TimestampMs = GetInt64(root, "_timestamp_ms"),
                    ImageFile = GetString(root, "cam/image_array"),
                    Angle = GetDouble(root, "user/angle"),
                    Throttle = GetDouble(root, "user/throttle")
                });
            }

            return frames;
        }

        private static string GetString(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var value) ? value.GetString() ?? string.Empty : string.Empty;
        }

        private static int GetInt32(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var value) ? value.GetInt32() : 0;
        }

        private static long GetInt64(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var value) ? value.GetInt64() : 0L;
            // 속성이 존재하지 않는 경우 기본값으로 0을 반환
        }

        private static double GetDouble(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value))
            {
                return 0d;// 속성이 존재하지 않는 경우 기본값으로 0을 반환
            }

            return value.ValueKind == JsonValueKind.String
                ? double.Parse(value.GetString() ?? "0", CultureInfo.InvariantCulture)
                : value.GetDouble();
            // 일부 값이 문자열로 저장되어 있을 수 있으므로, 문자열인 경우에는 파싱하여 double로 변환
        }
    }
}
