import 'package:flutter/material.dart';
import 'package:vitalact/main.dart';

class IndexPage extends StatelessWidget {
  const IndexPage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Positioned(
            top: height * -.4,
            child: Image.asset('assets/images/Background.png', width: width),
          ),

          Positioned(
            top: height * -.39,
            left: width * -.3,
            height: height * 0.5,
            child: Image.asset('assets/images/Ellipse.png'),
          ),

          Positioned(
            top: height * .08,
            left: 0,
            right: 0,
            child: Center(
              child: Container(
                width: width * 0.9,
                height: 70,
                decoration: BoxDecoration(
                  boxShadow: [
                    BoxShadow(
                      color: Color(0xCCCC3838).withValues(alpha: 1),
                      blurRadius: 0,
                      offset: Offset(0, 5),
                    ),
                  ],
                  borderRadius: BorderRadius.circular(15),
                  color: Color(0xFFFF4646),
                ),
              ),
            ),
          ),

          Positioned.fill(top: height * .162, child: Lesson()),
        ],
      ),

      bottomNavigationBar: SafeArea(child: const BottomBar()),
    );
  }
}

class BottomBar extends StatefulWidget {
  const BottomBar({super.key});

  @override
  State<BottomBar> createState() => _BottomBarState();
}

class _BottomBarState extends State<BottomBar> {
  int selected = 2;

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    return Container(
      height: 65,
      padding: EdgeInsets.symmetric(horizontal: width * 0.09),
      decoration: BoxDecoration(
        border: Border(top: BorderSide(color: Color(0xFFFF4646), width: 3.5)),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          IconItems(
            path: 'assets/icons/profile.png',
            isSelected: selected == 0,
            onTap: () {
              setState(() => selected = 0);
            },
          ),
          IconItems(
            path: 'assets/icons/practice.png',
            isSelected: selected == 1,
            onTap: () {
              setState(() => selected = 1);
            },
          ),
          IconItems(
            path: 'assets/icons/home.png',
            isSelected: selected == 2,
            onTap: () {
              setState(() => selected = 2);
            },
          ),
          IconItems(
            path: 'assets/icons/setting.png',
            isSelected: selected == 3,
            onTap: () {
              setState(() => selected = 3);
            },
          ),
        ],
      ),
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

  void press(int step) {
    if (step == currentstep) {
      setState(() {
        currentstep++;
      });
    }
  }

  Widget button(int step) {
    bool active = step <= currentstep;
    final width = MediaQuery.of(context).size.width;

    return GestureDetector(
      onTap: active ? () => press(step) : null,
      child: Container(
        width: width * 0.8,
        height: 110,
        decoration: BoxDecoration(
          image: DecorationImage(
            image: AssetImage(
              active ? 'assets/images/press.png' : 'assets/images/Notpress.png',
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      physics: const SlowScrollPhysics(),
      child: Column(
        children: [
          Padding(padding: EdgeInsets.only(top: 50)),

          Container(
            padding: EdgeInsets.only(right: 110),
            child: Text(
              'RECOGNITION',
              style: TextStyle(
                fontSize: 35,
                fontWeight: FontWeight.w800,
                color: Color(0xFFFF4646),
                height: 0.65,
              ),
            ),
          ),
          SizedBox(height: 10),
          button(0),
          SizedBox(height: 60),
          button(1),
          SizedBox(height: 60),
          button(2),
          SizedBox(height: 60),
          button(3),
          SizedBox(height: 60),
          button(4),
          SizedBox(height: 60),
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
