import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class WelcomePage extends StatelessWidget {
  final VoidCallback onLogin;
  final VoidCallback onSignup;

  const WelcomePage({
    super.key,
    required this.onLogin,
    required this.onSignup,
  });

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Positioned(
            child: Image.asset(
              'assets/images/Background.png',
              fit: BoxFit.cover,
              width: width,
              height: height * 0.7,
            ),
          ),
          Positioned(
            top: height * -0.15,
            left: width * -0.6,
            child: Image.asset(
              'assets/images/Ellipse.png',
              height: height * 0.65,
            ),
          ),
          Positioned(
              top: height * 0.21,
              left: 0,
              right: 0,
              child: const SpriteAnimation()),
          Positioned(
            left: 0,
            right: 0,
            top: height * 0.1,
            child: const Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  'VitalAct',
                  style: TextStyle(
                    fontSize: 70,
                    fontWeight: FontWeight.w800,
                    color: Colors.white,
                    height: 0.65,
                  ),
                ),
                SizedBox(height: 17),
                Text(
                  'A fun way to learn First Aid!',
                  style: TextStyle(
                    fontSize: 25,
                    fontWeight: FontWeight.w700,
                    color: Colors.white,
                    height: 0.65,
                  ),
                ),
              ],
            ),
          ),
          Align(
            alignment: const Alignment(0, .75),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                AppButton(
                  width: width * 0.9,
                  onPressed: onSignup,
                  height: 60,
                  child: const Text("GET STARTED"),
                ),
                const SizedBox(height: 23),
                AppButton(
                  width: width * 0.9,
                  variant: ButtonVariant.outlined,
                  onPressed: onLogin,
                  height: 60,
                  child: const Text("I ALREADY HAVE AN ACCOUNT"),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
