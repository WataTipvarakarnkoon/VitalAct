// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Mediapipe.Tasks.Vision.HandLandmarker;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
  public class HandLandmarkDetectionConfig
  {
    public Tasks.Core.BaseOptions.Delegate Delegate { get; set; } =
      // CPU on all platforms when running as Unity library inside Flutter.
      // GPU delegate can fail in Virtual Display (TextureView) mode due to
      // EGL context sharing issues between Unity and MediaPipe's secondary context.
      Tasks.Core.BaseOptions.Delegate.CPU;

    public ImageReadMode ImageReadMode { get; set; } = ImageReadMode.CPUAsync;

    public Tasks.Vision.Core.RunningMode RunningMode { get; set; } = Tasks.Vision.Core.RunningMode.LIVE_STREAM;

    public int NumHands { get; set; } = 2;
    // Slightly stricter detection (only fires when tracking is already lost — be confident).
    public float MinHandDetectionConfidence { get; set; } = 0.55f;
    // More lenient: presence/tracking confidence dips during fast CPR motion blur.
    // 0.4 bridges the 1-3 blurry frames without causing full re-detection cycles.
    public float MinHandPresenceConfidence { get; set; } = 0.4f;
    public float MinTrackingConfidence { get; set; } = 0.4f;
    public string ModelPath => "hand_landmarker.bytes";

    public HandLandmarkerOptions GetHandLandmarkerOptions(HandLandmarkerOptions.ResultCallback resultCallback = null)
    {
      return new HandLandmarkerOptions(
        new Tasks.Core.BaseOptions(Delegate, modelAssetPath: ModelPath),
        runningMode: RunningMode,
        numHands: NumHands,
        minHandDetectionConfidence: MinHandDetectionConfidence,
        minHandPresenceConfidence: MinHandPresenceConfidence,
        minTrackingConfidence: MinTrackingConfidence,
        resultCallback: resultCallback
      );
    }
  }
}
