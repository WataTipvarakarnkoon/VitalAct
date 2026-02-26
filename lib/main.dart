import 'package:flutter/material.dart';
import 'screens/welcome.dart';
import 'screens/login.dart';
import 'screens/signup.dart';
import 'screens/splash_screen.dart';
import 'screens/index_page.dart';

void main() => runApp(const MainApp());

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData(fontFamily: 'BalooBhai2'),
      debugShowCheckedModeBanner: false,
      initialRoute: '/splash',
      routes: {
        '/splash': (context) => const SplashScreen(),
        '/welcome': (context) => const WelcomePage(),
        '/login': (context) => const LoginPage(),
        '/signup': (context) => const SignupPage(),
        '/index': (context) => const IndexPage(),
      },
    );
  }
}

class IconItems extends StatelessWidget {
  final String path;
  final bool isSelected;
  final VoidCallback onTap;

  const IconItems({
    super.key,
    required this.path,
    required this.onTap,
    required this.isSelected,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: EdgeInsets.all(7),
        decoration: isSelected
            ? BoxDecoration(
                border: Border.all(color: Color(0xFFFF4646), width: 2.5),
                color: Color(0xFFFFEBEB),
                borderRadius: BorderRadius.circular(7),
              )
            : null,
        child: SizedBox(height: 30, child: Image.asset(path)),
      ),
    );
  }
}
