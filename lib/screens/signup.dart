import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/auth_text_field.dart';

class SignupPage extends StatelessWidget {
  const SignupPage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: LayoutBuilder(
          builder: (context, constraints) {
            return SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: ConstrainedBox(
                constraints: BoxConstraints(minHeight: constraints.maxHeight),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Text(
                      'Sign up',
                      style: TextStyle(
                        fontWeight: FontWeight.w800,
                        fontSize: 30,
                        color: Color.fromARGB(255, 52, 52, 52),
                      ),
                    ),

                    const SizedBox(height: 25),

                    AuthTextField(hintText: 'Username'),

                    const SizedBox(height: 10),

                    AuthTextField(hintText: 'Password', obscureText: false),

                    const SizedBox(height: 25),

                    AppButton(
                      width: width * 0.9,
                      height: 45,
                      text: 'SIGN UP',
                      onPressed: () {
                        Navigator.pushNamed(context, '/index');
                      },
                    ),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}
