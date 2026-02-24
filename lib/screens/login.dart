import 'package:flutter/material.dart';
import 'package:vitalact/main.dart' show AppSize;

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(children: [Text("login")]),
    );
  }
}
