import 'package:flutter/material.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData(fontFamily: 'BalooBhai2'),
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        backgroundColor: Colors.white,
        body: Stack(
          children: [
            Image.asset('assets/images/Background.png', fit: BoxFit.cover),

            Positioned(
              top: -150,
              left: -160,
              child: Image.asset('assets/images/Ellipse.png', height: 500),
            ),

            Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    'VitalAct',
                    style: TextStyle(
                      fontSize: 70,
                      fontFamily: 'BalooBhai2',
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
                      fontFamily: 'BalooBhai2',
                      fontWeight: FontWeight.w700,
                      color: Colors.white,
                      height: 0.65,
                    ),
                  ),
                ],
              ),
            ),

            Align(
              alignment: Alignment(0, 0.8),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Container(
                    margin: EdgeInsets.only(),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(40),
                      boxShadow: [
                        BoxShadow(
                          color: Color(0xCCCC3838),
                          blurRadius: 0,
                          offset: Offset(0, 2),
                        ),
                      ],
                    ),

                    child: SizedBox(
                      width: 350,
                      height: 60,
                      child: ElevatedButton(
                        onPressed: () {},

                        style: ElevatedButton.styleFrom(
                          backgroundColor: Color(0xFFFF4646),
                          foregroundColor: Colors.white,
                        ),

                        child: Text(
                          'GET STARTED',
                          style: TextStyle(
                            fontWeight: FontWeight.w700,
                            fontSize: 20,
                          ),
                        ),
                      ),
                    ),
                  ),

                  SizedBox(height: 23),

                  SizedBox(
                    width: 350,
                    height: 60,
                    child: ElevatedButton(
                      onPressed: () {},

                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.white,
                        foregroundColor: Color(0xFFFF4646),
                        elevation: 0,

                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(40),
                          side: BorderSide(color: Color(0xFFFF9393), width: 4),
                        ),
                      ),

                      child: Text(
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
      ),
    );
  }
}
