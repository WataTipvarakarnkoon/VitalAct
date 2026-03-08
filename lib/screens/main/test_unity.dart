import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:flutter/material.dart';

class TestUnity extends StatefulWidget {
  const TestUnity({Key? key}) : super(key: key);

  @override
  State<TestUnity> createState() => _TestUnityState();
}

class _TestUnityState extends State<TestUnity> {
  UnityWidgetController? _unityWidgetController;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        color: Colors.yellow,
        child: UnityWidget(
          onUnityCreated: onUnityCreated,
        ),
      ),
    );
  }

  // Callback that connects the created controller to the unity controller
  void onUnityCreated(UnityWidgetController controller) {
    _unityWidgetController = controller;
  }
}
