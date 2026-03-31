import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:flutter/material.dart';

class TestUnity extends StatefulWidget {
  const TestUnity({super.key});

  @override
  State<TestUnity> createState() => _TestUnityState();
}

class _TestUnityState extends State<TestUnity> {
  UnityWidgetController? _unityWidgetController;

  void onUnityMessage(message) {
    if (message == "quit") {
      Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        color: Colors.yellow,
        child: UnityWidget(
          onUnityCreated: onUnityCreated,
          onUnityMessage: onUnityMessage,
        ),
      ),
    );
  }

  void onUnityCreated(UnityWidgetController controller) {
    _unityWidgetController = controller;
  }
}
