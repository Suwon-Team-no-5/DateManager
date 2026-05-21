using System;
using System.Collections.Generic;
using System.Linq;

namespace DateManager.services
{
    internal static class FrameFilterService
    {
        //LINQ Where절을 사용하여 프레임 데이터를 필터링하는 메서드들을 정의합니다.
        public static List<FrameData> GetStoppedFrames(IEnumerable<FrameData> frames)
        {
            // Throttle이 0인 프레임을 필터링하여 반환합니다.
            return frames
                .Where(frame => frame.Throttle == 0)
                .ToList();
        }

        public static List<FrameData> GetStraightDrivingFrames(IEnumerable<FrameData> frames)
        {
            // Angle이 거의 0이고 Throttle이 양수인 프레임을 필터링하여 반환합니다.
            return frames
                .Where(frame => Math.Abs(frame.Angle) <= 0.01 && frame.Throttle > 0)
                .ToList();
        }
    }
}
