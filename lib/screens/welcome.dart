import 'package:flutter/material.dart';

class AppSize {
  static late double width;
  static late double height;

  static void init(BuildContext context) {
    final size = MediaQuery.of(context).size;
    width = size.width;
    height = size.height;
  }
}

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
                Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(40),
                    boxShadow: const [
                      BoxShadow(
                        color: Color(0xCCCC3838),
                        blurRadius: 0,
                        offset: Offset(0, 2),
                      ),
                    ],
                  ),
                  child: SizedBox(
                    width: AppSize.width * 0.9,
                    height: 60,
                    child: ElevatedButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/signup');
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFFFF4646),
                        foregroundColor: Colors.white,
                      ),
                      child: const Text(
                        'GET STARTED',
                        style: TextStyle(
                          fontWeight: FontWeight.w700,
                          fontSize: 20,
                        ),
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 23),

                SizedBox(
                  width: AppSize.width * 0.9,
                  height: 60,
                  child: ElevatedButton(
                    onPressed: () {
                      Navigator.pushNamed(context, '/login');
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.white,
                      foregroundColor: const Color(0xFFFF4646),
                      elevation: 0,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(40),
                        side: const BorderSide(
                          color: Color(0xFFFF9393),
                          width: 4,
                        ),
                      ),
                    ),
                    child: const Text(
                      'I ALREADY HAVE AN ACCOUNT',
                      style: TextStyle(
                        fontWeight: FontWeight.w700,
                        fontSize: 20,
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
