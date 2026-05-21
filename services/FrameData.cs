using System;

namespace DateManager.services
{
    internal class FrameData
    {
        // CatalogServices에서 JSON 데이터를 파싱하여 FrameData 객체를 생성할 때 필요한 속성들을 정의
        public int Index { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public long TimestampMs { get; set; }
        public string ImageFile { get; set; } = string.Empty;
        public double Angle { get; set; }
        public double Throttle { get; set; }

        public override string ToString()
        {
            // Index, Angle, Throttle 속성을 포함하는 문자열을 반환하여 ListBox에 표시할 때 사용
            return $"Frame {Index:D4} | angle {Angle:0.000} | throttle {Throttle:0.000}";
        }
    }
}
