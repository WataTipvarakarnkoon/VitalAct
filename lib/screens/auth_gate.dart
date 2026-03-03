import 'package:flutter/material.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'splash_screen.dart';
import 'index_page.dart';
import 'welcome.dart';

class AuthGate extends StatelessWidget {
  const AuthGate({super.key});

  Future<User?> _initialize() async {
    final results = await Future.wait([
      Future.delayed(const Duration(milliseconds: 3000)),
      FirebaseAuth.instance.authStateChanges().first,
    ]);

    return results[1] as User?;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<User?>(
      future: _initialize(),
      builder: (context, snapshot) {
        Widget child;

        if (snapshot.connectionState != ConnectionState.done) {
          child = const SplashScreen(key: ValueKey('splash'));
        } else if (snapshot.data != null) {
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
