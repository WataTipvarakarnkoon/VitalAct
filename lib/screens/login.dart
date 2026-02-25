import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

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
                      'Log In',
                      style: TextStyle(
                        fontWeight: FontWeight.w800,
                        fontSize: 30,
                        color: Color.fromARGB(255, 52, 52, 52),
                      ),
                    ),

                    const SizedBox(height: 25),

                    SizedBox(
                      width: width * 0.9,
                      height: 45,
                      child: TextField(
                        style: const TextStyle(
                          fontWeight: FontWeight.w700,
                          fontSize: 20,
                          color: Color.fromARGB(255, 119, 119, 100),
                        ),
                        decoration: InputDecoration(
                          contentPadding: const EdgeInsets.symmetric(
                            horizontal: 20,
                          ),
                          hintText: 'Username',
                          hintStyle: const TextStyle(
                            color: Color.fromARGB(255, 119, 119, 100),
                          ),
                          enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(50),
                            borderSide: const BorderSide(
                              color: Color.fromARGB(255, 196, 196, 196),
                              width: 3,
                            ),
                          ),
                          focusedBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(50),
                            borderSide: const BorderSide(
                              color: Color.fromARGB(255, 0, 170, 255),
                              width: 3,
                            ),
                          ),
                        ),
                      ),
                    ),

                    const SizedBox(height: 10),

                    SizedBox(
                      width: width * 0.9,
                      height: 45,
                      child: TextField(
                        obscureText: true,
                        style: const TextStyle(
                          fontWeight: FontWeight.w700,
                          fontSize: 20,
                          color: Color.fromARGB(255, 119, 119, 100),
                        ),
                        decoration: InputDecoration(
                          contentPadding: const EdgeInsets.symmetric(
                            horizontal: 20,
                          ),
                          hintText: 'Password',
                          hintStyle: const TextStyle(
                            color: Color.fromARGB(255, 119, 119, 100),
                          ),
                          enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(50),
                            borderSide: const BorderSide(
                              color: Color.fromARGB(255, 196, 196, 196),
                              width: 3,
                            ),
                          ),
                          focusedBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(50),
                            borderSide: const BorderSide(
                              color: Color.fromARGB(255, 0, 170, 255),
                              width: 3,
                            ),
                          ),
                        ),
                      ),
                    ),

                    const SizedBox(height: 25),

                    AppButton(
                      width: width * 0.9,
                      height: 45,
                      text: 'LOG IN',
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
