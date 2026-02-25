import 'package:flutter/material.dart';
import 'package:vitalact/main.dart' show AppSize;
import 'package:vitalact/widgets/app_button.dart';

class WelcomePage extends StatelessWidget {
  const WelcomePage({super.key});

  @override
  Widget build(BuildContext context) {
    AppSize.init(context);
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Positioned(
            child: Image.asset(
              'assets/images/Background.png',
              fit: BoxFit.cover,
              width: AppSize.width * 1,
              height: AppSize.height * .7,
            ),
          ),

          Positioned(
            top: AppSize.height * -0.15,
            left: AppSize.width * -.6,
            child: Image.asset(
              'assets/images/Ellipse.png',
              height: AppSize.height * 0.65,
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
                  width: AppSize.width * 0.9,
                  text: 'GET STARTED',
                  onPressed: () {
                    Navigator.pushNamed(context, '/signup');
                  },
                ),

                const SizedBox(height: 23),

                AppButton(
                  width: AppSize.width * 0.9,
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
