import 'package:flutter/material.dart';
import 'welcome.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData(fontFamily: 'BalooBhai2'),
      debugShowCheckedModeBanner: false,
      initialRoute: '/welcome',
      routes: {'/welcome': (context) => const WelcomePage()},
    );
  }
}
