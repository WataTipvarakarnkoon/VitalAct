import 'package:flutter/material.dart';
import 'package:firebase_auth/firebase_auth.dart';
import '../splash_screen.dart';
import '../main/index_page.dart';
import 'welcome.dart';
import 'auth_page.dart';

class AuthGate extends StatefulWidget {
  const AuthGate({super.key});

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  bool _showSplash = true;
  bool _showLogin = false;
  bool _showSignup = false;

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
          if (_showLogin) {
            child = AuthPage(
              key: const ValueKey('auth-login'),
              mode: AuthMode.login,
              onBack: () {
                setState(() {
                  _showLogin = false;
                });
              },
            );
          } else if (_showSignup) {
            child = AuthPage(
              key: const ValueKey('auth-signup'),
              mode: AuthMode.signup,
              onBack: () {
                setState(() {
                  _showSignup = false;
                });
              },
            );
          } else {
            child = WelcomePage(
              key: const ValueKey('welcome'),
              onLogin: () {
                setState(() {
                  _showLogin = true;
                });
              },
              onSignup: () {
                setState(() {
                  _showSignup = true;
                });
              },
            );
          }
        }

        return AnimatedSwitcher(
          duration: const Duration(milliseconds: 600),
          child: child,
        );
      },
    );
  }
}
