import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';

class WelcomePage extends StatelessWidget {
  const WelcomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final width = size.width;
    final height = size.height;

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

          Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: const [
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
            alignment: const Alignment(0, 0.8),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                AppButton(
                  width: width * 0.9,
                  text: 'GET STARTED',
                  onPressed: () {
                    Navigator.pushNamed(context, '/signup');
                  },
                ),

                const SizedBox(height: 23),

                AppButton(
                  width: width * 0.9,
                  variant: ButtonVariant.outlined,
                  text: 'I ALREADY HAVE AN ACCOUNT',
                  onPressed: () {
                    Navigator.pushNamed(context, '/login');
                  },
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
