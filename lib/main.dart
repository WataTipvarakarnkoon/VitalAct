import 'package:flutter/material.dart';
import 'screens/welcome.dart';
import 'screens/auth_page.dart';
import 'screens/splash_screen.dart';
import 'screens/index_page.dart';
import 'firebase_options.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'screens/auth_gate.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData(fontFamily: 'BalooBhai2'),
      debugShowCheckedModeBanner: false,
      home: const AuthGate(),
      routes: {
        '/splash': (context) => const SplashScreen(),
        '/welcome': (context) => const WelcomePage(),
        '/login': (context) => const AuthPage(mode: AuthMode.login),
        '/signup': (context) => const AuthPage(mode: AuthMode.signup),
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
        padding: const EdgeInsets.all(5),
        decoration: isSelected
            ? BoxDecoration(
                border: Border.all(color: const Color(0xFFFF4646), width: 2.5),
                color: const Color(0xFFFFEBEB),
                borderRadius: BorderRadius.circular(7),
              )
            : null,
        child: SizedBox(height: 30, child: Image.asset(path)),
      ),
    );
  }
}
