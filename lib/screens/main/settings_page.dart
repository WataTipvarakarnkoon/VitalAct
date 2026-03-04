import 'package:flutter/material.dart';
import 'package:vitalact/services/auth_service.dart';
import 'package:vitalact/screens/auth/auth_gate.dart';

class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key});

  Future<void> _signOut(BuildContext context) async {
    await AuthService.signOut();

    if (!context.mounted) return;

    Navigator.of(context).pushAndRemoveUntil(
      MaterialPageRoute(builder: (_) => const AuthGate()),
      (route) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return Scaffold(
      appBar: AppBar(
        title: const Text("Settings"),
      ),
      body: Center(
        child: SizedBox(
          width: width * 0.9,
          child: TextButton(
            onPressed: () => _signOut(context),
            child: const Text('Sign Out'),
          ),
        ),
      ),
    );
  }
}
