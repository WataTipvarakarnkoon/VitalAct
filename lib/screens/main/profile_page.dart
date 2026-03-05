import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/services/auth_service.dart';
import 'package:vitalact/screens/auth/auth_gate.dart';

class ProfileRow extends StatelessWidget {
  final String label;
  final String value;

  const ProfileRow({super.key, required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return SizedBox(
      width: width * .8,
      child: Row(
        children: [
          Text(
            label,
            style: const TextStyle(
              fontSize: 17,
              fontWeight: FontWeight.w500,
              color: Color.fromARGB(255, 132, 132, 132),
            ),
          ),
          Expanded(
            child: Text(
              value,
              textAlign: TextAlign.end,
              style: const TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.w500,
                color: Color.fromARGB(255, 132, 132, 132),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});
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
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Container(
            width: double.infinity,
            height: 150,
            color: const Color(0xFFFF4646),
          ),
          Container(
            width: double.infinity,
            margin: const EdgeInsets.only(top: 80),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Container(
                  width: 125,
                  height: 125,
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.white, width: 4),
                    color: const Color.fromARGB(255, 155, 38, 38),
                    shape: BoxShape.circle,
                  ),
                ),
                const SizedBox(height: 30),
                const Text(
                  "Name Surname",
                  style: TextStyle(
                      fontSize: 30,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFFFF4646)),
                ),
                const SizedBox(height: 50),
                const ProfileRow(label: 'Email:', value: 'X'),
                const SizedBox(
                  height: 10,
                ),
                const ProfileRow(label: 'Password:', value: 'X'),
                const SizedBox(
                  height: 10,
                ),
                Container(
                  width: width * .8,
                  height: 1.5,
                  color: const Color.fromARGB(255, 132, 132, 132),
                ),
                const SizedBox(
                  height: 10,
                ),
                const ProfileRow(label: 'Dark Mode:', value: 'Switch'),
                const SizedBox(height: 30),
                const Spacer(),
                AppButton(
                  onPressed: () => _signOut(context),
                  width: 140,
                  height: 35,
                  backgroundColor: const Color.fromARGB(255, 132, 132, 132),
                  foregroundColor: Colors.white,
                  borderColor: const Color.fromARGB(255, 132, 132, 132),
                  shadowColor: const Color.fromARGB(255, 92, 88, 88),
                  child: const Text(
                    "LOG OUT",
                    style: TextStyle(
                      fontWeight: FontWeight.w500,
                      fontSize: 18,
                    ),
                  ),
                ),
                const SizedBox(height: 30),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
