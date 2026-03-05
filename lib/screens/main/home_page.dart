import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Stack(
      children: [
        Positioned(
          top: height * -0.4,
          child: Image.asset(
            'assets/images/Background.png',
            width: width,
          ),
        ),
        Positioned(
          top: height * -0.39,
          left: width * -0.3,
          height: height * 0.5,
          child: Image.asset('assets/images/Ellipse.png'),
        ),
        Positioned(
          top: height * 0.08,
          left: 0,
          right: 0,
          child: Center(
              child: AppButton(
            onPressed: () {},
            width: width * 0.9,
            height: 70,
            borderRadius: 15,
            backgroundColor: const Color(0xFFFF4646),
            borderColor: const Color(0xFFFF4646),
            shadowColor: const Color(0xFFCC3838),
            child: const Text("PLACEHOLDER FOR THE NEXT LESSON"),
          )),
        ),
        Positioned.fill(
          top: height * 0.162,
          child: const Lesson(),
        ),
      ],
    );
  }
}

class Lesson extends StatefulWidget {
  const Lesson({super.key});

  @override
  State<Lesson> createState() => _LessonState();
}

class _LessonState extends State<Lesson> {
  int currentstep = 0;
  bool isPressed = false;
  int? pressedStep;

  void press(int step) {
    if (step == currentstep) {
      setState(() {
        currentstep++;
      });
    }
  }

  Widget button(int step) {
    bool active = step <= currentstep;
    bool isPressed = pressedStep == step;
    final width = MediaQuery.of(context).size.width;
    final stepHeading = [
      'Normal vs abnormal signs',
      'Life-threatening red flags',
      'Breathing Distress Detection',
      'Chest Pain & Stroke Clues',
      'Mixed Symptom Recognition Challenge',
    ];
    final stepSub = [
      'Learn to quickly recognize abnormal breathing and emergency warning signs.',
      'Identify critical signs that require immediate emergency action.',
      'Recognize early and severe signs of breathing difficulty.',
      'Spot key symptoms of heart attack and stroke quickly.',
      'Practice identifying emergencies when multiple symptoms appear.',
    ];

    return Material(
      color: Colors.transparent,
      child: InkWell(
        splashColor: Colors.transparent,
        highlightColor: Colors.transparent,
        hoverColor: Colors.transparent,
        focusColor: Colors.transparent,
        onTap: active ? () => press(step) : null,
        onHighlightChanged: (pressed) {
          setState(() {
            pressedStep = pressed ? step : null;
          });
        },
        child: AnimatedScale(
          scale: isPressed ? 0.9 : 1.0,
          duration: const Duration(milliseconds: 100),
          curve: Curves.easeOut,
          child: SizedBox(
            width: width * .8,
            child: Stack(children: [
              Positioned(
                child: Image.asset(
                  active
                      ? 'assets/images/press.png'
                      : 'assets/images/Notpress.png',
                ),
              ),
              Column(
                children: [
                  Container(
                    width: 260,
                    padding: const EdgeInsets.only(top: 17, left: 15),
                    child: Text(
                      active ? stepHeading[step] : '',
                      style: const TextStyle(
                          fontWeight: FontWeight.w700,
                          fontSize: 18,
                          color: Colors.white,
                          height: 0.9),
                    ),
                  ),
                  Container(
                    width: 250,
                    padding: const EdgeInsets.only(top: 7, left: 11),
                    child: Text(
                      active ? stepSub[step] : '',
                      style: const TextStyle(
                          fontWeight: FontWeight.w500,
                          fontSize: 13,
                          color: Colors.white,
                          height: 0.9),
                    ),
                  )
                ],
              ),
            ]),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    return SingleChildScrollView(
      physics: const SlowScrollPhysics(),
      child: Column(
        children: [
          const Padding(padding: EdgeInsets.only(top: 50)),
          Container(
            padding: EdgeInsets.only(right: width * .25),
            child: const Text(
              'RECOGNITION',
              style: TextStyle(
                fontSize: 35,
                fontWeight: FontWeight.w800,
                color: Color(0xFFFF4646),
                height: 0.65,
              ),
            ),
          ),
          const SizedBox(height: 10),
          button(0),
          const SizedBox(height: 60),
          button(1),
          const SizedBox(height: 60),
          button(2),
          const SizedBox(height: 60),
          button(3),
          const SizedBox(height: 60),
          button(4),
          const SizedBox(height: 60),
        ],
      ),
    );
  }
}

class SlowScrollPhysics extends BouncingScrollPhysics {
  const SlowScrollPhysics({super.parent});

  @override
  double applyPhysicsToUserOffset(ScrollMetrics position, double offset) {
    return offset * 0.3;
  }
}
