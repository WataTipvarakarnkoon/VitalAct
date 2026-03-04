import 'package:flutter/material.dart';
import 'package:firebase_auth/firebase_auth.dart';
import '../splash_screen.dart';
import '../main/index_page.dart';
import 'welcome.dart';

class AuthGate extends StatefulWidget {
  const AuthGate({super.key});

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  bool _showSplash = true;

  @override
  void initState() {
    super.initState();
    Future.delayed(const Duration(milliseconds: 3000), () {
      if (mounted) {
        setState(() {
          _showSplash = false;
        });
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return StreamBuilder<User?>(
      stream: FirebaseAuth.instance.authStateChanges(),
      builder: (context, snapshot) {
        Widget child;

        if (_showSplash) {
          child = const SplashScreen(key: ValueKey('splash'));
        } else if (snapshot.hasData) {
          child = const IndexPage(key: ValueKey('index'));
        } else {
          child = const WelcomePage(key: ValueKey('welcome'));
        }

        return AnimatedSwitcher(
          duration: const Duration(milliseconds: 600),
          transitionBuilder: (child, animation) {
            return FadeTransition(
              opacity: animation,
              child: child,
            );
          },
          child: child,
        );
      },
    );
  }
}
