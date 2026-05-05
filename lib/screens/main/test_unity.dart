import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:flutter/material.dart';
import 'package:flutter/gestures.dart';
import 'package:flutter/foundation.dart';
import 'package:video_player/video_player.dart';

class TestUnity extends StatefulWidget {
  const TestUnity({super.key});

  @override
  State<TestUnity> createState() => _TestUnityState();
}

class _TestUnityState extends State<TestUnity> {
  UnityWidgetController? _unityWidgetController;
  VideoPlayerController? _videoController;
  String? _currentVideoFile;
  bool _showVideo = false;

  @override
  void dispose() {
    _videoController?.dispose();
    super.dispose();
  }

  void onUnityMessage(message) {
    final msg = message.toString();

    if (msg == 'quit') {
      Navigator.pop(context);
    } else if (msg.startsWith('video:play:')) {
      final fileName = msg.substring('video:play:'.length);
      _loadAndPlay(fileName);
    } else if (msg == 'video:restart') {
      if (_currentVideoFile != null) _loadAndPlay(_currentVideoFile!);
    } else if (msg == 'video:pause') {
      _videoController?.pause();
    }
  }

  Future<void> _loadAndPlay(String fileName) async {
    _currentVideoFile = fileName;

    final previous = _videoController;
    final controller = VideoPlayerController.asset('assets/videos/$fileName');
    _videoController = controller;

    await previous?.dispose();

    try {
      await controller.initialize();
    } catch (e) {
      debugPrint('[VideoPlayer] ERROR: $e');
      return;
    }

    controller.setLooping(true);
    if (!mounted) return;
    setState(() => _showVideo = true);
    controller.play();
  }

  void _onContinue() {
    _videoController?.pause();
    setState(() => _showVideo = false);
    _unityWidgetController?.postMessage('GameManager', 'Choosed', '');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          UnityWidget(
            onUnityCreated: onUnityCreated,
            onUnityMessage: onUnityMessage,
            useAndroidViewSurface: true,
            fullscreen: false,
            gestureRecognizers: <Factory<OneSequenceGestureRecognizer>>{
              Factory<OneSequenceGestureRecognizer>(
                () => EagerGestureRecognizer(),
              ),
            },
          ),
          if (_showVideo &&
              _videoController != null &&
              _videoController!.value.isInitialized)
            Positioned.fill(
              child: Stack(
                children: [
                  Positioned.fill(
                    child: FittedBox(
                      fit: BoxFit.cover,
                      child: SizedBox(
                        width: _videoController!.value.size.width,
                        height: _videoController!.value.size.height,
                        child: VideoPlayer(_videoController!),
                      ),
                    ),
                  ),
                  Positioned(
                    bottom: 16,
                    right: 16,
                    child: ElevatedButton(
                      onPressed: _onContinue,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.white,
                        foregroundColor: Colors.black,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 24, vertical: 10),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(20),
                        ),
                      ),
                      child: const Text('Continue',
                          style: TextStyle(fontWeight: FontWeight.bold)),
                    ),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }

  void onUnityCreated(UnityWidgetController controller) {
    _unityWidgetController = controller;
  }
}
