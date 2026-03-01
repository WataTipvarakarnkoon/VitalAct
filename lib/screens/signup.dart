import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/auth_text_field.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:vitalact/utils/validators.dart';

class SignupPage extends StatefulWidget {
  const SignupPage({super.key});

  @override
  State<SignupPage> createState() => _SignupPageState();
}

class _SignupPageState extends State<SignupPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  @override
  void dispose() {
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  Future<void> signUp() async {
    if (!_formKey.currentState!.validate()) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Please fix the errors"),
        ),
      );
      return;
    }

    try {
      await FirebaseAuth.instance.createUserWithEmailAndPassword(
        email: emailController.text.trim(),
        password: passwordController.text.trim(),
      );

      if (!mounted) return;

      Navigator.pushReplacementNamed(context, '/login');

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Signup successful!")),
      );
    } on FirebaseAuthException catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(e.message ?? "Signup failed")),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20),
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
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

                  /// Email
                  SizedBox(
                    width: width * 0.9,
                    child: AuthTextField(
                      hintText: 'Email',
                      controller: emailController,
                      validator: Validators.email,
                    ),
                  ),
                  const SizedBox(height: 10),

                  /// Password
                  SizedBox(
                    width: width * 0.9,
                    child: AuthTextField(
                      hintText: "Password",
                      controller: passwordController,
                      validator: Validators.password,
                      isPassword: true,
                    ),
                  ),
                  const SizedBox(height: 25),

                  AppButton(
                    width: width * 0.9,
                    height: 45,
                    text: 'SIGN UP',
                    onPressed: () {
                      signUp();
                    },
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
