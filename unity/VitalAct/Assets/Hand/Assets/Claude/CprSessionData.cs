using System.Collections.Generic;

/// <summary>ข้อมูลสรุปและ analytics ของ session CPR หนึ่งครั้ง</summary>
public class CprSessionData
{
    // ---- basic counts ----
    public int   totalCompressions;
    public int   goodRateCompressions;
    public int   goodDepthCompressions;
    public int   completedCycles;
    public float sessionDuration;

    // ---- analytics (คำนวณโดย CprSessionAnalytics) ----
    public float avgRate;               // BPM เฉลี่ย
    public float rateConsistency;       // 0-100 ยิ่งสูงยิ่งสม่ำเสมอ
    public float avgDepth01;            // depth เฉลี่ย (0-1)
    public float peakRate;              // BPM สูงสุด
    public float handOnTimePercent;     // % เวลาที่มืออยู่บนหุ่น

    // per-compression records สำหรับ graph/detail
    public List<CompressionRecord> compressions = new List<CompressionRecord>();

    // ---- computed properties ----
    public float RateAccuracy    => totalCompressions > 0
        ? (float)goodRateCompressions  / totalCompressions * 100f : 0f;
    public float DepthAccuracy   => totalCompressions > 0
        ? (float)goodDepthCompressions / totalCompressions * 100f : 0f;
    public float OverallScore    => RateAccuracy * 0.4f + DepthAccuracy * 0.4f + rateConsistency * 0.2f;

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

public struct CompressionRecord
{
    public float time;    // วินาทีนับจากเริ่ม session
    public float rate;    // BPM ณ ตอนนั้น
    public float depth;   // 0-1
}
