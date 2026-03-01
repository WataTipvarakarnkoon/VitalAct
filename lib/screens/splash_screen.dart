import 'package:flutter/material.dart';
import 'package:vitalact/screens/welcome.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _opacity;
  late Animation<double> _scaleUp;
  late Animation<double> _scaleDown;
  late Animation<double> _scaleFull;
  late Animation<double> _rotation;
  late Animation<Offset> _position;
  late Animation<Offset> _positionLeft;
  late Animation<double> _textOpacity;

  @override
  void initState() {
    super.initState();

    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 2700),
    );

    _position = Tween<Offset>(
      begin: const Offset(0, .7),
      end: const Offset(0, 0),
    ).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(
          0.10,
          0.17,
          curve: Cubic(0.34, 1.56, 0.64, 1.0),
        ),
      ),
    );

    _opacity = Tween<double>(begin: 0, end: 1).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.10, 0.17, curve: Curves.easeIn),
      ),
    );

    _scaleUp = Tween<double>(begin: 1.0, end: 1.5).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.25, 0.29, curve: Curves.easeOut),
      ),
    );

    _rotation = Tween<double>(begin: 0, end: 0.5).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.25, 0.29, curve: Curves.easeIn),
      ),
    );

    _scaleDown = Tween<double>(begin: 1.5, end: 0.2).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.4, 0.45, curve: Cubic(0.34, 1.56, 0.64, 1.0)),
      ),
    );

    _positionLeft = Tween<Offset>(
      begin: const Offset(0, 0),
      end: const Offset(4.5, -2.7),
    ).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(
          0.5,
          0.65,
          curve: Cubic(0.34, 1.56, 0.64, 1.0),
        ),
      ),
    );

    _scaleFull = Tween<double>(begin: 1.0, end: 50).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.9, 1.0, curve: Curves.easeOut),
      ),
    );

    _textOpacity = Tween<double>(begin: 0, end: 1).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.52, 0.7, curve: Cubic(0.34, 1.56, 0.64, 1.0)),
      ),
    );

    _controller.forward();

    Future.delayed(const Duration(milliseconds: 3000), () {
      if (!mounted) return;

      Navigator.of(context).pushReplacement(
        PageRouteBuilder(
          transitionDuration: const Duration(milliseconds: 600),
          pageBuilder: (context, animation, secondaryAnimation) =>
              const WelcomePage(),
          transitionsBuilder: (context, animation, secondaryAnimation, child) {
            return FadeTransition(opacity: animation, child: child);
          },
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Stack(
          alignment: Alignment.center,
          children: [
            AnimatedBuilder(
              animation: _controller,
              builder: (context, child) {
                return Opacity(
                  opacity: _opacity.value,
                  child: Transform.scale(
                    scale: _scaleUp.value,
                    child: SlideTransition(
                      position: _position,
                      child: Transform.rotate(
                        angle: _rotation.value,
                        child: Transform.scale(
                          scale: _scaleDown.value,
                          child: SlideTransition(
                            position: _positionLeft,
                            child: Transform.scale(
                              scale: _scaleFull.value,
                              child: child,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                );
              },
              child: Container(
                height: 80,
                width: 80,
                decoration: BoxDecoration(
                  color: const Color(0xFFFF4646),
                  borderRadius: BorderRadius.circular(5),
                ),
              ),
            ),
            const SizedBox(height: 20),
            Transform.translate(
              offset: const Offset(-30, 0),
              child: FadeTransition(
                opacity: _textOpacity,
                child: const Text(
                  'VitalAct',
                  style: TextStyle(
                    color: Color(0xFFFF4646),
                    fontFamily: 'BalooBhai2',
                    fontSize: 70,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
