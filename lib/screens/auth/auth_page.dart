import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/auth_text_field.dart';
import 'package:vitalact/services/auth_service.dart';
import 'package:vitalact/utils/auth_error_mapper.dart';
import 'package:vitalact/utils/validators.dart';

enum AuthMode { login, signup }

class AuthPage extends StatefulWidget {
  final AuthMode mode;
  final VoidCallback? onBack;

  const AuthPage({
    super.key,
    required this.mode,
    this.onBack,
  });

  @override
  State<AuthPage> createState() => _AuthPageState();
}

class _AuthPageState extends State<AuthPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  @override
  void dispose() {
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  bool get isLogin => widget.mode == AuthMode.login;

  Future<void> submit() async {
    final email = emailController.text.trim();
    final password = passwordController.text.trim();

    // Don't proceed if email or password is empty, but don't show error
    if (email.isEmpty || password.isEmpty) {
      return;
    }

    if (!_formKey.currentState!.validate()) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please fix the errors')),
      );
      return;
    }

    try {
      if (isLogin) {
        await AuthService.signIn(
          email: email,
          password: password,
        );

        TextInput.finishAutofillContext();

        if (!mounted) return;
      } else {
        await AuthService.signUp(
          email: email,
          password: password,
        );
        TextInput.finishAutofillContext();

        if (!mounted) return;
      }
    } on Exception catch (e) {
      if (!mounted) return;

      if (e is FirebaseAuthException) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(AuthErrorMapper.messageFromException(e))),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.toString())),
        );
      }
    }
  }

  Future<void> signInAnonymously() async {
    try {
      await AuthService.signInAnonymously();

      if (!mounted) return;
    } on FirebaseAuthException catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(AuthErrorMapper.messageFromException(e))),
      );
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Anonymous sign-in failed')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final title = isLogin ? 'Log In' : 'Sign up';
    final primaryText = isLogin ? 'LOG IN' : 'SIGN UP';

    return Scaffold(
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Colors.transparent,
        leading: BackButton(
            onPressed: widget.onBack != null
                ? widget.onBack!
                : () => Navigator.pop(context)),
      ),
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20),
            child: AutofillGroup(
              child: Form(
                key: _formKey,
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      title,
                      style: const TextStyle(
                        fontWeight: FontWeight.w800,
                        fontSize: 30,
                        color: Color.fromARGB(255, 52, 52, 52),
                      ),
                    ),
                    const SizedBox(height: 25),
                    SizedBox(
                      width: width * 0.9,
                      child: AuthTextField(
                        hintText: 'Email',
                        controller: emailController,
                        validator: Validators.email,
                        autofillHints: const [AutofillHints.email],
                      ),
                    ),
                    const SizedBox(height: 10),
                    SizedBox(
                      width: width * 0.9,
                      child: AuthTextField(
                        hintText: 'Password',
                        controller: passwordController,
                        validator: Validators.password,
                        isPassword: true,
                        autofillHints: [
                          isLogin
                              ? AutofillHints.password
                              : AutofillHints.newPassword,
                        ],
                      ),
                    ),
                    const SizedBox(height: 25),
                    AppButton(
                      height: 55,
                      onPressed: submit,
                      child: Text(primaryText),
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
          ),
        ),
      ),
    );
  }
}
