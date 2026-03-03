import 'package:flutter/material.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/auth_text_field.dart';
import 'package:vitalact/services/auth_service.dart';
import 'package:vitalact/utils/auth_error_mapper.dart';
import 'package:vitalact/utils/validators.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  @override
  void dispose() {
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  Future<void> signIn() async {
    if (!_formKey.currentState!.validate()) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Please fix the errors"),
        ),
      );
      return;
    }

    try {
      await AuthService.signIn(
        email: emailController.text.trim(),
        password: passwordController.text.trim(),
      );

      if (!mounted) return;

      Navigator.pushReplacementNamed(context, '/index');

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Signin successful!")),
      );
    } on FirebaseAuthException catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(AuthErrorMapper.messageFromException(e))),
      );
    }
  }

  Future<void> signInAnonymously() async {
    try {
      await AuthService.signInAnonymously();

      if (!mounted) return;

      Navigator.pushReplacementNamed(context, '/index');

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Signed in anonymously")),
      );
    } on FirebaseAuthException catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(AuthErrorMapper.messageFromException(e))),
      );
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Anonymous sign-in failed")),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const Text(
                'Log In',
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
                text: 'LOG IN',
                onPressed: signIn,
              ),
              const SizedBox(height: 10),
              SizedBox(
                width: width * 0.9,
                child: TextButton(
                  onPressed: signInAnonymously,
                  child: const Text('Continue as Guest'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
