/// <summary>ข้อมูลสรุปของ session CPR หนึ่งครั้ง</summary>
public class CprSessionData
{
    public int   totalCompressions;
    public int   goodRateCompressions;   // rate อยู่ใน 100-120 /min
    public int   goodDepthCompressions;  // depth > 50%
    public int   completedCycles;        // รอบ 30:2 ที่เสร็จ
    public float sessionDuration;        // วินาที

    public float RateAccuracy  => totalCompressions > 0
        ? (float)goodRateCompressions  / totalCompressions * 100f : 0f;
    public float DepthAccuracy => totalCompressions > 0
        ? (float)goodDepthCompressions / totalCompressions * 100f : 0f;
    public float OverallScore  => (RateAccuracy + DepthAccuracy) / 2f;

    public string Grade
    {
        get
        {
            float s = OverallScore;
            if (s >= 90f) return "A";
            if (s >= 80f) return "B";
            if (s >= 70f) return "C";
            if (s >= 60f) return "D";
            return "F";
        }
    }
}
